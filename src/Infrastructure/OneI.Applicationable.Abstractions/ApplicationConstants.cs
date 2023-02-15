namespace OneI.Applicationable;

public static class ApplicationConstants
{
    public static class Configurations
    {
        public const string EnvironmentName = "environment";
        public const string ApplicationName = "name";
        public const string RootPath = "root";
    }

    public static class Environments
    {
        public const string Development = "Development";

        public const string Staging = "Staging";

        public const string Production = "Production";
    }

    public static class Listener
    {
        public const string Name = "OneI.Applicationable.Application";

        public static class Events
        {
            public const string Building = nameof(Building);
        }
    }
}
