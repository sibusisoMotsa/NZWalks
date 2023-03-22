using FluentValidation;

namespace NZWalks.API.Validators
{
    public class AddWalkValidator: AbstractValidator<Models.DTO.AddWalkRequest>
    {
        public AddWalkValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Length).GreaterThan(0);
        }
    }
}
