using Application.DTOs.Requests;
using Application.Validators.Common;
using Application.Validators.Requests;
using FluentValidation.TestHelper;

namespace UnitTests.Application.Validators.Requests;

public sealed class GetPedidosRequestValidatorTests
{
    private readonly GetPedidosRequestValidator _validator = new();

    [Fact]
    public void NaoDeve_GerarErro_Quando_RequisicaoValida()
    {
        var request = new GetPedidosRequest(
            "Pago",
            1,
            10
        );

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("Novo")]
    [InlineData("Pago")]
    [InlineData("Cancelado")]
    [InlineData("novo")]
    [InlineData("pago")]
    [InlineData("cancelado")]
    public void NaoDeve_GerarErro_Quando_StatusPedidoValido(string statusPedido)
    {
        var request = new GetPedidosRequest(
            statusPedido,
            1,
            10
        );

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.StatusPedido);
    }

    [Theory]
    [InlineData("ABC")]
    [InlineData("Teste")]
    [InlineData("123")]
    public void Deve_GerarErro_Quando_StatusPedidoInvalido(string statusPedido)
    {
        var request = new GetPedidosRequest(
            statusPedido,
            1,
            10
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.StatusPedido)
            .WithErrorMessage(ValidationMessages.InvalidPedidoStatus);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(10)]
    public void NaoDeve_GerarErro_Quando_PageValido(int page)
    {
        var request = new GetPedidosRequest(
            "Pago",
            page,
            10
        );

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.Page);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Deve_GerarErro_Quando_PageInvalido(int page)
    {
        var request = new GetPedidosRequest(
            "Pago",
            page,
            10
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Page)
            .WithErrorMessage(ValidationMessages.InvalidPage);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void NaoDeve_GerarErro_Quando_PageSizeValido(int pageSize)
    {
        var request = new GetPedidosRequest(
            "Pago",
            1,
            pageSize
        );

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Deve_GerarErro_Quando_PageSizeInvalido(int pageSize)
    {
        var request = new GetPedidosRequest(
            "Pago",
            1,
            pageSize
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage(
                ValidationMessages.InvalidPageSize
            );
    }
}