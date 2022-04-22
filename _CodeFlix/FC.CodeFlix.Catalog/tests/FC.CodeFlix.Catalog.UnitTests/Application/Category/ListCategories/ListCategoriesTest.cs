using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Category.ListCategories;
using FC.CodeFlix.Catalog.Domain.Entity;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using UseCase = FC.CodeFlix.Catalog.Application.UseCases.Category.ListCategories;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Category.ListCategories
{
    [Collection(nameof(ListCategoriesTestFixture))]
    public class ListCategoriesTest
    {
        private readonly ListCategoriesTestFixture _fixture;

        public ListCategoriesTest(ListCategoriesTestFixture fixture) => _fixture = fixture;

        [Fact(DisplayName = nameof(Search))]
        [Trait("Application", "ListCategories - Use Cases")]
        public async Task Search()
        {
            var categoriesExampleList = _fixture.GetExampleCategoriesList();
            var repositoryMock = _fixture.GetRepositoryMock();
            var input = _fixture.GetExampleInput();

            var outputRepositorySearch = new SearchOutput<DomainEntity.Category>(
                currentPage: input.Page,
                perPage: input.PerPage,
                items: categoriesExampleList,
                total: new Random().Next(50, 200));

            repositoryMock.Setup(p => p.Search(
                It.Is<SearchInput>(searchInput =>
                    searchInput.Page.Equals(input.Page) &&
                    searchInput.PerPage.Equals(input.PerPage) &&
                    searchInput.Search.Equals(input.Search) &&
                    searchInput.OrderBy.Equals(input.Sort) &&
                    searchInput.Order.Equals(input.Dir)
                ),
                It.IsAny<CancellationToken>())
            ).ReturnsAsync(outputRepositorySearch);
            var useCase = new UseCase.ListCategories(repositoryMock.Object);

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Page.Should().Be(outputRepositorySearch.CurrentPage);
            output.PerPage.Should().Be(outputRepositorySearch.PerPage);
            output.Total.Should().Be(outputRepositorySearch.Total);
            output.Items.Should().HaveCount(outputRepositorySearch.Items.Count);
            ((List<CategoryModelOutput>)output.Items).ForEach(outputItem =>
            {
                var repositoryCategory = outputRepositorySearch.Items.FirstOrDefault(p => p.Id.Equals(outputItem.Id));
                outputItem.Should().NotBeNull();
                outputItem.Name.Should().Be(repositoryCategory!.Name);
                outputItem.Description.Should().Be(repositoryCategory.Description);
                outputItem.IsActive.Should().Be(repositoryCategory.IsActive);
                outputItem.CreatedAt.Should().Be(repositoryCategory.CreatedAt);
            });

            repositoryMock.Verify(p => p.Search(
                It.Is<SearchInput>(searchInput =>
                    searchInput.Page.Equals(input.Page) &&
                    searchInput.PerPage.Equals(input.PerPage) &&
                    searchInput.Search.Equals(input.Search) &&
                    searchInput.OrderBy.Equals(input.Sort) &&
                    searchInput.Order.Equals(input.Dir)
                ),
                It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact(DisplayName = nameof(Search_WhenIsEmpty))]
        [Trait("Application", "ListCategories - Use Cases")]
        public async Task Search_WhenIsEmpty()
        {
            var repositoryMock = _fixture.GetRepositoryMock();
            var input = _fixture.GetExampleInput();

            var outputRepositorySearch = new SearchOutput<DomainEntity.Category>(
                currentPage: input.Page,
                perPage: input.PerPage,
                items: new List<DomainEntity.Category>().AsReadOnly(),
                total: 0);

            repositoryMock.Setup(p => p.Search(
                It.Is<SearchInput>(searchInput =>
                    searchInput.Page.Equals(input.Page) &&
                    searchInput.PerPage.Equals(input.PerPage) &&
                    searchInput.Search.Equals(input.Search) &&
                    searchInput.OrderBy.Equals(input.Sort) &&
                    searchInput.Order.Equals(input.Dir)
                ),
                It.IsAny<CancellationToken>())
            ).ReturnsAsync(outputRepositorySearch);
            var useCase = new UseCase.ListCategories(repositoryMock.Object);

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Page.Should().Be(outputRepositorySearch.CurrentPage);
            output.PerPage.Should().Be(outputRepositorySearch.PerPage);
            output.Total.Should().Be(0);
            output.Items.Should().BeEmpty();

            repositoryMock.Verify(p => p.Search(
                It.Is<SearchInput>(searchInput =>
                    searchInput.Page.Equals(input.Page) &&
                    searchInput.PerPage.Equals(input.PerPage) &&
                    searchInput.Search.Equals(input.Search) &&
                    searchInput.OrderBy.Equals(input.Sort) &&
                    searchInput.Order.Equals(input.Dir)
                ),
                It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Theory(DisplayName = nameof(Search_WithoutParams))]
        [Trait("Application", "ListCategories - Use Cases")]
        [MemberData(nameof(ListCategoriesTestDataGenerator.GetInputsWithoutParams), parameters: 14, MemberType = typeof(ListCategoriesTestDataGenerator))]
        public async Task Search_WithoutParams(ListCategoriesInput input)
        {
            var categoriesExampleList = _fixture.GetExampleCategoriesList();
            var repositoryMock = _fixture.GetRepositoryMock();

            var outputRepositorySearch = new SearchOutput<DomainEntity.Category>(
                currentPage: input.Page,
                perPage: input.PerPage,
                items: categoriesExampleList,
                total: new Random().Next(50, 200));

            repositoryMock.Setup(p => p.Search(
                It.Is<SearchInput>(searchInput =>
                    searchInput.Page.Equals(input.Page) &&
                    searchInput.PerPage.Equals(input.PerPage) &&
                    searchInput.Search.Equals(input.Search) &&
                    searchInput.OrderBy.Equals(input.Sort) &&
                    searchInput.Order.Equals(input.Dir)
                ),
                It.IsAny<CancellationToken>())
            ).ReturnsAsync(outputRepositorySearch);
            var useCase = new UseCase.ListCategories(repositoryMock.Object);

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Page.Should().Be(outputRepositorySearch.CurrentPage);
            output.PerPage.Should().Be(outputRepositorySearch.PerPage);
            output.Total.Should().Be(outputRepositorySearch.Total);
            output.Items.Should().HaveCount(outputRepositorySearch.Items.Count);
            ((List<CategoryModelOutput>)output.Items).ForEach(outputItem =>
            {
                var repositoryCategory = outputRepositorySearch.Items.FirstOrDefault(p => p.Id.Equals(outputItem.Id));
                outputItem.Should().NotBeNull();
                outputItem.Name.Should().Be(repositoryCategory!.Name);
                outputItem.Description.Should().Be(repositoryCategory.Description);
                outputItem.IsActive.Should().Be(repositoryCategory.IsActive);
                outputItem.CreatedAt.Should().Be(repositoryCategory.CreatedAt);
            });

            repositoryMock.Verify(p => p.Search(
                It.Is<SearchInput>(searchInput =>
                    searchInput.Page.Equals(input.Page) &&
                    searchInput.PerPage.Equals(input.PerPage) &&
                    searchInput.Search.Equals(input.Search) &&
                    searchInput.OrderBy.Equals(input.Sort) &&
                    searchInput.Order.Equals(input.Dir)
                ),
                It.IsAny<CancellationToken>()),
                Times.Once
            );
        }
    }
}
