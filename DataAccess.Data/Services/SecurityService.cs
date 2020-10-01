using DataAccess.Data.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Data.Services
{
    public class SecurityService : ISecurityService
    {
        private readonly ILogger<SecurityService> _logger;

        private readonly DataContext _context;

        public SecurityService(DataContext context, ILogger<SecurityService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException("ILogger");
            _context = context ?? throw new ArgumentNullException("DataContext");
        }

        public async Task<bool> AuthenticateTeam(string username, string secretToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(secretToken))
                    return false;
                username = username.ToLowerInvariant();
                secretToken = secretToken.ToLowerInvariant();

                var auth = await _context.Teams.Where(team => team.TeamId == username && team.SecretToken == secretToken).SingleOrDefaultAsync();

                return auth != null;
            }
            catch(Exception ex)
            {
                _logger.LogError($"Authentication Failed, {ex}");
                return false;
            }
        }
    }
}