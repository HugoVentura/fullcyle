using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FC.CodeFlix.Catalog.Domain.Repository;

namespace FC.CodeFlix.Catalog.Application.UseCases.Genre.UpdateGenre
{
    public class UpdateGenre : IUpdateGenre
    {
        private readonly IGenreRepository _genreRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryRepository _categoryRepository;

        public UpdateGenre(IGenreRepository genreRepository, IUnitOfWork unitOfWork, ICategoryRepository categoryRepository)
        {
            this._genreRepository = genreRepository;
            this._unitOfWork = unitOfWork;
            this._categoryRepository = categoryRepository;
        }

        public async Task<GenreModelOutput> Handle(UpdateGenreInput request, CancellationToken cancellationToken)
        {
            var genre = await this._genreRepository.Get(request.Id, cancellationToken);
            genre.Update(request.Name);
            if (request.IsActive is not null && request.IsActive != genre.IsActive)
            {
                if (request.IsActive.Value)
                    genre.Activate();
                else
                    genre.Deactivate();
            }
            await this.ValidateCategoriesIds(request, cancellationToken);
            if (request.CategoriesIds is not null)
            {
                genre.RemoveAllCategory();
                request.CategoriesIds.ForEach(genre.AddCategory);
            }

            await this._genreRepository.Update(genre, cancellationToken);
            await this._unitOfWork.Commit(cancellationToken);
            return GenreModelOutput.FromGenre(genre);
        }

        private async Task ValidateCategoriesIds(UpdateGenreInput request, CancellationToken cancellationToken)
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
