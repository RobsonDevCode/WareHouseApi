namespace SelfProjectApi.Configuration
{
    internal class ConnectionString
    {
        private IConfiguration _config;
        internal ConnectionString(IConfiguration configuration) 
        {
            _config = configuration;
        }

        //settings connection strings from the appsettings.json
        public string OrderDataBase => _config.GetConnectionString(nameof(OrderDataBase)) ??
            throw new NullReferenceException($"Connection: {nameof(OrderDataBase)} hasnt been configured correctly");

        public string SupplierDataBase => _config.GetConnectionString(nameof(SupplierDataBase)) ??
            throw new NullReferenceException($"Connection: {nameof(SupplierDataBase)} hasnt been configured correctly");

        public string LoggingDataBase => _config.GetConnectionString(nameof(LoggingDataBase)) ??
            throw new NullReferenceException($"Connection {nameof(LoggingDataBase)} hasnt been configured correctly");
    }
}
