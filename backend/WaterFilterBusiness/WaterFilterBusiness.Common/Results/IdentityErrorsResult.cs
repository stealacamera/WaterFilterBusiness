using FluentResults;
using Microsoft.AspNetCore.Identity;
using WaterFilterBusiness.Common.DTOs;

namespace WaterFilterBusiness.Common.Results;

public class IdentityErrorsResult : Result<User>
{
    public Dictionary<string, IList<string>> BaseErrors { get; } = new();

    public IdentityErrorsResult(
        IdentityErrorDescriber errorDescriber,
        IEnumerable<IdentityError> identityErrors)
    {
        if (!identityErrors.Any())
            return;

        var errors = new Dictionary<string, Error>();

        /*
         * For each error,
         * get the attribute it targets (e.g.: password, email, etc.)
         * and group the errors for each attribute
         */
        foreach (var error in identityErrors)
        {
            string errorProperty = GetPropertyOfError(errorDescriber, error.Code);
            Error reason = new Error(error.Description);

            if (errors.ContainsKey(errorProperty))
            {
                errors[errorProperty].Reasons.Add(reason);
                BaseErrors[errorProperty].Add(error.Description);
            }
            else
            {
                errors.Add(errorProperty, new Error(errorProperty, reason));
                BaseErrors.Add(errorProperty, new List<string> { error.Description });
            }
        }
        
        Reasons.AddRange(errors.Values.ToList());
    }

    private string GetPropertyOfError(IdentityErrorDescriber errorDescriber, string code)
    {
        switch (code)
        {
            case nameof(errorDescriber.DefaultError):
                return "Default";
            case nameof(errorDescriber.DuplicateEmail):
            case nameof(errorDescriber.InvalidEmail):
                return "Email";
            case nameof(errorDescriber.DuplicateUserName):
            case nameof(errorDescriber.InvalidUserName):
                return "Username";
            case nameof(errorDescriber.PasswordRequiresDigit):
            case nameof(errorDescriber.PasswordRequiresLower):
            case nameof(errorDescriber.PasswordRequiresUpper):
            case nameof(errorDescriber.PasswordRequiresNonAlphanumeric):
            case nameof(errorDescriber.PasswordRequiresUniqueChars):
            case nameof(errorDescriber.PasswordTooShort):
                return "Password";
            default:
                return "Other";
        }
    }
}
