using FC.CodeFlix.Catalog.Application.UseCases.Category.UpdateCategory;
using FluentAssertions;
using System;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.UpdateCategory
{
    [Collection(nameof(UpdateCategoryTestFixture))]
    public class UpdateCategoryInputValidatorTest
    {
        private readonly UpdateCategoryTestFixture _fixture;

        public UpdateCategoryInputValidatorTest(UpdateCategoryTestFixture fixture)
        {
            this._fixture = fixture;
        }

        [Fact(DisplayName = nameof(Test_Validate_ValidateWhenEmptyGuid))]
        [Trait("Application", "UpdateCategoryInputValidator - Use Cases")]
        public void Test_Validate_ValidateWhenEmptyGuid()
        {
            var input = this._fixture.GetValidInput(Guid.Empty);
            var validator = new UpdateCategoryInputValidator();

            var validateResult = validator.Validate(input);

            validateResult.Should().NotBeNull();
            validateResult.IsValid.Should().BeFalse();
            validateResult.Errors.Should().NotBeEmpty();
            validateResult.Errors.Should().Contain(p => p.ErrorMessage.Equals("'Id' must not be empty."));
        }

        [Fact(DisplayName = nameof(Test_Validate_ValidateWhenGuidIsValid))]
        [Trait("Application", "UpdateCategoryInputValidator - Use Cases")]
        public void Test_Validate_ValidateWhenGuidIsValid()
        {
            var input = this._fixture.GetValidInput();
            var validator = new UpdateCategoryInputValidator();

            var validateResult = validator.Validate(input);

            validateResult.Should().NotBeNull();
            validateResult.IsValid.Should().BeTrue();
            validateResult.Errors.Should().BeEmpty();
        }
    }
}
