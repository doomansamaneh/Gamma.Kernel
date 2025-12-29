namespace Gamma.Kernel.Abstractions;

public interface IHasEntity<TEntity> : ICommand
{
    TEntity Entity { get; }
}

public interface IHasKey<TKey> : ICommand
{
    TKey Id { get; }
}