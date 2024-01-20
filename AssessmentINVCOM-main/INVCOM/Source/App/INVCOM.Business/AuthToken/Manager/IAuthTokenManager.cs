namespace INVCOM.Business.AuthToken.Manager
{
    using INVCOM.Business.AuthToken.Models;
    using System.Threading.Tasks;
    public interface IAuthTokenManager
    {
        Task<AuthTokenModel> GenerateToken(string userId, string userName);
    }
}
