using Application.DTOs.Responses;
using AutoMapper;

using Domain.Entities;

namespace Application.Mappings;

public sealed class PedidoProfile : Profile
{
    public PedidoProfile()
    {
        CreateMap<ItemPedido, PedidoItemResponse>()
            .ForCtorParam(
                nameof(PedidoItemResponse.Id),
                opt => opt.MapFrom(src => src.Id)
            )
            .ForCtorParam(
                nameof(PedidoItemResponse.ProdutoNome),
                opt => opt.MapFrom(src => src.ProdutoNome)
            )
            .ForCtorParam(
                nameof(PedidoItemResponse.Quantidade),
                opt => opt.MapFrom(src => src.Quantidade)
            )
            .ForCtorParam(
                nameof(PedidoItemResponse.PrecoUnitario),
                opt => opt.MapFrom(src => src.PrecoUnitario)
            )
            .ForCtorParam(
                nameof(PedidoItemResponse.Subtotal),
                opt => opt.MapFrom(src => src.Subtotal)
            );

        CreateMap<Pedido, PedidoResponse>()
            .ForCtorParam(
                nameof(PedidoResponse.Id),
                opt => opt.MapFrom(src => src.Id)
            )
            .ForCtorParam(
                nameof(PedidoResponse.ClienteNome),
                opt => opt.MapFrom(src => src.ClienteNome)
            )
            .ForCtorParam(
                nameof(PedidoResponse.DataCriacao),
                opt => opt.MapFrom(src => src.DataCriacao)
            )
            .ForCtorParam(
                nameof(PedidoResponse.Status),
                opt => opt.MapFrom(src => src.Status.ToString())
            )
            .ForCtorParam(
                nameof(PedidoResponse.ValorTotal),
                opt => opt.MapFrom(src => src.ValorTotal)
            )
            .ForCtorParam(
                nameof(PedidoResponse.Itens),
                opt => opt.MapFrom(src => src.Itens)
            );
    }
}