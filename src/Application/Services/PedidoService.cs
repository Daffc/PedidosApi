using Application.Common.Pagination;
using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public sealed class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IMapper _mapper;

    public PedidoService(
        IPedidoRepository pedidoRepository,
        IMapper mapper
    )
    {
        _pedidoRepository = pedidoRepository;
        _mapper = mapper;
    }

    public async Task<PedidoResponse> CreateAsync(CreatePedidoRequest request)
    {
        var itens = request.Itens
            .Select(item => new ItemPedido(
                item.ProdutoNome,
                item.Quantidade,
                item.PrecoUnitario
            ))
            .ToList();

        var pedido = new Pedido(
            request.ClienteNome,
            itens
        );

        await _pedidoRepository.CreateAsync(pedido);

        return _mapper.Map<PedidoResponse>(pedido);
    }

    public async Task<PedidoResponse> GetByIdAsync(Guid id)
    {
        var pedido = await _pedidoRepository.GetByIdAsync(id);

        if (pedido is null)
            throw new KeyNotFoundException($"Pedido '{id}' não encontrado.");

        return _mapper.Map<PedidoResponse>(pedido);
    }

    public async Task<PagedResponse<PedidoResponse>> GetPagedAsync(GetPedidosRequest request)
    {
        StatusPedido? statusPedido = null;

        if (!string.IsNullOrWhiteSpace(request.StatusPedido))
        {
            var parseSucceeded = Enum.TryParse<StatusPedido>(
                request.StatusPedido,
                ignoreCase: true,
                out var parsedStatusPedido
            );

            if (!parseSucceeded)
            {
                throw new ArgumentException("StatusPedido inválido.");
            }

            statusPedido = parsedStatusPedido;
        }

        var pagedResult = await _pedidoRepository.GetPagedAsync(
            statusPedido,
            request.Page,
            request.PageSize
        );

        var items = _mapper.Map<IReadOnlyCollection<PedidoResponse>>(pagedResult.Items);

        return new PagedResponse<PedidoResponse>(
            pagedResult.Page,
            pagedResult.PageSize,
            pagedResult.TotalItems,
            pagedResult.TotalPages,
            items
        );
    }

    public async Task CancelAsync(Guid id)
    {
        var pedido = await _pedidoRepository.GetByIdAsync(id);

        if (pedido is null)
        {
            throw new KeyNotFoundException($"Pedido '{id}' não encontrado.");
        }

        pedido.Cancelar();

        await _pedidoRepository.UpdateAsync(pedido);
    }
}