using FC.CodeFlix.Catalog.Domain.Exceptions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.UnitTests.Domain.Entity.Category
{
    [Collection(nameof(CategoryTestFixture))]
    public class CategoryTest
    {
        private readonly CategoryTestFixture _categoryTestFixture;

        public CategoryTest(CategoryTestFixture categoryTestFixture) => this._categoryTestFixture = categoryTestFixture;

        [Fact(DisplayName = nameof(Test_Instantiate))]
        [Trait("Domain", "Category - Aggregates")]
        public void Test_Instantiate()
        {
            var validCategory = this._categoryTestFixture.GetValidCategory();
            var dateTimeBefore = DateTime.Now;
            var category = new DomainEntity.Category(validCategory.Name, validCategory.Description);
            var dateTimeAfter = DateTime.Now;

            category.Should().NotBeNull();
            category.Name.Should().Be(validCategory.Name);
            category.Description.Should().Be(validCategory.Description);
            category.Id.Should().NotBeEmpty();
            category.CreatedAt.Should().NotBeSameDateAs(default);
            (category.CreatedAt > dateTimeBefore).Should().BeTrue();
            (category.CreatedAt < dateTimeAfter).Should().BeTrue();
            category.IsActive.Should().BeTrue();
        }

        [Theory(DisplayName = nameof(Test_InstantiateWithIsActive))]
        [Trait("Domain", "Category - Aggregates")]
        [InlineData(true)]
        [InlineData(false)]
        public void Test_InstantiateWithIsActive(bool isActive)
        {
            var validCategory = this._categoryTestFixture.GetValidCategory();
            var dateTimeBefore = DateTime.Now;
            var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, isActive);
            var dateTimeAfter = DateTime.Now.AddSeconds(1);

            category.Should().NotBeNull();
            category.Name.Should().Be(validCategory.Name);
            category.Description.Should().Be(validCategory.Description);
            category.Id.Should().NotBeEmpty();
            category.CreatedAt.Should().NotBeSameDateAs(default);
            (category.CreatedAt >= dateTimeBefore).Should().BeTrue();
            (category.CreatedAt <= dateTimeAfter).Should().BeTrue();
            category.IsActive.Should().Be(isActive);
        }

        [Theory(DisplayName = nameof(Test_InstantiateErrorWhenNameIsEmpty))]
        [Trait("Domain", "Category - Aggregates")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void Test_InstantiateErrorWhenNameIsEmpty(string? name)
        {
            var validCategory = this._categoryTestFixture.GetValidCategory();
            Action action = () => new DomainEntity.Category(name!, validCategory.Description);

            action.Should()
                .Throw<EntityValidationException>()
                .WithMessage("Name should not be empty or null");
        }

        [Fact(DisplayName = nameof(Test_InstantiateErrorWhenDescriptionIsNull))]
        [Trait("Domain", "Category - Aggregates")]
        public void Test_InstantiateErrorWhenDescriptionIsNull()
        {
            var validCategory = this._categoryTestFixture.GetValidCategory();
            Action action = () => new DomainEntity.Category(validCategory.Name, null!);
            var exception = Assert.Throws<EntityValidationException>(action);

            action.Should()
                .Throw<EntityValidationException>()
                .WithMessage("Description should not be null");
        }

        [Theory(DisplayName = nameof(Test_InstantiateErrorWhenNameIsLessThan3Characters))]
        [Trait("Domain", "Category - Aggregates")]
        [InlineData("1")]
        [InlineData("12")]
        [InlineData("a")]
        [InlineData("ca")]
        public void Test_InstantiateErrorWhenNameIsLessThan3Characters(string invalidName)
        {
            var validCategory = this._categoryTestFixture.GetValidCategory();
            Action action = () => new DomainEntity.Category(invalidName, validCategory.Description);

            action.Should()
                .Throw<EntityValidationException>()
                .WithMessage("Name should be at least 3 characters long");
        }

        [Fact(DisplayName = nameof(Test_InstantiateErrorWhenNameIsgreaterThan255Characters))]
        [Trait("Domain", "Category - Aggregates")]
        public void Test_InstantiateErrorWhenNameIsgreaterThan255Characters()
        {
            var validCategory = this._categoryTestFixture.GetValidCategory();
            var invalidName = string.Join(null, Enumerable.Range(1, 256).Select(_ => "a").ToArray());
            Action action = () => new DomainEntity.Category(invalidName, validCategory.Description);

            action.Should()
                .Throw<EntityValidationException>()
                .WithMessage("Name should be less or equal 255 characters long");
        }

        [Fact(DisplayName = nameof(Test_InstantiateErrorWhenDescriptionIsgreaterThan10_000Characters))]
        [Trait("Domain", "Category - Aggregates")]
        public void Test_InstantiateErrorWhenDescriptionIsgreaterThan10_000Characters()
        {
            var validCategory = this._categoryTestFixture.GetValidCategory();
            var invalidDescription = string.Join(null, Enumerable.Range(1, 10_001).Select(_ => "a").ToArray());
            Action action = () => new DomainEntity.Category(validCategory.Name, invalidDescription);

            action.Should()
                .Throw<EntityValidationException>()
                .WithMessage("Description should be less or equal 10000 characters long");
        }

        [Fact(DisplayName = nameof(Test_Activate))]
        [Trait("Domain", "Category - Aggregates")]
        public void Test_Activate()
        {
            var validCategory = this._categoryTestFixture.GetValidCategory();
            var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, false);
            category.Activate();

            category.IsActive.Should().BeTrue();
        }

        [Fact(DisplayName = nameof(Test_Deactivate))]
        [Trait("Domain", "Category - Aggregates")]
        public void Test_Deactivate()
        {
            var validCategory = this._categoryTestFixture.GetValidCategory();
            var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, true);
            category.Deactivate();

            category.IsActive.Should().BeFalse();
        }

        [Fact(DisplayName = nameof(Test_Update))]
        [Trait("Domain", "Category - Aggregates")]
        public void Test_Update()
        {
            var category = this._categoryTestFixture.GetValidCategory();
            var categoryWithNewValues = this._categoryTestFixture.GetValidCategory();
            category.Update(categoryWithNewValues.Name, categoryWithNewValues.Description);

            category.Name.Should().Be(categoryWithNewValues.Name);
            category.Description.Should().Be(categoryWithNewValues.Description);
        }

        [Fact(DisplayName = nameof(Test_Update))]
        [Trait("Domain", "Category - Aggregates")]
        public void Test_UpdateOnlyName()
        {
            var category = this._categoryTestFixture.GetValidCategory();
            var newName = this._categoryTestFixture.GetValidCategoryName();
            var currentDescription = category.Description;
            category.Update(newName);

            category.Name.Should().Be(newName);
            category.Description.Should().Be(currentDescription);
        }

        [Theory(DisplayName = nameof(Test_UpdateErrorWhenNameIsEmpty))]
        [Trait("Domain", "Category - Aggregates")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void Test_UpdateErrorWhenNameIsEmpty(string? name)
        {
            var category = this._categoryTestFixture.GetValidCategory();
            Action action = () => category.Update(name!);

            action.Should()
                .Throw<EntityValidationException>()
                .WithMessage("Name should not be empty or null");
        }

        [Theory(DisplayName = nameof(Test_UpdateErrorWhenNameIsLessThan3Characters))]
        [Trait("Domain", "Category - Aggregates")]
        [MemberData(nameof(GetNamesWithLessThan3Characters), parameters: 10)]
        public void Test_UpdateErrorWhenNameIsLessThan3Characters(string invalidName)
        {
            var category = this._categoryTestFixture.GetValidCategory();
            Action action = () => category.Update(invalidName!);

            action.Should()
                .Throw<EntityValidationException>()
                .WithMessage("Name should be at least 3 characters long");
        }

        public static IEnumerable<object[]> GetNamesWithLessThan3Characters(int numberOfTests = 6)
        {
            var fixture = new CategoryTestFixture();
            for (int idx = 0; idx < numberOfTests; idx++)
            {
                var isOdd = idx % 2 == 1;
                yield return new object[] { fixture.GetValidCategoryName()[..(isOdd ? 1 : 2)] };
            }
        }

        [Fact(DisplayName = nameof(Test_UpdateErrorWhenNameIsGreaterThan255Characters))]
        [Trait("Domain", "Category - Aggregates")]
        public void Test_UpdateErrorWhenNameIsGreaterThan255Characters()
        {
            var category = this._categoryTestFixture.GetValidCategory();
            var invalidName = this._categoryTestFixture.Faker.Lorem.Letter(256);
            Action action = () => category.Update(invalidName!);

            action.Should()
                .Throw<EntityValidationException>()
                .WithMessage("Name should be less or equal 255 characters long");
        }

        [Fact(DisplayName = nameof(Test_UpdateErrorWhenDescriptionIsgreaterThan10_000Characters))]
        [Trait("Domain", "Category - Aggregates")]
        public void Test_UpdateErrorWhenDescriptionIsgreaterThan10_000Characters()
        {
            var category = this._categoryTestFixture.GetValidCategory();
            var invalidDescription = this._categoryTestFixture.Faker.Commerce.ProductDescription();
            while (invalidDescription.Length <= 10_000)
                invalidDescription = $"{invalidDescription} {this._categoryTestFixture.Faker.Commerce.ProductDescription()}";

            Action action = () => category.Update("Category New Name", invalidDescription);

            action.Should()
                .Throw<EntityValidationException>()
                .WithMessage("Description should be less or equal 10000 characters long");
        }
    }
}
