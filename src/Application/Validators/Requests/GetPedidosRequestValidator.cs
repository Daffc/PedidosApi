using FluentValidation;

using Application.DTOs.Requests;
using Application.Validators.Common;
using Domain.Enums;

namespace Application.Validators.Requests;

public sealed class GetPedidosRequestValidator : AbstractValidator<GetPedidosRequest>
{
    public GetPedidosRequestValidator()
    {
        RuleFor(x => x.StatusPedido)
            .Must(BeAValidStatus)
            .When(x => !string.IsNullOrWhiteSpace(x.StatusPedido))
            .WithMessage(ValidationMessages.InvalidPedidoStatus);

        Include(new PaginationRequestValidator());
    }

    private static bool BeAValidStatus(string? status)
    {
         if( Enum.TryParse<StatusPedido>( status, ignoreCase: true, out var result))
        {
            return Enum.IsDefined(typeof(StatusPedido), result);
        }

        return false;
    }
}