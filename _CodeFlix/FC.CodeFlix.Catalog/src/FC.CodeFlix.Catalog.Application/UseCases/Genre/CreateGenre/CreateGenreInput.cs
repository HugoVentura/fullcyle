using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Genre.CreateGenre
{
    public class CreateGenreInput : IRequest<GenreModelOutput>
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public List<Guid>? CategoriesIds { get; set; }

        public CreateGenreInput(string name, bool isActive, List<Guid>? categoriesIds = null)
        {
            this.Name = name;
            this.IsActive = isActive;
            this.CategoriesIds = categoriesIds;
        }
    }
}
