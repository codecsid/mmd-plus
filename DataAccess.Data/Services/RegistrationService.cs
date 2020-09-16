using DataAccess.Data.Abstract;
using DataAccess.Data.Repositories;
using DataAccess.Model;
using DataAccess.Model.SharedModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using NotificationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Data.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly DataContext _context;
        private readonly IEntityBaseRepository<Team> _teamBaseRepo;
        private readonly IEntityBaseRepository<User> _userBaseRepo;
        private readonly ILogger<RegistrationService> _logger;
        private readonly ITeamRepository _teamRepo;
        private readonly IUserRepository _userRepo;
        private readonly INotificationService _emailNotificationService;

        public RegistrationService(DataContext context, IEntityBaseRepository<Team> teamBaseRepo, IEntityBaseRepository<User> userBaseRepo, ILogger<RegistrationService> logger, ITeamRepository teamRepo, IUserRepository userRepo, Func<string, INotificationService> notificationServiceAccessor)
        {
            _context = context;
            _teamBaseRepo = teamBaseRepo;
            _userBaseRepo = userBaseRepo;
            _logger = logger;
            _teamRepo = teamRepo;
            _userRepo = userRepo;
            _emailNotificationService = notificationServiceAccessor("Email");
        }

        public List<RegisteredTeam> GetRegisteredTeams(Dictionary<string, string> searchValues, string sortColumn, string sortDir, int start, int length, out int filteredCount, out int totalCount)
        {
            IQueryable<Team> query = null;

            totalCount = _teamBaseRepo.Count();

            query = _context.Teams.Include(team => team.LocationNav);

            query = GetQuery(query, sortColumn, sortDir);

            foreach(var x in searchValues)
            {
                query = AddSearchParameter(query, x.Key, x.Value.ToLowerInvariant());
            }

            filteredCount = query.Count();

            query = query.Skip(start).Take(length);

            var result = query.ToList();

            var listOfRegisteredTeams = new List<RegisteredTeam>();

            foreach (var team in result)
                listOfRegisteredTeams.Add(GetTeamDisplayModel(team));

            return listOfRegisteredTeams;
        }

        private RegisteredTeam GetTeamDisplayModel(Team item)
        {
            return new RegisteredTeam
            {
                TeamId = item.TeamId,
                Location = item.LocationNav.DisplayName,
                RegistrationTime = item.RegisteredAt
            };
        }

        private IQueryable<Team> AddSearchParameter(IQueryable<Team> query, string key, string value)
        {
            IQueryable<Team> _query = query;
            switch(key)
            {
                case "id": return query.Where(t => t.Id.ToString().Contains(value));
                case "teamId": return query.Where(t => t.TeamId.Contains(value));
                case "location": return query.Where(t => t.Location.Contains(value));
                case "registrationTime": return query.Where(t => t.RegisteredAt.ToString().Contains(value));
            }
            return _query;
        }

        private IQueryable<Team> GetQuery(IQueryable<Team> query, string sortColumn, string sortDir)
        {
            if(sortDir == "asc")
            {
                switch (sortColumn)
                {
                    case "id": return query.OrderBy(t => t.Id);
                    case "teamId": return query.OrderBy(t => t.TeamId);
                    case "location": return query.OrderBy(t => t.Location);
                    case "registrationTime": return query.OrderBy(t => t.RegisteredAt);
                }
            }
            else
            {
                switch (sortColumn)
                {
                    case "id": return query.OrderByDescending(t => t.Id);
                    case "teamId": return query.OrderByDescending(t => t.TeamId);
                    case "location": return query.OrderByDescending(t => t.Location);
                    case "registrationTime": return query.OrderByDescending(t => t.RegisteredAt);
                }
            }
            return query;
        }

        public string RegisterOrUpdate(RegisterTeam validRegisterTeam)
        {
            try
            {
                string msg = null;

                var alreadyRegisteredTeam = _teamBaseRepo.GetSingle(team => team.TeamId == validRegisterTeam.TeamId, team => team.Users);

                if (alreadyRegisteredTeam == null)
                {
                    var locationId = _context.Locations.Where(loc => loc.DisplayName == validRegisterTeam.Location).SingleOrDefault();

                    if (locationId == null)
                        throw new Exception("Invalid location provided in request form");

                    var newToken = Guid.NewGuid().ToString("N");

                    // Register New Team
                    var newTeam = new Team
                    {
                        TeamId = validRegisterTeam.TeamId,
                        Location = locationId.LocationId,
                        RegisteredAt = validRegisterTeam.RequestTime,
                        LastUpdatedAt = validRegisterTeam.RequestTime,
                        SecretToken = newToken,
                    };

                    var Users = new List<User>();

                    foreach (var user in validRegisterTeam.TeamMembers)
                        Users.Add(new User { UserId = user, AddedAt = validRegisterTeam.RequestTime });

                    newTeam.Users = Users;

                    _teamBaseRepo.Add(newTeam);

                    _teamBaseRepo.Commit();

                    msg = "Registration Success";

                    if (SendRegistrationEmail(newTeam.TeamId, newToken, newTeam.Users))
                        _logger.LogWarning($"Registration Email Sent for {newTeam.TeamId}");
                    else
                        _logger.LogError($"Email send failure for team {newTeam.TeamId}");
                }
                else
                {
                    // Update Already Registered Team

                    // Delete all current users
                    foreach (var user in alreadyRegisteredTeam.Users)
                        _userBaseRepo.Delete(user);

                    // Add all new users
                    foreach (var user in validRegisterTeam.TeamMembers)
                        _userBaseRepo.Add(new User { UserId = user, AddedAt = validRegisterTeam.RequestTime, TeamId = alreadyRegisteredTeam.TeamId });

                    alreadyRegisteredTeam.LastUpdatedAt = validRegisterTeam.RequestTime;

                    _teamBaseRepo.Commit();

                    msg = "Team Updated";
                }

                return msg;
            }
            catch(Exception ex)
            {
                _logger.LogError($"Registration Failed | {validRegisterTeam.TeamId} | {validRegisterTeam.Location}");
                _logger.LogError($"{ex}");
                return null;
            }
        }

        public List<string> GetLocations()
        {
            return _context.Locations.Select(loc => loc.DisplayName).ToList();
        }

        public bool TeamIdIsUnique(string teamId)
        {
            return _teamRepo.isTeamIdUnique(teamId.ToLowerInvariant());
        }

        public bool EmailIdIsUnique(string emailId)
        {
            return _userRepo.IsEmailUniq(emailId.ToLowerInvariant());
        }

        public bool DoesTeamExist(string teamId)
        {
            return _teamBaseRepo.FindBy(team => team.TeamId == teamId).Count() == 1;
        }

        public bool IsSecretTokenCorrect(string teamId, string secretToken)
        {
            return _teamBaseRepo.FindBy(team => team.TeamId == teamId && team.SecretToken == secretToken).Count() == 1;
        }

        public List<string> GetTeamMembers(string teamId)
        {
            var team = _teamBaseRepo.GetSingle(team => team.TeamId == teamId, team => team.Users);

            var memberList = new List<string>();

            team.Users.ToList().ForEach(u => memberList.Add(u.UserId));

            return memberList;
        }

        public bool UpdateTeamMembers(string teamId, List<string> newTeamMembers)
        {
            try
            {
                var oldTeamMembers = _context.Users.Where(user => user.TeamId == teamId).Select(user => user.UserId).ToList();

                var currentTeam = _teamBaseRepo.GetSingle(team => team.TeamId == teamId);

                var teamMembersToDelete = new List<string>();
                var teamMembersToAdd = new List<string>();

                foreach (var tm in oldTeamMembers)
                {
                    if (!newTeamMembers.Contains(tm))
                        teamMembersToDelete.Add(tm);
                }

                foreach (var tm in newTeamMembers)
                {
                    if (!oldTeamMembers.Contains(tm))
                        teamMembersToAdd.Add(tm);
                }

                if (teamMembersToAdd.Count == 0 && teamMembersToDelete.Count == 0)
                    return false;

                // Delete

                foreach (var tm in teamMembersToDelete)
                {
                    _userBaseRepo.DeleteWhere(user => user.UserId == tm);
                }

                // Add

                foreach (var tm in teamMembersToAdd)
                {
                    if (EmailIdIsUnique(tm))
                        _userBaseRepo.Add(new User { UserId = tm, TeamId = teamId, AddedAt = DateTime.UtcNow });
                }

                currentTeam.LastUpdatedAt = DateTime.UtcNow;

                _userBaseRepo.Commit();

                if (SendTeamChangeEmail(teamId, GetTeamMembers(teamId)))
                    _logger.LogWarning($"Team Update Email sent for {teamId}");
                else
                    _logger.LogError($"Team Update Email sent failure for {teamId}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"TeamMembers Change Failed, {ex}");
                return false;
            }
        }

        private bool SendTeamChangeEmail(string teamId, List<string> teamMembers)
        {
            try {
                var sender = Environment.GetEnvironmentVariable("FROM_ADDRESS");

                return _emailNotificationService.Notify(new NotificationContent
                {
                    Subject = "CodeComp - Team Change Successful",
                    Recievers = teamMembers,
                    Sender = sender,
                    CcUsers = new List<string>(),
                    BccUsers = new List<string>(),
                    Body = "Your team has been changed successfully.",
                });
            }
            catch(Exception ex)
            {
                _logger.LogError($"EmailNotification for Team Change Failed | {teamId} | {ex}");
                return false;
            }
        }

        private bool SendRegistrationEmail(string teamId, string newToken, ICollection<User> users)
        {
            try
            {
                var sender = Environment.GetEnvironmentVariable("FROM_ADDRESS");
                var notificationContent = new NotificationContent
                {
                    Subject = "CodeComp - Team Registration Successful",
                    Recievers = users.Select(u => u.UserId).ToList(),
                    Sender = sender,
                    CcUsers = new List<string>(),
                    BccUsers = new List<string>(),
                    Body = $"Your team {teamId} is successfully registered. Your secretToken is {newToken}",
                };

                return _emailNotificationService.Notify(notificationContent);
            }
            catch(Exception ex)
            {
                _logger.LogError($"EmailNotification for Team Registration Failed | {teamId} | {ex}");
                return false;
            }
        }

        public bool EmailIdIsUnique(string userId, string teamId)
        {
            return _userRepo.IsEmailUniq(userId, teamId);
        }
    }
}
