using FluentValidation;

namespace SupplyChainManagement.Application.Features.Suppliers.Queries.GetAll;

public class GetAllSuppliersQueryValidator : AbstractValidator<GetAllSuppliersQuery>
{
    public GetAllSuppliersQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than zero.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
    }
}
