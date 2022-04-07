using FC.CodeFlix.Catalog.Application.UseCases.Category.GetCategory;
using FluentAssertions;
using System;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.GetCategory
{
    [Collection(nameof(GetCategoryTestFixture))]
    public class GetCategoryInputValidatorTest
    {
        private readonly GetCategoryTestFixture _fixture;

        public GetCategoryInputValidatorTest(GetCategoryTestFixture fixture) => this._fixture = fixture;

        [Fact(DisplayName = nameof(Test_Validation_Ok))]
        [Trait("Application", "GetCategoryInputValidation - UseCases")]
        public void Test_Validation_Ok()
        {
            var validInput = new GetCategoryInput(Guid.NewGuid());
            var validator = new GetCategoryInputValidator();
            
            var validationResult = validator.Validate(validInput);

            validationResult.Should().NotBeNull();
            validationResult.IsValid.Should().BeTrue();
            validationResult.Errors.Should().BeEmpty();
        }

        [Fact(DisplayName = nameof(Test_Validation_InvalidWhenEmptyGuidId))]
        [Trait("Application", "GetCategoryInputValidation - UseCases")]
        public void Test_Validation_InvalidWhenEmptyGuidId()
        {
            var invalidInput = new GetCategoryInput(Guid.Empty);
            var validator = new GetCategoryInputValidator();

            var validationResult = validator.Validate(invalidInput);

            validationResult.Should().NotBeNull();
            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Should().NotBeEmpty();
            validationResult.Errors.Should().Contain(p => p.ErrorMessage.Equals("'Id' must not be empty."));
        }
    }
}
