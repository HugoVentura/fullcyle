using FC.CodeFlix.Catalog.Domain.Repository;

namespace FC.CodeFlix.Catalog.Application.UseCases.Genre.ListGenres
{
    public class ListGenres : IListGenres
    {
        private readonly IGenreRepository _genreRepository;

        public ListGenres(IGenreRepository genreRepository) => this._genreRepository = genreRepository;

        public async Task<ListGenresOutput> Handle(ListGenresInput input, CancellationToken cancellationToken)
        {
            var searchOutput = await this._genreRepository.Search(input.ToSearchInput(), cancellationToken);

            return ListGenresOutput.FromSearchOutput(searchOutput);
        }
    }
}
