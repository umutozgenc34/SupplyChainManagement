using FluentValidation;

namespace SupplyChainManagement.Application.Features.Orders.Commands.ChangeStatus;

public class ChangeOrderStatusCommandValidator : AbstractValidator<ChangeOrderStatusCommand>
{
    public ChangeOrderStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid order status.");
    }
}
