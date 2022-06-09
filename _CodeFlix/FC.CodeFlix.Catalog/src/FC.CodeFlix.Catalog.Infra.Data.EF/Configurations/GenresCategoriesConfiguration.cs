﻿using FC.CodeFlix.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FC.CodeFlix.Catalog.Infra.Data.EF.Configurations
{
    internal class GenresCategoriesConfiguration : IEntityTypeConfiguration<GenresCategories>
    {
        public void Configure(EntityTypeBuilder<GenresCategories> builder) => builder.HasKey(p => new { p.GenreId, p.CategoryId });
    }
}
