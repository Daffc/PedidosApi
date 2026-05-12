using System.Net;
using System.Net.Http.Json;
using ApiTests.Fixtures;
using Application.DTOs.Requests;
using Application.DTOs.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiTests.Middleware;

public sealed class GlobalExceptionHandlingMiddlewareTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public GlobalExceptionHandlingMiddlewareTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GET_pedidos_Deve_Retornar_200_Quando_RequisicaoValida()
    {
        var response = await _client.GetAsync("/pedidos");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GET_pedido_id_Deve_Retornar_404_Quando_PedidoInexistente()
    {
        var invalidId = Guid.NewGuid();

        var response = await _client.GetAsync($"/pedidos/{invalidId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails!.Status.Should().Be(StatusCodes.Status404NotFound);
        problemDetails.Title.Should().Be("Not Found");
        problemDetails.Extensions.Should().ContainKey("traceId");
    }

    [Fact]
    public async Task POST_pedidos_Deve_Retornar_400_Quando_RequestInvalido()
    {
        var invalidRequest = new { clienteNome = "" 

        var response = await _client.PostAsJsonAsync("/pedidos", invalidRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails!.Status.Should().Be(StatusCodes.Status400BadRequest);
        problemDetails.Title.Should().NotBeEmpty();
        problemDetails.Extensions.Should().ContainKey("traceId");
    }

    [Fact]
    public async Task GET_pedido_erro_Deve_Conter_TraceId()
    {
        var invalidId = Guid.NewGuid();

        var response = await _client.GetAsync($"/pedidos/{invalidId}");

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        
        problemDetails.Should().NotBeNull();
        problemDetails!.Extensions.Should().ContainKey("traceId");
        problemDetails.Extensions["traceId"].Should().NotBeNull();
    }

    [Fact]
    public async Task GET_pedido_erro_Deve_Retornar_ContentType_ApplicationJson()
    {
        var invalidId = Guid.NewGuid();

        var response = await _client.GetAsync($"/pedidos/{invalidId}");

        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async Task GET_pedido_erro_Deve_Conter_InstancePath()
    {
        var invalidId = Guid.NewGuid();

        var response = await _client.GetAsync($"/pedidos/{invalidId}");

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        
        problemDetails.Should().NotBeNull();
        problemDetails!.Instance.Should().Contain($"/pedidos/{invalidId}");
    }

    [Fact]
    public async Task PATCH_cancelar_Deve_Retornar_400_Quando_PedidoJaPago()
    {
        var createRequest = new CreatePedidoRequest(
            "Cliente",
            new[]
            {
                new ItemPedidoRequest("Item", 1, 20m)
            }
        );

        var createResponse = await _client.PostAsJsonAsync("/pedidos", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var pedido = await createResponse.Content.ReadFromJsonAsync<PedidoResponse>();

        var payResponse = await _client.PatchAsync($"/pedidos/{pedido!.Id}/pagar", null);
        payResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var cancelResponse = await _client.PatchAsync($"/pedidos/{pedido!.Id}/cancelar", null);

        cancelResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var problemDetails = await cancelResponse.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails!.Status.Should().Be(StatusCodes.Status400BadRequest);
        problemDetails.Title.Should().Be("Bad Request");
    }
}