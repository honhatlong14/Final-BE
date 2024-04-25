using Data.Entities.BaseEntity;
using Data.Entities.User;
using Data.IRepository;

namespace Data.Repository;

public static class UserRepository
{
    public static async Task UpdateRefreshToken(this IRepository<User> repository, string id, List<RefreshToken> refreshTokens,  string updateBy)
    {
        var user = await repository.GetFirstOrDefaultAsync(_ => _.Id == id && !_.IsDeleted);
        user.UpdateAt = DateTime.Now;
        user.UpdateBy = updateBy;
        user.RefreshTokens = refreshTokens;
        repository.Update(user);
        await repository.SaveChangesAsync();
    }
}