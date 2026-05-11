using Application.DTOs.Requests;
using Application.Validators.Common;
using Application.Validators.Requests;
using FluentValidation.TestHelper;

namespace UnitTests.Application.Validators.Requests;

public sealed class CreatePedidoRequestValidatorTests
{
    private readonly CreatePedidoRequestValidator _validator = new();

    [Fact]
    public void NaoDeve_GerarErro_Quando_RequisicaoValida()
    {
        var request = new CreatePedidoRequest(
            "Cliente",
            [
                new ItemPedidoRequest(
                    "Item",
                    1,
                    5000m
                )
            ]
        );

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Deve_GerarErro_Quando_ClienteNomeVazio(string clienteNome)
    {
        var request = new CreatePedidoRequest(
            clienteNome,
            [
                new ItemPedidoRequest(
                    "Item",
                    1,
                    5000m
                )
            ]
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ClienteNome)
            .WithErrorMessage(ValidationMessages.RequiredField
        );
    }

    [Fact]
    public void Deve_GerarErro_Quando_ClienteNomeMaiorQueMaximo()
    {
        var clienteNome = new string('A', 201);

        var request = new CreatePedidoRequest(
            clienteNome,
            [
                new ItemPedidoRequest(
                    "Item",
                    1,
                    5000m
                )
            ]
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ClienteNome)
            .WithErrorMessage(ValidationMessages.PedidoClienteNomeMaxLength);
    }

    [Fact]
    public void Deve_GerarErro_Quando_ItensVazio()
    {
        var request = new CreatePedidoRequest(
            "Cliente",
            []
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Itens)
            .WithErrorMessage(ValidationMessages.EmptyPedidoItens);
    }

    [Fact]
    public void Deve_GerarErro_Quando_ItensInvalido()
    {
        var request = new CreatePedidoRequest(
            "Cliente",
            [
                new ItemPedidoRequest(
                    "",
                    0,
                    0
                )
            ]
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor("Itens[0].ProdutoNome");
        result.ShouldHaveValidationErrorFor("Itens[0].Quantidade");
        result.ShouldHaveValidationErrorFor("Itens[0].PrecoUnitario");
    }
}