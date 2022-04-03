namespace FC.CodeFlix.Catalog.Domain.SeedWork
{
    public interface IGenericRepository<TAggregate> : IRepository
    {
        Task Insert(TAggregate aggregate, CancellationToken cancellationToken);
        Task<TAggregate> Get(Guid id, CancellationToken cancellationToken);
    }
}
