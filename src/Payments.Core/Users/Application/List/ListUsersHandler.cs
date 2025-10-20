using Payments.Core.Shared.Domain;
using Payments.Core.Shared.Domain.FiltersByCriteria;
using Payments.Core.Shared.Infrastructure;
using Payments.Core.Users.Domain;

namespace Payments.Core.Users.Application.List;

public sealed class ListUsersHandler(IUserRepository userRepository) : IHandler
{
    public async Task<ListUsersResponse> Find(List<Dictionary<string, string>> filters, int? limit = null, int? offset = null)
    {
        Filters? criteriaFilters = Filters.FromValues(filters.Only("email", "fullname"));

        Criteria criteria = new(
            criteriaFilters,
            Order.FromValues("CreatedAt", nameof(OrderType.DESC)),
            limit,
            offset
        );

        IEnumerable<User> users = await userRepository.Matching(criteria);

        List<UserSummaryResponse> userResponses = [.. users.Select(user => new UserSummaryResponse(
            user.Id,
            user.Email,
            user.FullName,
            user.CreatedAt.ToApplicationString()
        ))];

        return new ListUsersResponse(userResponses, userResponses.Count);
    }
}
