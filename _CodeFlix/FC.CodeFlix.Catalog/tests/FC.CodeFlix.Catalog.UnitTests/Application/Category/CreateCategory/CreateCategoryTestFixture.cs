using FC.CodeFlix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.CodeFlix.Catalog.UnitTests.Application.Category.Common;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Category.CreateCategory
{
    public class CreateCategoryTestFixture : CategoryUseCasesBaseFixture
    {
        public CreateCategoryTestFixture() : base() { }

        public CreateCategoryInput GetInput() => new(GetValidCategoryName(), GetValidCategoryDescription(), GetRandomBoolean());

        public CreateCategoryInput GetInvalidInputShortName()
        {
            var invalidInputShortName = GetInput();
            invalidInputShortName.Name = invalidInputShortName.Name[..2];
            return invalidInputShortName;
        }

        public CreateCategoryInput GetInvalidInputTooLongName()
        {
            var invalidInputTooLongName = GetInput();
            var tooLongNameForCategory = Faker.Commerce.ProductName();
            while (tooLongNameForCategory.Length <= 255)
                tooLongNameForCategory = $"{tooLongNameForCategory} {Faker.Commerce.ProductName()}";
            invalidInputTooLongName.Name = tooLongNameForCategory;

            return invalidInputTooLongName;
        }

        public CreateCategoryInput GetInvalidInputCategoryNull()
        {
            var invalidInputDescriptionNull = GetInput();
            invalidInputDescriptionNull.Description = null!;

            return invalidInputDescriptionNull;
        }

        public CreateCategoryInput GetInvalidInputTooLongDescription()
        {
            var invalidInputTooLongDescription = GetInput();
            var tooLongDescriptionForCategory = Faker.Commerce.ProductDescription();
            while (tooLongDescriptionForCategory.Length <= 10_000)
                tooLongDescriptionForCategory = $"{tooLongDescriptionForCategory} {Faker.Commerce.ProductDescription()}";
            invalidInputTooLongDescription.Description = tooLongDescriptionForCategory;

            return invalidInputTooLongDescription;
        }
    }

    [CollectionDefinition(nameof(CreateCategoryTestFixture))]
    public class CreateCategoryTestFixtureCollection : ICollectionFixture<CreateCategoryTestFixture> { }
}
