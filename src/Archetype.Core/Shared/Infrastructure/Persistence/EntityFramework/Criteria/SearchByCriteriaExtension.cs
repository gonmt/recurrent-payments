using Microsoft.EntityFrameworkCore;

using DomainCriteria = Archetype.Core.Shared.Domain.FiltersByCriteria.Criteria;

namespace Archetype.Core.Shared.Infrastructure.Persistence.EntityFramework.Criteria;

public static class SearchByCriteriaExtension
{
    public static IQueryable<T> SearchByCriteria<T>(this DbSet<T> dbSet, DomainCriteria criteria)
        where T : class
    {

        return dbSet.Where(criteria).OrderBy(criteria).Offset(criteria).Limit(criteria);
    }
}
