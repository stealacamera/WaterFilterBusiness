using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.DTOs.ViewModels;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.ErrorHandling.Errors;
using WaterFilterBusiness.Common.Utilities;
using WaterFilterBusiness.DAL;

namespace WaterFilterBusiness.BLL.Services.Identity;

public interface IUsersService
{
    Task<OffsetPaginatedList<User>> GetAllAsync(int page, int pageSize, bool excludeDeleted = true);
    Task<Result<User>> GetByIdAsync(int id);
    Task<Result<Role>> GetRoleAsync(int id);
    Task<Result> RemoveAsync(int id);

    Task<Result<User>> AddAsync(User_AddRequestModel user);
    Task<Result<User>> AddUserToRole(int userId, string roleName);
    Task<Result<User>> AddUserToRole(int userId, Role role);

    Task<Result<User>> GetByCredentials(LoginCredentials credentials);
}

internal sealed class UsersService : Service, IUsersService
{
    private readonly IdentityErrorDescriber _identityErrorDescriber;

    public UsersService(
        IWorkUnit workUnit,
        IUtilityService utilityService,
        IServiceProvider serviceProvider) : base(workUnit, utilityService)
    {
        _identityErrorDescriber = serviceProvider.GetRequiredService<UserManager<DAL.Entities.User>>()
                                                 .ErrorDescriber;
    }

    public async Task<Result<User>> GetByIdAsync(int id)
    {
        var dbModel = await _workUnit.UsersRepository.GetByIdAsync(id);

        if (dbModel == null)
            return UserErrors.NotFound(nameof(id));

        return new User
        {
            Id = dbModel.Id,
            Username = dbModel.UserName,
            Name = dbModel.Name,
            Surname = dbModel.Surname,
            Email = dbModel.Email,
            Role = (await _workUnit.UsersRepository.GetRoleAsync(dbModel)).Name
        };
    }

    public async Task<Result<User>> AddAsync(User_AddRequestModel model)
    {
        var entity = new DAL.Entities.User
        {
            UserName = model.Username,
            Name = model.Name,
            Surname = model.Surname,
            Email = model.Email,
            EmailConfirmed = true
        };

        var result = await _workUnit.UsersRepository.AddAsync(entity, model.Password);

        if (!result.Succeeded)
            return ResultUtility.CreateResult(_identityErrorDescriber, result.Errors);

        await _workUnit.SaveChangesAsync();

        return new User
        {
            Id = entity.Id,
            Email = entity.Email,
            Name = entity.Name,
            Surname = entity.Surname,
            Username = entity.UserName
        };
    }

    public async Task<Result<User>> AddUserToRole(int userId, string roleName)
    {
        Role? role;

        if (!Role.TryFromName(roleName, ignoreCase: true, out role))
            return UserErrors.RoleNotFound(nameof(role));

        return await AddUserToRole(userId, role);
    }

    public async Task<Result<User>> AddUserToRole(int userId, Role role)
    {
        var user = await _workUnit.UsersRepository.GetByIdAsync(userId);

        if (user == null)
            return UserErrors.NotFound(nameof(userId));

        var result = await _workUnit.UsersRepository.AddToRoleAsync(user, role);

        if (!result.Succeeded)
        {
            var errors = string.Join("\n", result.Errors.Select(e => $"{e.Code}: {e.Description}").ToArray());
            throw new InvalidOperationException(errors);
        }

        await _workUnit.SaveChangesAsync();
        return ConvertEntityToModel(role, user);
    }

    public async Task<OffsetPaginatedList<User>> GetAllAsync(int page, int pageSize, bool excludeDeleted = true)
    {
        var result = await _workUnit.UsersRepository.GetAllAsync(page, pageSize, excludeDeleted);

        return new OffsetPaginatedList<User>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = result.TotalCount,
            Values = result.Values
                           .Select(ConvertEntityToModel)
                           .Select(e => e.Result)
                           .ToList()
        };
    }

    public async Task<Result> RemoveAsync(int id)
    {
        var user = await _workUnit.UsersRepository.GetByIdAsync(id);

        if (user == null || user.DeletedAt != null)
            return UserErrors.NotFound(nameof(id));

        user.DeletedAt = DateTime.Now;
        await _workUnit.SaveChangesAsync();

        return Result.Ok();
    }

    public async Task<Result<Role>> GetRoleAsync(int id)
    {
        var dbModel = await _workUnit.UsersRepository.GetByIdAsync(id);

        if (dbModel == null)
            return UserErrors.NotFound(nameof(id));

        return await _workUnit.UsersRepository.GetRoleAsync(dbModel);
    }

    public async Task<Result<User>> GetByCredentials(LoginCredentials credentials)
    {
        var user = await _workUnit.UsersRepository
                                  .GetByCredentials(credentials.Email, credentials.Password);

        if (user == null)
            return UserErrors.NotFound(string.Empty);

        return await ConvertEntityToModel(user);
    }

    private async Task<User> ConvertEntityToModel(DAL.Entities.User entity)
    {
        Role role = await _workUnit.UsersRepository.GetRoleAsync(entity);
        return ConvertEntityToModel(role, entity);
    }

    private User ConvertEntityToModel(Role role, DAL.Entities.User entity)
    {
        return new User
        {
            Id = entity.Id,
            Email = entity.Email,
            Name = entity.Name,
            Surname = entity.Surname,
            Username = entity.UserName,
            Role = role.Name
        };
    }
}