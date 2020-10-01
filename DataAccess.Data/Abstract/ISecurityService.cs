using System.Threading.Tasks;

namespace DataAccess.Data.Abstract
{
    public interface ISecurityService
    {
        Task<bool> AuthenticateTeam(string username, string secretToken);
    }
}