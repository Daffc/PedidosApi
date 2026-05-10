using Application.DTOs.Requests;
using Application.DTOs.Responses;

namespace Application.Interfaces.Services;

public interface IPedidoService
{
    Task<PedidoResponse> CreateAsync(CreatePedidoRequest request);
    Task<PedidoResponse> GetByIdAsync(Guid id);
    Task<PagedResponse<PedidoResponse>> GetPagedAsync(GetPedidosRequest request);
    Task CancelAsync(Guid id);
}