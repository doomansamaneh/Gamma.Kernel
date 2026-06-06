using Dapper;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Data;

namespace Gamma.Next.Infra.Repositories.Ast;

public sealed class ProductRepository(
    ICurrentUser currentUser,
    IUidGenerator uidGenerator,
    ISystemClock clock)
    : GenericRepository<Domain.Entities.Ast.Product>(currentUser, uidGenerator, clock)
    , Domain.Interfaces.Ast.IProductRepository;