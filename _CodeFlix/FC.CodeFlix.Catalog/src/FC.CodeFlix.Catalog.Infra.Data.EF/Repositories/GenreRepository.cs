using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Domain.Entity;
using FC.CodeFlix.Catalog.Domain.Repository;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.CodeFlix.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace FC.CodeFlix.Catalog.Infra.Data.EF.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly CodeFlixCatalogDbContext _context;
        private DbSet<Genre> _genres => this._context.Set<Genre>();
        private DbSet<GenresCategories> _genresCategories => this._context.Set<GenresCategories>();

        public GenreRepository(CodeFlixCatalogDbContext context) => this._context = context;

        public async Task Insert(Genre genre, CancellationToken cancellationToken)
        {
            await this._genres.AddAsync(genre);
            if (genre.Categories.Count > 0)
            {
                var relations = genre.Categories.Select(categoryId => new GenresCategories(genre.Id, categoryId));
                await this._genresCategories.AddRangeAsync(relations);
            }
        }

        public async Task Update(Genre genre, CancellationToken cancellationToken)
        {
            this._genres.Update(genre);
            this._genresCategories.RemoveRange(this._genresCategories.Where(p => p.GenreId.Equals(genre.Id)));
            if (genre.Categories.Count > 0)
            {
                var relations = genre.Categories.Select(categoryId => new GenresCategories(genre.Id, categoryId));
                await this._genresCategories.AddRangeAsync(relations);
            }
        }

        public Task Delete(Genre aggregate, CancellationToken cancellationToken)
        {
            this._genresCategories.RemoveRange(this._genresCategories.Where(p => p.GenreId.Equals(aggregate.Id)));
            this._genres.Remove(aggregate);

            return Task.CompletedTask;
        }

        public async Task<Genre> Get(Guid id, CancellationToken cancellationToken)
        {
            var genre = await this._genres.AsNoTracking().FirstOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);
            NotFoundException.ThrowIfNull(genre, $"Genre '{id}' not found.");
            var categoryIds = await this._genresCategories
                .Where(p => p.GenreId.Equals(genre!.Id))
                .Select(p => p.CategoryId)
                .ToListAsync(cancellationToken);
            categoryIds.ForEach(genre!.AddCategory);

            return genre;
        }

        public async Task<SearchOutput<Genre>> Search(SearchInput input, CancellationToken cancellationToken)
        {
            var toSkip = (input.Page - 1) * input.PerPage;
            var query = this._genres.AsNoTracking();
            query = this.AddOrderToQuery(query, input.OrderBy, input.Order);
            if (!string.IsNullOrWhiteSpace(input.Search))
                query = query.Where(genre => genre.Name.Contains(input.Search));
            var total = await query.CountAsync();
            var genres = await query.Skip(toSkip).Take(input.PerPage).ToListAsync();
            var genresIds = genres.Select(p => p.Id).ToList();
            var relations = await this._genresCategories.Where(relation => genresIds.Contains(relation.GenreId)).ToListAsync();
            var relationsByGenreIdGroup = relations.GroupBy(p => p.GenreId).ToList();
            relationsByGenreIdGroup.ForEach(relationGroup =>
            {
                var genre = genres.Find(p => p.Id.Equals(relationGroup.Key));
                if (genre is null)
                    return;
                relationGroup.ToList().ForEach(relation => genre.AddCategory(relation.CategoryId));
            });

            return new SearchOutput<Genre>(input.Page, input.PerPage, total, genres);
        }

        private IQueryable<Genre> AddOrderToQuery(IQueryable<Genre> query, string orderProperty, SearchOrder order)
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
    }
}
