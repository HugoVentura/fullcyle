using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Genre.GetGenre
{
    public class GetGenreInput : IRequest<GenreModelOutput>
    {
        public Guid Id { get; set; }

        public GetGenreInput(Guid id) => this.Id = id;
    }
}
