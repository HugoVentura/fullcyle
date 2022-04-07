using FluentValidation;

namespace FC.CodeFlix.Catalog.Application.UseCases.Category.GetCategory
{
    public class GetCategoryInputValidator : AbstractValidator<GetCategoryInput>
    {
        public GetCategoryInputValidator() => RuleFor(p => p.Id).NotEmpty();
    }
}
