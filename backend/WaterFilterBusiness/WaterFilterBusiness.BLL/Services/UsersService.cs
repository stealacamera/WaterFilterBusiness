using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.DTOs.ViewModels;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.Results;
using WaterFilterBusiness.DAL;

namespace WaterFilterBusiness.BLL.Services;

public interface IUsersService
{
    Task<OffsetPaginatedList<User>> GetAllAsync(int page, int pageSize, bool excludeDeleted = true);
    Task<Result<User>> AddAsync(UserAddRequestModel user);
    Task<Result> AddUserToRole(int userId, Role role);
    Task<Result> RemoveAsync(int id);
}

internal sealed class UsersService : Service, IUsersService
{
    private readonly IdentityErrorDescriber _identityErrorDescriber;

    public UsersService(IServiceProvider serviceProvider) : base(serviceProvider.GetRequiredService<IWorkUnit>())
    {
        _identityErrorDescriber = serviceProvider.GetRequiredService<UserManager<DAL.Entities.User>>()
                                                 .ErrorDescriber;
    }

    public async Task<Result<User>> AddAsync(UserAddRequestModel model)
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
            return new IdentityErrorsResult(_identityErrorDescriber, result.Errors);

        await _workUnit.SaveChangesAsync();

        return Result.Ok(new User
        {
            Id = entity.Id,
            Username = entity.UserName,
            Name = entity.Name,
            Surname = entity.Surname,
            Email = entity.Email
        });
    }

    public async Task<Result> AddUserToRole(int userId, Role role)
    {
        var user = await _workUnit.UsersRepository.GetByIdAsync(userId);

        if (user == null)
            return GeneralResults.NotFoundFailResult(nameof(User));

        var result = await _workUnit.UsersRepository.AddToRoleAsync(user, role);

        if (!result.Succeeded)
        {
            var errors = string.Join("\n", result.Errors.Select(e => $"{e.Code}: {e.Description}").ToArray());
            throw new InvalidOperationException(errors);
        }

        await _workUnit.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<OffsetPaginatedList<User>> GetAllAsync(int page, int pageSize, bool excludeDeleted = true)
    {
        var result = await _workUnit.UsersRepository.GetAllAsync(page, pageSize, excludeDeleted);

        var values = result.Values
                           .Select(async e => new User
                           {
                               Id = e.Id,
                               Email = e.Email,
                               Name = e.Name,
                               Surname = e.Surname,
                               Username = e.UserName,
                               Role = (await _workUnit.UsersRepository.GetRoleAsync(e)).Name
                           })
                           .Select(e => e.Result)
                           .ToList();

        return new OffsetPaginatedList<User>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = result.TotalCount,
            Values = values
        };
    }

    public async Task<Result> RemoveAsync(int id)
    {
        var user = await _workUnit.UsersRepository.GetByIdAsync(id);

        if (user == null || user.DeletedAt != null)
            return GeneralResults.NotFoundFailResult(nameof(User));

        user.DeletedAt = DateTime.Now;
        await _workUnit.SaveChangesAsync();

        return Result.Ok();
    }
}
