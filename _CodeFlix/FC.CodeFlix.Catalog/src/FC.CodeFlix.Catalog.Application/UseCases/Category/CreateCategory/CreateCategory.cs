using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FC.CodeFlix.Catalog.Domain.Repository;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.Application.UseCases.Category.CreateCategory
{
    public class CreateCategory : ICreateCategory
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryRepository _categoryRepository;

        public CreateCategory(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
            this._categoryRepository = categoryRepository;
        }

        public async Task<CategoryModelOutput> Handle(CreateCategoryInput input, CancellationToken cancellationToken)
        {
            var category = new DomainEntity.Category(input.Name, input.Description, input.IsActive);
            await this._categoryRepository.Insert(category, cancellationToken);
            await this._unitOfWork.Commit(cancellationToken);

            return CategoryModelOutput.FromCategory(category);
        }
    }
}
