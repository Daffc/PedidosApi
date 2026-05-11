using FluentValidation;

using Application.DTOs.Requests;
using Application.Validators.Common;

namespace Application.Validators.Requests;

public sealed class CreatePedidoRequestValidator : AbstractValidator<CreatePedidoRequest>
{
    public CreatePedidoRequestValidator()
    {
        RuleFor(x => x.ClienteNome)
            .NotEmpty()
            .WithMessage(ValidationMessages.RequiredField)
            .MaximumLength(200)
            .WithMessage(ValidationMessages.PedidoClienteNomeMaxLength);

        RuleFor(x => x.Itens)
            .NotNull()
            .WithMessage(ValidationMessages.RequiredField)
            .Must(itens => itens is not null && itens.Any())
            .WithMessage(ValidationMessages.EmptyPedidoItens);

        RuleForEach( x => x.Itens)
            .SetValidator(new ItemPedidoRequestValidator());
    }
}
