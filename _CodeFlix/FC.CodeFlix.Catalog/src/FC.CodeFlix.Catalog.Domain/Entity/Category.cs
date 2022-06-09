using FC.CodeFlix.Catalog.Domain.SeedWork;
using FC.CodeFlix.Catalog.Domain.Validation;

namespace FC.CodeFlix.Catalog.Domain.Entity
{
    public class Category : AggregateRoot
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public Category(string name, string description, bool isActive = true) : base()
        {
            Name = name;
            Description = description;
            IsActive = isActive;
            CreatedAt = DateTime.Now;
            this.Validate();
        }

        private void Validate()
        {
            DomainValidation.NotNullOrEmpty(this.Name, nameof(this.Name));
            DomainValidation.MinLength(this.Name, 3, nameof(this.Name));
            DomainValidation.MaxLength(this.Name, 255, nameof(this.Name));

            DomainValidation.NotNull(this.Description, nameof(this.Description));
            DomainValidation.MaxLength(this.Description, 10_000, nameof(this.Description));
        }

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

        public void Update(string name, string? description = null)
        {
            Name = name;
            Description = description ?? Description;

            this.Validate();
        }
    }
}
