using Microsoft.EntityFrameworkCore;

using DomainCriteria = Payments.Core.Shared.Domain.FiltersByCriteria.Criteria;

namespace Payments.Core.Shared.Infrastructure.Persistence.EntityFramework.Criteria;

public static class SearchByCriteriaExtension
{
    public static IQueryable<T> SearchByCriteria<T>(this DbSet<T> dbSet, DomainCriteria criteria)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(dbSet);
        ArgumentNullException.ThrowIfNull(criteria);

        return dbSet.Where(criteria).OrderBy(criteria).Offset(criteria).Limit(criteria);
    }
}
