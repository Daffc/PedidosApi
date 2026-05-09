using FluentAssertions;

using Domain.Enums;

namespace UnitTests.Domain.Enums;

public class StatusPedidoTests
{
    [Fact]
    public void Deve_Conter_Status_Novo()
    {
        var exists = Enum.IsDefined(
            typeof(StatusPedido),
            StatusPedido.Novo);

        exists.Should().BeTrue();
    }

    [Fact]
    public void Deve_Conter_Status_Pago()
    {
        var exists = Enum.IsDefined(
            typeof(StatusPedido),
            StatusPedido.Pago);

        exists.Should().BeTrue();
    }

    [Fact]
    public void Deve_Conter_Status_Cancelado()
    {
        var exists = Enum.IsDefined(
            typeof(StatusPedido),
            StatusPedido.Cancelado);

        exists.Should().BeTrue();
    }

    [Fact]
    public void Deve_Possuir_Quantidade_Correta_De_Status()
    {
        var values = Enum.GetValues<StatusPedido>();

        values.Should().HaveCount(3);
    }

    [Fact]
    public void Deve_Possuir_Nomes_Corretos()
    {
        var names = Enum.GetNames<StatusPedido>();

        names.Should().Contain([
            "Novo",
            "Pago",
            "Cancelado"
        ]);
    }

    [Fact]
    public void Deve_Possuir_Valores_Unicos()
    {
        var values = Enum.GetValues<StatusPedido>();

        values.Distinct().Should().HaveCount(values.Length);
    }

    [Fact]
    public void Deve_Converter_Enum_Para_String_Corretamente()
    {
        var status = StatusPedido.Pago;

        var result = status.ToString();

        result.Should().Be("Pago");
    }

    [Fact]
    public void Deve_Converter_String_Para_Enum_Corretamente()
    {
        var value = "Cancelado";

        var result = Enum.Parse<StatusPedido>(value);

        result.Should().Be(StatusPedido.Cancelado);
    }

    [Fact]
    public void Deve_Falhar_Ao_Converter_String_Invalida()
    {
        var value = "Inexistente";

        var action = () => Enum.Parse<StatusPedido>(value);

        action.Should()
            .Throw<ArgumentException>();
    }
}
