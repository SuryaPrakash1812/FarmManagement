using FarmManagement.Api.Dtos;
using FluentValidation;

namespace FarmManagement.Api.Validators;

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator() { RuleFor(x => x.Email).NotEmpty().EmailAddress(); RuleFor(x => x.Password).NotEmpty(); }
}

public sealed class AnimalRequestValidator : AbstractValidator<UpsertAnimalRequest>
{
    public AnimalRequestValidator()
    {
        RuleFor(x => x.TagNumber).NotEmpty().MaximumLength(40);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Species).NotEmpty().MaximumLength(80);
        RuleFor(x => x.PurchasePrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CurrentValue).GreaterThanOrEqualTo(0);
    }
}

public sealed class StockRequestValidator : AbstractValidator<UpsertStockItemRequest>
{
    public StockRequestValidator() { RuleFor(x => x.ItemName).NotEmpty(); RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0); RuleFor(x => x.ReorderLevel).GreaterThanOrEqualTo(0); }
}
