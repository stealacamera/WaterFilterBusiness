﻿using FluentResults;

namespace WaterFilterBusiness.Common.Errors;

public static class UserErrors
{
    public static Error NotFound => GeneralErrors.NotFoundError("User");
    public static Error RoleNotFound => new Error("The given role does not exist");
}