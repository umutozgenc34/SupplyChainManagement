using FluentValidation;

namespace SupplyChainManagement.Application.Features.Orders.Commands.Create;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.OrderItems)
            .NotEmpty().WithMessage("Order must contain at least one item.")
            .Must(items => items.All(i => i.Quantity > 0)).WithMessage("Each order item must have a quantity greater than zero.");
    }
}
