using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Domain.Repository;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Genre.DeleteGenre
{
    public class DeleteGenre : IDeleteGenre
    {
        private readonly IGenreRepository _genreRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteGenre(IGenreRepository genreRepository, IUnitOfWork unitOfWork)
        {
            this._genreRepository = genreRepository;
            this._unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteGenreInput input, CancellationToken cancellationToken)
        {
            var genre = await this._genreRepository.Get(input.Id, cancellationToken);
            await this._genreRepository.Delete(genre, cancellationToken);
            await this._unitOfWork.Commit(cancellationToken);

            return Unit.Value;
        }
    }
}
