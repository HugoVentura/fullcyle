using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Domain.Repository;
using FC.CodeFlix.Catalog.UnitTests.Common;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Genre.Common
{
    public class GenreUseCasesBaseFixture : BaseFixture
    {
        public string GetValidGenreName() => this.Faker.Commerce.Categories(1)[0];

        public DomainEntity.Genre GetExampleGenre(bool? isActive = null, List<Guid>? categoriesIds = null)
        {
            var genre = new DomainEntity.Genre(this.GetValidGenreName(), isActive ?? this.GetRandomBoolean());
            categoriesIds?.ForEach(genre.AddCategory);

            return genre;
        }

        public List<DomainEntity.Genre> GetExampleGenresList(int count = 10) =>
            Enumerable.Range(1, count).Select(_ =>
            {
                var genre = new DomainEntity.Genre(this.GetValidGenreName(), this.GetRandomBoolean());
                this.GetRandomIdsList().ForEach(genre.AddCategory);

                return genre;
            }).ToList();

        public Mock<IGenreRepository> GetGenreRepositoryMock() => new();

        public Mock<IUnitOfWork> GetUnitOfWorkMock() => new();

        public Mock<ICategoryRepository> GetCategoryRepositoryMock() => new();

        public List<Guid> GetRandomIdsList(int? count = null) =>
            Enumerable.Range(1, count ?? (new Random().Next(1, 10)))
                .Select(_ => Guid.NewGuid()).ToList();
    }
}
