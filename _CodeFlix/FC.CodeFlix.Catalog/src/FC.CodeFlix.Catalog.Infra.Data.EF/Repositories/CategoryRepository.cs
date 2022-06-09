using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Domain.Entity;
using FC.CodeFlix.Catalog.Domain.Repository;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using Microsoft.EntityFrameworkCore;

namespace FC.CodeFlix.Catalog.Infra.Data.EF.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CodeFlixCatalogDbContext _context;
        private DbSet<Category> _categories => this._context.Set<Category>();

        public CategoryRepository(CodeFlixCatalogDbContext context) => this._context = context;


        public async Task Insert(Category aggregate, CancellationToken cancellationToken) => await this._categories.AddAsync(aggregate, cancellationToken);

        public Task Update(Category aggregate, CancellationToken _) => Task.FromResult(this._categories.Update(aggregate));

        public Task Delete(Category aggregate, CancellationToken _) => Task.FromResult(this._categories.Remove(aggregate));

        public async Task<Category> Get(Guid id, CancellationToken cancellationToken)
        {
            var category = await this._categories.AsNoTracking().FirstOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);
            NotFoundException.ThrowIfNull(category, $"Category '{id}' not found.");

            return category!;
        }

        public async Task<SearchOutput<Category>> Search(SearchInput input, CancellationToken cancellationToken)
        {
            var toSkip = (input.Page - 1) * input.PerPage;
            var query = this._categories.AsNoTracking();
            query = this.AddOrderToQuery(query, input.OrderBy, input.Order);
            if (!string.IsNullOrWhiteSpace(input.Search))
                query = query.Where(p => p.Name.Contains(input.Search));

            var total = await query.CountAsync();
            var items = await query.Skip(toSkip).Take(input.PerPage).ToListAsync();

            return new SearchOutput<Category>(input.Page, input.PerPage, total, items);
        }

        private IQueryable<Category> AddOrderToQuery(IQueryable<Category> query, string orderProperty, SearchOrder order)
        {
            var orderedQuery = (orderProperty.ToLower(), order) switch
            {
                ("name", SearchOrder.Asc) => query.OrderBy(p => p.Name).ThenBy(p => p.Id),
                ("name", SearchOrder.Desc) => query.OrderByDescending(p => p.Name).ThenByDescending(p => p.Id),
                ("id", SearchOrder.Asc) => query.OrderBy(p => p.Id),
                ("id", SearchOrder.Desc) => query.OrderByDescending(p => p.Id),
                ("createdat", SearchOrder.Asc) => query.OrderBy(p => p.CreatedAt),
                ("createdat", SearchOrder.Desc) => query.OrderByDescending(p => p.CreatedAt),
                _ => query.OrderBy(p => p.Name).ThenBy(p => p.Id)
            };

            return orderedQuery;
        }

        public async Task<IReadOnlyList<Guid>> GetIdsListByIds(List<Guid> ids, CancellationToken cancellationToken) =>
            await this._categories.AsNoTracking().Where(p => ids.Contains(p.Id)).Select(p => p.Id).ToListAsync();
    }
}
