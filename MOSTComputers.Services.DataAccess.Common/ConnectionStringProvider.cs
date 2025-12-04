namespace MOSTComputers.Services.DataAccess.Common;
public class ConnectionStringProvider : IConnectionStringProvider
{
    public ConnectionStringProvider(string connectionString)
    {
        _connectionString = connectionString;
    }

    private readonly string _connectionString;

    public string ConnectionString => _connectionString;
}