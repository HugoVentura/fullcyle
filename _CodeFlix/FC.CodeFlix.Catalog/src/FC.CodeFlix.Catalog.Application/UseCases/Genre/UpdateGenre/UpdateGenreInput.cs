using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Genre.UpdateGenre
{
    public class UpdateGenreInput : IRequest<GenreModelOutput>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        public List<Guid>? CategoriesIds { get; set; }

        public UpdateGenreInput(Guid id, string name, bool? isActive = null, List<Guid>? categoriesIds = null)
        {
            this.Id = id;
            this.Name = name;
            this.IsActive = isActive;
            this.CategoriesIds = categoriesIds;
        }
    }
}
