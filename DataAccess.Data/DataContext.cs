using DataAccess.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Data
{
    public class DataContext : DbContext
    {
        public virtual DbSet<FutureGame> FutureGames { get; set; }
        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<Guess> Guesses { get; set; }
        public virtual DbSet<Kill> Kills { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Participant> Participants { get; set; }
        public virtual DbSet<Phase> Phases { get; set; }
        public virtual DbSet<Request> Requests { get; set; }
        public virtual DbSet<Round> Rounds { get; set; }
        public virtual DbSet<RoundConfig> RoundConfigs { get; set; }
        public virtual DbSet<Score> Scores { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<ThrottledRequest> ThrottledRequests { get; set; }
        public virtual DbSet<User> Users { get; set; }

        public DataContext()
        {

        }
        private const string TIMESTAMP_TYPE = "timestamp with time zone";

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            ConfigureModelBuilderForUser(modelBuilder);
        }

        void ConfigureModelBuilderForUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FutureGame>(entity =>
            {
                entity.Property(fg => fg.Id).ValueGeneratedOnAdd();
                
                entity.HasKey(fg => fg.Id);
                
                entity.Property(fg => fg.StartTime).IsRequired().HasColumnType(TIMESTAMP_TYPE);

                entity.Property(fg => fg.RunOnce).IsRequired().HasDefaultValue(false);
                
                entity.Property(fg => fg.RunGamesTill).HasColumnType(TIMESTAMP_TYPE).HasDefaultValue(null);
            });

            modelBuilder.Entity<Game>(entity => 
            {
                entity.Property(game => game.Id).ValueGeneratedOnAdd();

                entity.HasKey(game => game.GameId);
                
                entity.Property(game => game.TimeStamp).HasColumnType(TIMESTAMP_TYPE).IsRequired();
            });

            modelBuilder.Entity<Guess>(entity => 
            {
                entity.Property(guess => guess.Id).ValueGeneratedOnAdd();

                entity.HasKey(guess => guess.GuessId);

                entity.Property(guess => guess.GameId).IsRequired();
                entity.Property(guess => guess.RoundId).IsRequired();
                entity.Property(guess => guess.TeamId).IsRequired().HasMaxLength(20);
                entity.Property(guess => guess.TimeStamp).IsRequired().HasColumnType(TIMESTAMP_TYPE);

                entity.Property(guess => guess.GuessRequest).HasColumnType("jsonb");
                entity.Property(guess => guess.GuessResponse).HasColumnType("jsonb");

                entity.HasOne(d => d.Game)
                    .WithMany(f => f.Guesses)
                    .HasForeignKey(d => d.GameId)
                    .HasConstraintName("FK__Guess__Game");

                entity.HasOne(d => d.Round)
                    .WithMany(f => f.Guesses)
                    .HasForeignKey(d => d.RoundId)
                    .HasConstraintName("FK__Guess__Round");
                
                entity.HasOne(d => d.Team)
                    .WithMany(f => f.Guesses)
                    .HasForeignKey(d => d.TeamId)
                    .HasConstraintName("FK__Guess__Team");
            });

            modelBuilder.Entity<Kill>(entity => 
            {
                entity.Property(kill => kill.Id).ValueGeneratedOnAdd();
                
                entity.HasKey(kill => new { kill.RoundId, kill.VictimId, kill.KillerId });

                entity.Property(kill => kill.KillerId).IsRequired().HasMaxLength(20);

                entity.Property(kill => kill.VictimId).IsRequired().HasMaxLength(20);

                entity.Property(kill => kill.TimeStamp).IsRequired().HasColumnType(TIMESTAMP_TYPE);

                entity.HasOne(kill => kill.Game)
                    .WithMany(game => game.Kills)
                    .HasForeignKey(kill => kill.GameId)
                    .HasConstraintName("FK__Kill__Game");

                entity.HasOne(kill => kill.Round)
                    .WithMany(round => round.Kills)
                    .HasForeignKey(kill => kill.RoundId)
                    .HasConstraintName("FK__Kill__Round");
                
                entity.HasOne(kill => kill.Victim)
                    .WithMany(team => team.DeathRecord)
                    .HasForeignKey(kill => kill.VictimId)
                    .HasConstraintName("FK__Kill__Victim");

                entity.HasOne(kill => kill.Killer)
                    .WithMany(team => team.KillRecord)
                    .HasForeignKey(kill => kill.KillerId)
                    .HasConstraintName("FK__Kill__Killer");

            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.Property(loc => loc.Id).ValueGeneratedOnAdd();
             
                entity.Property(loc => loc.LocationId)
                .HasMaxLength(20)
                .IsRequired();

                entity.HasKey(loc => loc.LocationId);

                entity.Property(loc => loc.DisplayName).IsRequired();
            });

            modelBuilder.Entity<Participant>(entity => 
            {
                entity.Property(p => p.Id).ValueGeneratedOnAdd();

                entity.HasKey(p => new { p.RoundId, p.TeamId });
                
                entity.Property(p => p.GameId).IsRequired();

                entity.Property(p => p.RoundId).IsRequired();

                entity.Property(p => p.TeamId).IsRequired().HasMaxLength(20);

                entity.Property(p => p.Secret).IsRequired();

                entity.Property(p => p.IsAlive).IsRequired().HasDefaultValue(true);

                entity.Property(guess => guess.JoinedAt).IsRequired().HasColumnType(TIMESTAMP_TYPE);

                entity.HasOne(d => d.Game)
                    .WithMany(f => f.Participants)
                    .HasForeignKey(d => d.GameId)
                    .HasConstraintName("FK__Part__Game");

                entity.HasOne(d => d.Round)
                    .WithMany(f => f.Participants)
                    .HasForeignKey(d => d.RoundId)
                    .HasConstraintName("FK__Part__Round");
                
                entity.HasOne(d => d.Team)
                    .WithMany(f => f.ParticipationRecord)
                    .HasForeignKey(d => d.TeamId)
                    .HasConstraintName("FK__Part__Team");
            });

            modelBuilder.Entity<Phase>(entity => 
            {
                entity.Property(phase => phase.Id).ValueGeneratedOnAdd();

                entity.HasKey(phase => new { phase.RoundId, phase.PhaseType });

                entity.Property(phase => phase.GameId).IsRequired();
                
                entity.Property(phase => phase.RoundId).IsRequired();

                entity.Property(phase => phase.PhaseType).HasConversion(
                    type => type.ToString(), // code to db
                    type => (PhaseType)Enum.Parse(typeof(PhaseType), type) // db to code
                ).IsRequired();

                entity.Property(phase => phase.TimeStamp).HasColumnType(TIMESTAMP_TYPE).IsRequired();

                entity.HasOne(phase => phase.Game)
                    .WithMany(game => game.Phases)
                    .HasForeignKey(phase => phase.GameId)
                    .HasConstraintName("FK__Phase__Game");

                entity.HasOne(phase => phase.Round)
                    .WithMany(round => round.RoundPhases)
                    .HasForeignKey(phase => phase.RoundId)
                    .HasConstraintName("FK__Phase__Round");
            });

            modelBuilder.Entity<Request>(entity => 
            {
                entity.Property(req => req.Id).ValueGeneratedOnAdd();

                entity.HasKey(req => req.RequestId);

                //entity.Property(req => req.RequestApi).HasConversion(
                //    type => type.ToString(), // code to db
                //    type => (RequestApi)Enum.Parse(typeof(RequestApi), type) // db to code
                //).IsRequired();

                entity.Property(req => req.RequestApi).IsRequired();

                entity.Property(req => req.RequestMethod).HasConversion(
                    type => type.ToString(), // code to db
                    type => (RequestMethod)Enum.Parse(typeof(RequestMethod), type) // db to code
                ).IsRequired();

                entity.Property(req => req.StatusCode).IsRequired();

                entity.Property(req => req.GameId).HasDefaultValue(null);
                entity.Property(req => req.RoundId).HasDefaultValue(null);
                entity.Property(req => req.TeamId).HasDefaultValue(null);

                entity.Property(guess => guess.TimeStamp).IsRequired().HasColumnType(TIMESTAMP_TYPE);

                entity.HasOne(d => d.Game)
                    .WithMany(f => f.Requests)
                    .HasForeignKey(d => d.GameId)
                    .HasConstraintName("FK__Req__Game");

                entity.HasOne(d => d.Round)
                    .WithMany(f => f.Requests)
                    .HasForeignKey(d => d.RoundId)
                    .HasConstraintName("FK__Req__Round");
                
                entity.HasOne(d => d.Team)
                    .WithMany(f => f.Requests)
                    .HasForeignKey(d => d.TeamId)
                    .HasConstraintName("FK__Req__Team");
            });

            modelBuilder.Entity<Round>(entity => 
            {
                entity.Property(round => round.Id).ValueGeneratedOnAdd();

                entity.HasKey(round => round.RoundId);

                entity.Property(round => round.TimeStamp).HasColumnType(TIMESTAMP_TYPE).IsRequired();

                entity.Property(round => round.RoundNumber).IsRequired();

                entity.HasOne(round => round.Game)
                    .WithMany(game => game.Rounds)
                    .HasForeignKey(round => round.GameId)
                    .HasConstraintName("FK__Round__Game");
            });

            modelBuilder.Entity<RoundConfig>(entity => 
            {
                entity.HasKey(rc => rc.Id);
                entity.Property(rc => rc.Id).HasColumnName("RoundNumber");
                entity.Property(rc => rc.JoiningDuration).IsRequired();
                entity.Property(rc => rc.RunningDuration).IsRequired();
                entity.Property(rc => rc.FinishedDuration).IsRequired();
                entity.Property(rc => rc.Penalty).IsRequired();
                entity.Property(rc => rc.LifeLines).IsRequired();
            });

            modelBuilder.Entity<Score>(entity => {

                entity.Property(s => s.Id).ValueGeneratedOnAdd();

                entity.HasKey(s => s.Id);

                entity.Property(s => s.PointsScored).IsRequired().HasDefaultValue(0);

                entity.Property(s => s.GameId).IsRequired();

                entity.Property(s => s.RoundId).IsRequired();

                entity.Property(s => s.TeamId).IsRequired();

                entity.Property(guess => guess.TimeStamp).IsRequired().HasColumnType(TIMESTAMP_TYPE);

                entity.HasOne(d => d.Game)
                    .WithMany(f => f.Scores)
                    .HasForeignKey(d => d.GameId)
                    .HasConstraintName("FK__Score__Game");

                entity.HasOne(d => d.Round)
                    .WithMany(f => f.Scores)
                    .HasForeignKey(d => d.RoundId)
                    .HasConstraintName("FK__Score__Round");
                
                entity.HasOne(d => d.Team)
                    .WithMany(f => f.Scores)
                    .HasForeignKey(d => d.TeamId)
                    .HasConstraintName("FK__Score__Team");

                entity.HasOne(d => d.Guess)
                    .WithOne(f => f.Score)
                    .HasForeignKey<Score>(d => d.GuessId)
                    .HasConstraintName("FK__Score__Guess");
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.Property(team => team.TeamId)
                .HasMaxLength(20)
                .IsRequired();

                entity.HasKey(team => team.TeamId);

                entity.Property(team => team.RegisteredAt).HasColumnType(TIMESTAMP_TYPE);

                entity.Property(team => team.LastUpdatedAt).HasColumnType(TIMESTAMP_TYPE);

                entity.Property(team => team.IsRobot).HasDefaultValue(false);

                entity.Property(team => team.SecretToken).IsRequired();

                entity.HasOne(team => team.LocationNav)
                      .WithMany(loc => loc.Teams)
                      .HasForeignKey(team => team.Location)
                      .HasConstraintName("FK_Team_Loc");

                entity.Property(loc => loc.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<ThrottledRequest>(entity =>
            {
                entity.Property(t => t.Id).ValueGeneratedOnAdd();
                entity.HasKey(t => t.HitId);
                entity.Property(t => t.TeamId).HasMaxLength(20).IsRequired();
                entity.Property(t => t.LastHit).HasColumnType(TIMESTAMP_TYPE);

                entity.HasOne(t => t.Team).WithMany(t => t.ThrottledRequests)
                    .HasForeignKey(t => t.TeamId)
                    .HasConstraintName("FK__TRreq__Team");
            });

            modelBuilder.Entity<User>(entity => 
            {
                entity.HasKey(user => user.UserId);

                entity.Property(user => user.AddedAt).HasColumnType(TIMESTAMP_TYPE);

                entity.Property(loc => loc.Id).ValueGeneratedOnAdd();

                entity.Property(user => user.UserId)
                .HasMaxLength(60)
                .IsRequired();

                entity.Property(user => user.TeamId)
                .HasMaxLength(60)
                .IsRequired();

                entity.HasOne(p => p.Team).WithMany(d => d.Users).HasConstraintName("FK_User_Team").HasForeignKey(d => d.TeamId);
            });
        }
    }
}
