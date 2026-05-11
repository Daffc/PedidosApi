using Application.DTOs.Requests;
using Application.Validators.Common;
using Application.Validators.Requests;
using FluentValidation.TestHelper;

namespace UnitTests.Application.Validators.Requests;

public sealed class ItemPedidoRequestValidatorTests
{
    private readonly ItemPedidoRequestValidator _validator = new();

    [Fact]
    public void NaoDeve_GerarErro_Quando_RequisicaoValida()
    {
        var request = new ItemPedidoRequest(
            "Item",
            1,
            5000m
        );

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Deve_GerarErro_Quando_ProdutoNomeVazio(string produtoNome)
    {
        var request = new ItemPedidoRequest(
            produtoNome,
            1,
            5000m
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ProdutoNome)
            .WithErrorMessage(ValidationMessages.RequiredField);
    }

    [Fact]
    public void Deve_GerarErro_Quando_ProdutoNomeMaiorQueMaximo()
    {
        var produtoNome = new string('A', 201);

        var request = new ItemPedidoRequest(
            produtoNome,
            1,
            5000m
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ProdutoNome)
            .WithErrorMessage(
                ValidationMessages.ItemPedidoProdutoNomeMaxLength
            );
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public void NaoDeve_GerarErro_Quando_QuantidadeValida(int quantidade)
    {
        var request = new ItemPedidoRequest(
            "Item",
            quantidade,
            5000m
        );

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.Quantidade);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Deve_GerarErro_Quando_QuantidadeInvalida(int quantidade)
    {
        var request = new ItemPedidoRequest(
            "Item",
            quantidade,
            5000m
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Quantidade)
            .WithErrorMessage(
                ValidationMessages.InvalidItemPedidoQuantidade
            );
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(10)]
    [InlineData(5000)]
    public void NaoDeve_GerarErro_Quando_PrecoUnitarioValida(decimal precoUnitario)
    {
        var request = new ItemPedidoRequest(
            "Item",
            1,
            precoUnitario
        );

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.PrecoUnitario);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Deve_GerarErro_Quando_PrecoUnitarioInvalido(decimal precoUnitario)
    {
        var request = new ItemPedidoRequest(
            "Item",
            1,
            precoUnitario
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.PrecoUnitario)
            .WithErrorMessage(
                ValidationMessages.InvalidItemPedidoPrecoUnitario
            );
    }
}