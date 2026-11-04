using FluentValidation;

namespace RestaurantSaaS.Application.Common;

public static class PasswordRules
{
    /// <summary>At least 8 characters with an upper-case letter, a lower-case letter and a digit.</summary>
    public static IRuleBuilderOptions<T, string> MustBeStrongPassword<T>(this IRuleBuilder<T, string> rule) =>
        rule.NotEmpty().WithMessage("A password is required.")
            .MinimumLength(8).WithMessage("The password must have at least 8 characters.")
            .Matches("[A-Z]").WithMessage("The password needs an upper-case letter.")
            .Matches("[a-z]").WithMessage("The password needs a lower-case letter.")
            .Matches("[0-9]").WithMessage("The password needs a digit.");
}
