using Application.DTOs.Common.Pagination;
using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Repositories;

public interface IPedidoRepository
{
    Task CreateAsync(Pedido pedido);
    Task UpdateAsync(Pedido pedido);
    Task<Pedido?> GetByIdAsync(Guid id);
    Task<PagedResult<Pedido>> GetPagedAsync(
        StatusPedido? statusPedido,
        int page,
        int pageSize
    );
}