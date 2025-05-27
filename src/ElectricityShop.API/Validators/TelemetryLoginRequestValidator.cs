using FluentValidation;

namespace ElectricityShop.API.Validators
{
    /// <summary>
    /// Validator for the telemetry login request
    /// </summary>
    public class TelemetryLoginRequestValidator : AbstractValidator<Models.TelemetryLoginRequest>
    {
        public TelemetryLoginRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required");
        }
    }
}