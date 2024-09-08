namespace SelfProjectApi.Configuration
{
    /// <summary>
    /// Class to set the confguration of the api so we can refernce static infomation stored when we compile.
    /// </summary>
    internal static class Settings
    {
        private static IConfiguration _config { get; set; }

        //Initilize configuration settings based pulling mostly from appsettings
        internal static void Initilize(IConfiguration config)
        {
            _config = config;
        }

        public static ConnectionString ConnectionString => new ConnectionString(_config);

        public static string ApplicationName = nameof(SelfProjectApi).Replace("Api", "");

        //from jwt
        public const string AdminUserClaimName = "admin";

        public const string AdminUserPolicyName = "Admin";
    }
}
