using Microsoft.EntityFrameworkCore;

using Application.Common.Pagination;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

public sealed class PedidoRepository : IPedidoRepository
{
    private readonly AppDbContext _dbContext;

    public PedidoRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateAsync(Pedido pedido)
    {
        await _dbContext.Pedidos.AddAsync(pedido);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Pedido pedido)
    {
        _dbContext.Pedidos.Update(pedido);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Pedido?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Pedidos
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<PagedResult<Pedido>> GetPagedAsync(StatusPedido? statusPedido, int page, int pageSize)
    {
        IQueryable<Pedido> query = _dbContext.Pedidos
            .Include( p => p.Itens)
            .AsNoTracking();

        if (statusPedido.HasValue)
        {
            query = query.Where(
                p => p.Status == statusPedido.Value
            );
        }

        var totalItems = await query.CountAsync();

        var items = await query
            .OrderByDescending( p => p.DataCriacao)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Pedido>(
            items,
            totalItems,
            page,
            pageSize
        );
    }
}