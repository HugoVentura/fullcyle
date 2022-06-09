using FC.CodeFlix.Catalog.Domain.SeedWork;
using FC.CodeFlix.Catalog.Domain.Validation;

namespace FC.CodeFlix.Catalog.Domain.Entity
{
    public class Genre : AggregateRoot
    {
        private List<Guid> _categories;

        public string Name { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public IReadOnlyList<Guid> Categories { get => this._categories; }

        public Genre(string name, bool isActive = true)
        {
            this.Name = name;
            this.IsActive = isActive;
            this.CreatedAt = DateTime.Now;
            this._categories = new();

            this.Validate();
        }

        private void Validate() => DomainValidation.NotNullOrEmpty(this.Name, nameof(this.Name));

        public void Activate()
        {
            this.IsActive = true;

            this.Validate();
        }

        public void Deactivate()
        {
            this.IsActive = false;

            this.Validate();
        }

        public void Update(string name)
        {
            this.Name = name;

            this.Validate();
        }

        public void AddCategory(Guid categoryId)
        {
            this._categories.Add(categoryId);
            
            this.Validate();
        }

        public void RemoveCategory(Guid categoryId)
        {
            this._categories.Remove(categoryId);

            this.Validate();
        }

        public void RemoveAllCategory()
        {
            this._categories.Clear();

            this.Validate();
        }
    }
}
