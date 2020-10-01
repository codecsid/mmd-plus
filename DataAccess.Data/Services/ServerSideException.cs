using System;
using System.Runtime.Serialization;

namespace DataAccess.Data.Services
{
    [Serializable]
    public class ServerSideException : Exception
    {
        public ServerSideException()
        {
        }

        public ServerSideException(string message) : base(message)
        {
        }

        public ServerSideException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ServerSideException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class InvalidDataProvidedException : Exception
    {
        public InvalidDataProvidedException()
        {
        }

        public InvalidDataProvidedException(string message) : base(message)
        {
        }

        public InvalidDataProvidedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidDataProvidedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class TeamNotJoinedException : Exception
    {
        public TeamNotJoinedException()
        {
        }

        public TeamNotJoinedException(string message) : base(message)
        {
        }

        public TeamNotJoinedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TeamNotJoinedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class GameNotInRunningPhaseException : Exception
    {
        public GameNotInRunningPhaseException()
        {
        }

        public GameNotInRunningPhaseException(string message) : base(message)
        {
        }

        public GameNotInRunningPhaseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected GameNotInRunningPhaseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public class GuessingTeamDeadException : Exception
    {

    }

    public class TargetTeamDeadException : Exception
    {

    }

    public class TargetAlreadyKilledException : Exception
    {

    }

    public class GuessLimitExceededException : Exception
    {

    }
}