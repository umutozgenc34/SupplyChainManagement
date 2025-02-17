using FluentValidation;

namespace SupplyChainManagement.Application.Features.Suppliers.Commands.Update;

public class UpdateSupplierCommandValidator : AbstractValidator<UpdateSupplierCommand>
{
    public UpdateSupplierCommandValidator()
    {
        RuleFor(s=> s.Id).NotEmpty()
            .WithMessage("Id is required.");

        RuleFor(s => s.Name)
           .NotEmpty().WithMessage("Supplier name is required.")
           .MaximumLength(100).WithMessage("Supplier name cannot exceed 100 characters.");

        RuleFor(s => s.ContactEmail)
            .NotEmpty().WithMessage("Contact email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MaximumLength(255).WithMessage("Contact email cannot exceed 255 characters.");

        RuleFor(s => s.Phone)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\d{11}$").WithMessage("Phone number must be exactly 11 digits.");
    }
}
