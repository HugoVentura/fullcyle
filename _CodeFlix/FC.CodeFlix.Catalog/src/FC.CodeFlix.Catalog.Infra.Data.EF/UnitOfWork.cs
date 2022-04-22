using FC.CodeFlix.Catalog.Application.Interfaces;

namespace FC.CodeFlix.Catalog.Infra.Data.EF
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CodeFlixCatalogDbContext _context;

        public UnitOfWork(CodeFlixCatalogDbContext context) => this._context = context;

        public Task Commit(CancellationToken cancellationToken) => this._context.SaveChangesAsync(cancellationToken);
        public Task Rollback(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
