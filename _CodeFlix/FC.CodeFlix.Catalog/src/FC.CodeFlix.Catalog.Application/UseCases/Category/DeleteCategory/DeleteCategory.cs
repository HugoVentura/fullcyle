using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Domain.Repository;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Category.DeleteCategory
{
    public class DeleteCategory : IDeleteCategory
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCategory(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork) 
            => (this._categoryRepository, this._unitOfWork) = (categoryRepository, unitOfWork);

        public async Task<Unit> Handle(DeleteCategoryInput request, CancellationToken cancellationToken)
        {
            var category = await this._categoryRepository.Get(request.Id, cancellationToken);
            await this._categoryRepository.Delete(category, cancellationToken);
            await this._unitOfWork.Commit(cancellationToken);

            return Unit.Value;
        }
    }
}
