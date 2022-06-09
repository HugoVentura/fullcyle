using FC.CodeFlix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using UseCase = FC.CodeFlix.Catalog.Application.UseCases.Genre.ListGenres;

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Genre.ListGenres
{
    [Collection(nameof(ListGenresTestFixture))]
    public class ListGenresTest
    {
        private readonly ListGenresTestFixture _fixture;

        public ListGenresTest(ListGenresTestFixture fixture) => this._fixture = fixture;

        [Fact(DisplayName = nameof(ListGenres))]
        [Trait("Integration/Application", "ListGenres - Use Cases")]
        public async Task ListGenres()
        {
            var exampleGenres = this._fixture.GetExampleListGenres(10);
            var arrangeDbContext = this._fixture.CreateDbContext();
            await arrangeDbContext.AddRangeAsync(exampleGenres);
            await arrangeDbContext.SaveChangesAsync();
            var useCase = new UseCase.ListGenres(new GenreRepository(this._fixture.CreateDbContext(true)));
            var input = new UseCase.ListGenresInput(1, 20);

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Total.Should().Be(exampleGenres.Count);
            output.Items.Should().HaveCount(exampleGenres.Count);
            output.Items.ToList().ForEach(outputItem =>
            {
                var assertItem = exampleGenres.Find(p => p.Id.Equals(outputItem.Id));
                assertItem.Should().NotBeNull();
                outputItem.Name.Should().Be(assertItem!.Name);
                outputItem.IsActive.Should().Be(assertItem.IsActive);
            });
        }

        [Fact(DisplayName = nameof(ListGenresReturnsEmptWhenPerisstenceIsEmpty))]
        [Trait("Integration/Application", "ListGenres - Use Cases")]
        public async Task ListGenresReturnsEmptWhenPerisstenceIsEmpty()
        {
            var useCase = new UseCase.ListGenres(new GenreRepository(this._fixture.CreateDbContext()));
            var input = new UseCase.ListGenresInput(1, 20);

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Total.Should().Be(0);
            output.Items.Should().BeEmpty();
        }
    }
}
