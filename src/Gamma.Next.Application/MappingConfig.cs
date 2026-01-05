using Mapster;

namespace Gamma.Next.Application;

public sealed class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Product.Commands.ProductInput, Domain.Entities.Product>()
            .Ignore(dest => dest.Id);

        config.NewConfig<ProductGroup.Commands.ProductGroupInput, Domain.Entities.ProductGroup>()
           .Ignore(dest => dest.Id);
    }
}