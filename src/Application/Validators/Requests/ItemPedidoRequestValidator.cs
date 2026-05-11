using FluentValidation;

using Application.DTOs.Requests;
using Application.Validators.Common;

namespace Application.Validators.Requests;

public sealed class ItemPedidoRequestValidator : AbstractValidator<ItemPedidoRequest>
{
    public ItemPedidoRequestValidator()
    {
        RuleFor(x => x.ProdutoNome)
            .NotEmpty()
            .WithMessage(ValidationMessages.RequiredField)
            .MaximumLength(200)
            .WithMessage(ValidationMessages.ItemPedidoProdutoNomeMaxLength);

        RuleFor(x => x.Quantidade)
            .GreaterThan(0)
            .WithMessage(ValidationMessages.InvalidItemPedidoQuantidade);

        RuleFor(x => x.PrecoUnitario)
            .GreaterThan(0)
            .WithMessage(ValidationMessages.InvalidItemPedidoPrecoUnitario);
    }
}