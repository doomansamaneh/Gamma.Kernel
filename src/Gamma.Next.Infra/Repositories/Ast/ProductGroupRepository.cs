using Dapper;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Data;

namespace Gamma.Next.Infra.Repositories.Ast;

public sealed class ProductGroupRepository(
    ICurrentUser currentUser,
    IUidGenerator uidGenerator,
    ISystemClock clock)
    : GenericRepository<Domain.Entities.Ast.ProductGroup>(currentUser, uidGenerator, clock)
    , Domain.Interfaces.Ast.IProductGroupRepository;