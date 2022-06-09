using FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.Infra.Data.EF.Models
{
    public class GenresCategories
    {
        public GenresCategories(Guid genreId, Guid categoryId)
        {
            this.GenreId = genreId;
            this.CategoryId = categoryId;
        }

        public Guid GenreId { get; set; }
        public Guid CategoryId { get; set; }
        public Genre? Genre { get; set; }
        public Category? Category { get; set; }
    }
}
