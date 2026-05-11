using FluentValidation;

using Application.DTOs.Common.Pagination;

namespace Application.Validators.Common;

public sealed class PaginationRequestValidator : AbstractValidator<PaginationRequest>
{
    public PaginationRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage(ValidationMessages.InvalidPage);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage(ValidationMessages.InvalidPageSize);
    }
}