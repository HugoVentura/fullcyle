using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FC.CodeFlix.Catalog.Domain.Repository;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.Application.UseCases.Genre.CreateGenre
{
    public class CreateGenre : ICreateGenre
    {
        private readonly IGenreRepository _genreRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryRepository _categoryRepository;

        public CreateGenre(IGenreRepository genreRepository, IUnitOfWork unitOfWork, ICategoryRepository categoryRepository)
        {
            this._genreRepository = genreRepository;
            this._unitOfWork = unitOfWork;
            this._categoryRepository = categoryRepository;
        }

        public async Task<GenreModelOutput> Handle(CreateGenreInput request, CancellationToken cancellationToken)
        {
            var genre = new DomainEntity.Genre(request.Name, request.IsActive);
            await this.ValidateCategoriesIds(request, cancellationToken);
            request.CategoriesIds?.ForEach(id => genre.AddCategory(id));

            await this._genreRepository.Insert(genre, cancellationToken);
            await this._unitOfWork.Commit(cancellationToken);

            return GenreModelOutput.FromGenre(genre);
        }

        private async Task ValidateCategoriesIds(CreateGenreInput request, CancellationToken cancellationToken)
        {
            if ((request.CategoriesIds?.Count ?? 0) <= 0)
                return;

            var idsInPersistence = await this._categoryRepository.GetIdsListByIds(request.CategoriesIds!, cancellationToken);
            if (idsInPersistence.Count < request.CategoriesIds!.Count)
            {
                var notFoundIds = request.CategoriesIds.FindAll(p => !idsInPersistence.Contains(p));
                var notFoundIdsAsString = string.Join(", ", notFoundIds);
                throw new RelatedAggregateException($"Related category id (or ids) not found: {notFoundIdsAsString}");
            }
        }
    }
}
