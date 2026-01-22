using Microsoft.Data.SqlClient;
using MOSTComputers.Services.Identity.DAL.Contracts;
using MOSTComputers.Services.Identity.Models.Customers;
using System.Data;
using System.Diagnostics;

namespace MOSTComputers.Services.Identity.DAL;

internal sealed class CustomersViewLoginDataRepository : ICustomersViewLoginDataRepository
{
    internal static class CustomerDataView
    {
        internal const string Name = "ViewCustomersID";
        internal const string IdColumn = "BID";
        internal const string NameColumn = "Name";
        internal const string ContactPersonNameColumn = "ContactPerson";
        internal const string CountryColumn = "Country";
        internal const string AddressColumn = "Address";
        internal const string EmployeeIdColumn = "EmployeeID";
        internal const string LoginNameColumn = "LogonName";
        internal const string PasswordColumn = "Password";
    }

    private readonly string _connectionString;

    public CustomersViewLoginDataRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<CustomerData?> GetByIdAsync(int id)
    {
        const string query =
            $"""
            SELECT TOP 1 {CustomerDataView.IdColumn},
                {CustomerDataView.NameColumn},
                {CustomerDataView.ContactPersonNameColumn},
                {CustomerDataView.CountryColumn},
                {CustomerDataView.AddressColumn},
                {CustomerDataView.EmployeeIdColumn},
                {CustomerDataView.LoginNameColumn}
            FROM {CustomerDataView.Name} WITH (NOLOCK)
            WHERE {CustomerDataView.IdColumn} = @id;
            """;

        using SqlCommand sqlCommand = new()
        {
            CommandText = query,
            CommandType = CommandType.Text
        };

        SqlParameter idParameter = new()
        {
            ParameterName = "@id",
            SqlDbType = SqlDbType.Int,
            Value = id,
        };

        sqlCommand.Parameters.Add(idParameter);

        using SqlConnection sqlConnection = new(_connectionString);

        await sqlConnection.OpenAsync();

        sqlCommand.Connection = sqlConnection;

        using SqlDataReader resultReader = await sqlCommand.ExecuteReaderAsync(CommandBehavior.SingleResult);

        bool exists = await resultReader.ReadAsync();

        if (!exists) return null;

        CustomerData customerData = new()
        {
            Id = resultReader.GetInt32(0),
            Name = GetFieldValueOrDefault<string?>(resultReader, 1),
            ContactPersonName = GetFieldValueOrDefault<string?>(resultReader, 2),
            Country = GetFieldValueOrDefault<string?>(resultReader, 3),
            Address = GetFieldValueOrDefault<string?>(resultReader, 4),
            EmployeeId = GetFieldValueOrDefault<int?>(resultReader, 5),
            Username = GetFieldValueOrDefault<string?>(resultReader, 6)
        };

        return customerData;
    }

    public async Task<CustomerData?> GetByUsernameAsync(string username)
    {
        const string query =
            $"""
            SELECT TOP 1 {CustomerDataView.IdColumn},
                {CustomerDataView.NameColumn},
                {CustomerDataView.ContactPersonNameColumn},
                {CustomerDataView.CountryColumn},
                {CustomerDataView.AddressColumn},
                {CustomerDataView.EmployeeIdColumn},
                {CustomerDataView.LoginNameColumn}
            FROM {CustomerDataView.Name} WITH (NOLOCK)
            WHERE {CustomerDataView.LoginNameColumn} COLLATE Cyrillic_General_CS_AS = @username;
            """;

        using SqlCommand sqlCommand = new()
        {
            CommandText = query,
            CommandType = CommandType.Text
        };

        SqlParameter usernameParameter = new()
        {
            ParameterName = "@username",
            SqlDbType = SqlDbType.VarChar,
            Value = username,
        };

        sqlCommand.Parameters.Add(usernameParameter);

        using SqlConnection sqlConnection = new(_connectionString);

        await sqlConnection.OpenAsync();

        sqlCommand.Connection = sqlConnection;

        using SqlDataReader resultReader = await sqlCommand.ExecuteReaderAsync(CommandBehavior.SingleResult);

        bool exists = await resultReader.ReadAsync();

        if (!exists) return null;

        CustomerData customerData = new()
        {
            Id = resultReader.GetInt32(0),
            Name = GetFieldValueOrDefault<string?>(resultReader, 1),
            ContactPersonName = GetFieldValueOrDefault<string?>(resultReader, 2),
            Country = GetFieldValueOrDefault<string?>(resultReader, 3),
            Address = GetFieldValueOrDefault<string?>(resultReader, 4),
            EmployeeId = GetFieldValueOrDefault<int?>(resultReader, 5),
            Username = GetFieldValueOrDefault<string?>(resultReader, 6)
        };

        return customerData;
    }

    public async Task<CustomerLoginData?> GetLoginDataByIdAsync(int id)
    {
        const string query =
            $"""
            SELECT TOP 1 {CustomerDataView.IdColumn}, {CustomerDataView.LoginNameColumn}, {CustomerDataView.ContactPersonNameColumn}
            FROM {CustomerDataView.Name} WITH (NOLOCK)
            WHERE {CustomerDataView.IdColumn} = @id;
            """;

        using SqlCommand sqlCommand = new()
        {
            CommandText = query,
            CommandType = CommandType.Text
        };

        SqlParameter idParameter = new()
        {
            ParameterName = "@id",
            SqlDbType = SqlDbType.Int,
            Value = id,
        };

        sqlCommand.Parameters.Add(idParameter);

        using SqlConnection sqlConnection = new(_connectionString);

        await sqlConnection.OpenAsync();

        sqlCommand.Connection = sqlConnection;

        using SqlDataReader resultReader = await sqlCommand.ExecuteReaderAsync(CommandBehavior.SingleResult);

        bool exists = await resultReader.ReadAsync();

        if (!exists) return null;

        CustomerLoginData customerLoginData = new()
        {
            Id = resultReader.GetInt32(0),
            Username = GetFieldValueOrDefault<string?>(resultReader, 1),
            ContactPerson = GetFieldValueOrDefault<string?>(resultReader, 2),
        };

        return customerLoginData;
    }

    public async Task<CustomerLoginData?> GetLoginDataByUsernameAsync(string username)
    {
        const string query =
            $"""
            SELECT TOP 1 {CustomerDataView.IdColumn}, {CustomerDataView.LoginNameColumn}, {CustomerDataView.ContactPersonNameColumn}
            FROM {CustomerDataView.Name} WITH (NOLOCK)
            WHERE {CustomerDataView.LoginNameColumn} COLLATE Cyrillic_General_CS_AS = @username;
            """;

        using SqlCommand sqlCommand = new()
        {
            CommandText = query,
            CommandType = CommandType.Text
        };

        SqlParameter usernameParameter = new()
        {
            ParameterName = "@username",
            SqlDbType = SqlDbType.VarChar,
            Value = username,
        };

        sqlCommand.Parameters.Add(usernameParameter);

        using SqlConnection sqlConnection = new(_connectionString);

        await sqlConnection.OpenAsync();

        sqlCommand.Connection = sqlConnection;

        using SqlDataReader resultReader = await sqlCommand.ExecuteReaderAsync(CommandBehavior.SingleResult);

        bool exists = await resultReader.ReadAsync();

        if (!exists) return null;

        CustomerLoginData customerLoginData = new()
        {
            Id = resultReader.GetInt32(0),
            Username = GetFieldValueOrDefault<string?>(resultReader, 1),
            ContactPerson = GetFieldValueOrDefault<string?>(resultReader, 2),
        };

        return customerLoginData;
    }

    public async Task<CheckPasswordResult> IsPasswordEqualToExistingAsync(int id, string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return CheckPasswordResult.NoMatch;
        }

        const string query =
            $"""
            IF NOT EXISTS (SELECT 1 FROM {CustomerDataView.Name} WITH (NOLOCK) WHERE {CustomerDataView.IdColumn} = @id)
            BEGIN
                SELECT 2;
            END
            ELSE
            BEGIN
                SELECT CASE
                    WHEN (SELECT {CustomerDataView.PasswordColumn} FROM {CustomerDataView.Name} WITH (NOLOCK) WHERE {CustomerDataView.IdColumn} = @id) = @password THEN 0
                    ELSE 1
            END
            """;

        using SqlCommand sqlCommand = new()
        {
            CommandText = query,
            CommandType = CommandType.Text
        };

        SqlParameter idParameter = new()
        {
            ParameterName = "@id",
            SqlDbType = SqlDbType.Int,
            IsNullable = false,
            Value = id,
        };

        SqlParameter passwordParameter = new()
        {
            ParameterName = "@password",
            SqlDbType = SqlDbType.VarChar,
            IsNullable = false,
            Value = password,
        };

        sqlCommand.Parameters.Add(idParameter);
        sqlCommand.Parameters.Add(passwordParameter);

        using SqlConnection sqlConnection = new(_connectionString);

        await sqlConnection.OpenAsync();

        sqlCommand.Connection = sqlConnection;

        using SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync(CommandBehavior.SingleResult);

        await sqlDataReader.ReadAsync();

        if (sqlDataReader.IsDBNull(0))
        {
            return CheckPasswordResult.UserDoesNotExist;
        }

        int data = sqlDataReader.GetInt32(0);

        return data switch
        {
            0 => CheckPasswordResult.Success,
            1 => CheckPasswordResult.NoMatch,
            2 => CheckPasswordResult.UserDoesNotExist,
            _ => throw new UnreachableException()
        };
    }

    public async Task<CheckPasswordResult> IsPasswordEqualToExistingAsync(string username, string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return CheckPasswordResult.NoMatch;
        }

        const string query =
            $"""
            IF NOT EXISTS (SELECT 1 FROM {CustomerDataView.Name} WITH (NOLOCK)
                WHERE {CustomerDataView.LoginNameColumn} COLLATE Cyrillic_General_CS_AS = @username)
            BEGIN
                SELECT 2;
            END
            ELSE
            BEGIN
                SELECT CASE
                    WHEN (SELECT TOP 1 {CustomerDataView.PasswordColumn} FROM {CustomerDataView.Name} WITH (NOLOCK)
                        WHERE {CustomerDataView.LoginNameColumn} COLLATE Cyrillic_General_CS_AS = @username) COLLATE Cyrillic_General_CS_AS = @password
                        THEN 0
                    ELSE 1
                END
            END
            """;

        using SqlCommand sqlCommand = new()
        {
            CommandText = query,
            CommandType = CommandType.Text
        };

        SqlParameter usernameParameter = new()
        {
            ParameterName = "@username",
            SqlDbType = SqlDbType.VarChar,
            IsNullable = false,
            Value = username,
        };

        SqlParameter passwordParameter = new()
        {
            ParameterName = "@password",
            SqlDbType = SqlDbType.VarChar,
            IsNullable = false,
            Value = password,
        };

        sqlCommand.Parameters.Add(usernameParameter);
        sqlCommand.Parameters.Add(passwordParameter);

        using SqlConnection sqlConnection = new(_connectionString);

        await sqlConnection.OpenAsync();

        sqlCommand.Connection = sqlConnection;

        using SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync(CommandBehavior.SingleResult);

        await sqlDataReader.ReadAsync();

        if (sqlDataReader.IsDBNull(0))
        {
            return CheckPasswordResult.UserDoesNotExist;
        }

        int data = sqlDataReader.GetInt32(0);

        return data switch
        {
            0 => CheckPasswordResult.Success,
            1 => CheckPasswordResult.NoMatch,
            2 => CheckPasswordResult.UserDoesNotExist,
            _ => throw new UnreachableException()
        };
    }

    private static T? GetFieldValueOrDefault<T>(SqlDataReader reader, int columnOrdinal)
    {
        if (reader.IsDBNull(columnOrdinal)) return default;

        return reader.GetFieldValue<T>(columnOrdinal);
    }
}