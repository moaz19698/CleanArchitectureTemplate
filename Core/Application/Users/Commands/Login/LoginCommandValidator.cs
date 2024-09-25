using CleanArchitecture.Application.Common.Abstracts;
using FluentValidation;

namespace CleanArchitecture.Application.Users.Commands.Login
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        private IIdentityService IdentityService { get; }

        public LoginCommandValidator(IIdentityService identityService)
        {
            IdentityService = identityService;

            RuleFor(c => c.UserName)
                .NotEmpty()
                .MustAsync(async (username, cancellation) =>
                {
                    return !string.IsNullOrEmpty(await IdentityService.GetUserAsync(username!));
                })
                .WithMessage("invalid username or password");

            RuleFor(c => c.Password)
                .NotEmpty()
                .MustAsync(async (context, password, cancellation) =>
                {
                    try
                    {
                        return await IdentityService.CheckPasswordAsync(context.UserName!, password);
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }).WithMessage(m => "invalid username or password");
        }
    }
}
