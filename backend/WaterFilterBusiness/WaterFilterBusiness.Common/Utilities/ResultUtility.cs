using FluentResults;
using Microsoft.AspNetCore.Identity;

namespace WaterFilterBusiness.Common.Utilities;

public static class ResultUtility
{
    public static Dictionary<string, string[]> GetErrorsDictionary(this Result result)
    {
        return result.Errors
                     .ToDictionary(
                        e => e.Message,
                        e => e.Reasons.Select(e => e.Message).ToArray());
    }

    public static Dictionary<string, string[]> GetErrorsDictionary<T>(this Result<T> result)
    {
        return result.Errors
                     .ToDictionary(
                        e => e.Message,
                        e => e.Reasons.Select(e => e.Message).ToArray());
    }

    public static Result CreateResult(IdentityErrorDescriber errorDescriber, IEnumerable<IdentityError> identityErrors)
    {
        var errors = new Dictionary<string, Error>();

        foreach (var error in identityErrors)
        {
            string errorProperty = GetPropertyOfError(errorDescriber, error.Code);
            Error reason = new Error(error.Description);

            if (errors.ContainsKey(errorProperty))
                errors[errorProperty].Reasons.Add(reason);
            else
                errors.Add(errorProperty, new Error(errorProperty, reason));
        }

        return Result.Fail(errors.Values.ToList());
    }

    private static string GetPropertyOfError(IdentityErrorDescriber errorDescriber, string code)
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