using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Genre.DeleteGenre
{
    public class DeleteGenreInput : IRequest
    {
        public Guid Id { get; set; }

        public DeleteGenreInput(Guid id) => this.Id = id;
    }
}
