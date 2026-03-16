using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Products.Configuration;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.XmlDownloads;
using OneOf;
using OneOf.Types;
using System.Data;
using System.Transactions;

using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.XmlDownloadsTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess;

internal sealed class XmlDownloadsRepository : IXmlDownloadsRepository
{
    public XmlDownloadsRepository(
         [FromKeyedServices(ConfigureServices.LocalDBConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<OneOf<Success, UnexpectedFailureResult>> InsertAsync(XmlDownloadData xmlDownloadData)
    {
        const string query =
            $"""
            INSERT INTO {XmlDownloadsTableName} (
                {TimeStampColumnName},
                {XmlResourceTypeColumnName},
                {BIDColumnName},
                {UserNameColumnName},
                {ContactPersonColumnName})
            VALUES (@TimeStamp, @XmlResourceType, @BID, @UserName, @ContactPerson)
            """;

        var parameters = new
        {
            TimeStamp = xmlDownloadData.TimeStamp,
            XmlResourceType = xmlDownloadData.XmlResourceType,
            BID = xmlDownloadData.BID,
            UserName = xmlDownloadData.UserName,
            ContactPerson = xmlDownloadData.ContactPerson,
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int result = await dbConnection.ExecuteAsync(query, parameters, commandType: CommandType.Text);

        return result > 0 ? new Success() : new UnexpectedFailureResult();
    }
}