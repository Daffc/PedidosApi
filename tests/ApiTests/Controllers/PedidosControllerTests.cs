using System.Net;
using System.Net.Http.Json;
using ApiTests.Fixtures;
using Application.DTOs.Requests;
using Application.DTOs.Responses;
using FluentAssertions;

namespace ApiTests.Controllers;

public sealed class PedidosControllerTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public PedidosControllerTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task POST_pedidos_Deve_CriarPedido_E_Retornar_201_Quando_PedidoValido()
    {
        var request = new CreatePedidoRequest(
            "Cliente",
            new[]
            {
                new ItemPedidoRequest("Item", 2, 10m)
            }
        );

        var response = await _client.PostAsJsonAsync("/pedidos", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<PedidoResponse>();

        body.Should().NotBeNull();
        body!.ClienteNome.Should().Be("Cliente");
        body.Itens.Should().HaveCount(1);
    }

    [Fact]
    public async Task POST_pedidos_Deve_Retornar_400__Quando_RequestInvalido()
    {
        var request = new CreatePedidoRequest(
            "",
            new List<ItemPedidoRequest>()
        );

        var response = await _client.PostAsJsonAsync("/pedidos", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var body = await response.Content.ReadAsStringAsync();

        body.Should().Contain("ClienteNome");
        body.Should().Contain("Itens");
    }

    [Fact]
    public async Task GET_pedido_Deve_Retornar_200_Quando_PedidoExistente()
    {
        var createRequest = new CreatePedidoRequest(
            "Cliente",
            new[]
            {
                new ItemPedidoRequest("Item", 1, 20m)
            }
        );

        var createResponse = await _client.PostAsJsonAsync("/pedidos", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<PedidoResponse>();

        var response = await _client.GetAsync($"/pedidos/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<PedidoResponse>();

        body.Should().NotBeNull();
        body!.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task GET_pedido_id_Deve_Retornar_404_Quando_PedidoInexistente()
    {
        var response = await _client.GetAsync($"/pedidos/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var body = await response.Content.ReadAsStringAsync();

        body.Should().Contain("traceId");
    }

    [Fact]
    public async Task PATCH_cancelar_Deve_Retornar_204_Quando_PedidoValido()
    {
        var createRequest = new CreatePedidoRequest(
            "Cliente",
            new[]
            {
                new ItemPedidoRequest("Item", 1, 50m)
            }
        );

        var createResponse = await _client.PostAsJsonAsync("/pedidos", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<PedidoResponse>();

        var response = await _client.PatchAsync($"/pedidos/{created!.Id}/cancelar", null);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task PATCH_pagar_Deve_Retornar_204_Quando_PedidoValido()
    {
        var createRequest = new CreatePedidoRequest(
            "Cliente",
            new[]
            {
                new ItemPedidoRequest("Item", 1, 50m)
            }
        );

        var createResponse = await _client.PostAsJsonAsync("/pedidos", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<PedidoResponse>();

        var response = await _client.PatchAsync($"/pedidos/{created!.Id}/pagar", null);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GETdeve_retornar_200_Quando_RequestValido()
    {
        var response = await _client.GetAsync("/pedidos?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadAsStringAsync();

        body.Should().Contain("items");
        body.Should().Contain("page");
    }
}