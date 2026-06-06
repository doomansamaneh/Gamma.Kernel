using Dapper;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Data;

namespace {{InfraNamespace}}.Repositories.{{Schema}};

public sealed class {{Entity}}Repository(
    ICurrentUser currentUser,
    IUidGenerator uidGenerator,
    ISystemClock clock)
    : GenericRepository<Domain.Entities.{{Schema}}.{{Entity}}>(currentUser, uidGenerator, clock)
    , Domain.Interfaces.{{Schema}}.I{{Entity}}Repository;