using FC.CodeFlix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.Common
{
    public class CategoryPersistence
    {
        private readonly CodeFlixCatalogDbContext _dbContext;

        public CategoryPersistence(CodeFlixCatalogDbContext dbContext) => this._dbContext = dbContext;

        public async Task<DomainEntity.Category?> GetById(Guid id) => await this._dbContext.Categories.AsNoTracking().FirstOrDefaultAsync(p => p.Id.Equals(id));

        public async Task InsertList(List<DomainEntity.Category> categories)
        {
            await this._dbContext.Categories.AddRangeAsync(categories);
            await this._dbContext.SaveChangesAsync();
        }
    }
}
