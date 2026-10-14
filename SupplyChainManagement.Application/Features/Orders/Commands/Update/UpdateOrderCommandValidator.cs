using FluentValidation;

namespace SupplyChainManagement.Application.Features.Orders.Commands.Update;

public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.OrderItems)
            .NotEmpty().WithMessage("Order must contain at least one item.")
            .Must(items => items.All(i => i.Quantity > 0)).WithMessage("Each order item must have a quantity greater than zero.");
    }
}
