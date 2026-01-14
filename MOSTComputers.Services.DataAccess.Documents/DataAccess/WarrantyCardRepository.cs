using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Documents.Configuration;
using MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.Services.DataAccess.Documents.Models.DAO;
using MOSTComputers.Services.DataAccess.Documents.Models.Requests.Invoice;
using MOSTComputers.Services.DataAccess.Documents.Models.Requests.WarrantyCard;
using System.Data;
using System.Text;
using System.Transactions;
using static MOSTComputers.Services.DataAccess.Documents.Mapping.ResponseMapper;
using static MOSTComputers.Services.DataAccess.Documents.Utils.SqlGenerateUtils;
using static MOSTComputers.Services.DataAccess.Documents.Utils.TableAndColumnNameUtils;
using static MOSTComputers.Services.DataAccess.Documents.Utils.TableAndColumnNameUtils.WarrantyCardsTable;

namespace MOSTComputers.Services.DataAccess.Documents.DataAccess;
internal sealed class WarrantyCardRepository : IWarrantyCardRepository
{
    public WarrantyCardRepository([FromKeyedServices(ConfigureServices.DocumentsDataAccessServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    const string _selectWithItemsQueryBase =
        $"""
        SELECT warrantyCards.{ExportIdColumn},
            warrantyCards.{ExportDateColumn},
            warrantyCards.{ExportUserIdColumn},
            warrantyCards.{ExportUserColumn},
            warrantyCards.{OrderIdColumn},
            warrantyCards.{CustomerBIDColumn},
            warrantyCards.{CustomerNameColumn},
            warrantyCards.{WarrantyCardDateColumn},
            warrantyCards.{WarrantyCardTermColumn},

            warrantyCardItems.{WarrantyCardItemsTable.ExportedItemIdColumn},
            warrantyCardItems.{WarrantyCardItemsTable.ExportIdColumn} AS {WarrantyCardItemsTable.ExportIdAlias},
            warrantyCardItems.{WarrantyCardItemsTable.OrderIdColumn} AS {WarrantyCardItemsTable.OrderIdAlias},
            warrantyCardItems.{WarrantyCardItemsTable.ProductIdColumn},
            warrantyCardItems.{WarrantyCardItemsTable.ProductNameColumn},
            warrantyCardItems.{WarrantyCardItemsTable.PriceInLevaColumn},
            warrantyCardItems.{WarrantyCardItemsTable.QuantityColumn},
            warrantyCardItems.{WarrantyCardItemsTable.SerialNumberColumn},
            warrantyCardItems.{WarrantyCardItemsTable.WarrantyCardItemTermInMonthsColumn} AS {WarrantyCardItemsTable.WarrantyCardItemTermInMonthsAlias},
            warrantyCardItems.{WarrantyCardItemsTable.DisplayOrderColumn}

        FROM {WarrantyCardsTableName} warrantyCards WITH (NOLOCK)
        LEFT JOIN {WarrantyCardItemsTableName} warrantyCardItems WITH (NOLOCK)
        ON warrantyCards.{OrderIdColumn} = warrantyCardItems.{WarrantyCardItemsTable.OrderIdColumn}
        """;

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<List<WarrantyCard>> GetAllMatchingAsync(WarrantyCardSearchRequest warrantyCardSearchRequest)
    {
        (string whereClause, DynamicParameters parameters) = GetQueryAndAssignParametersFromSearchData(warrantyCardSearchRequest, true);

        string query =
            $"""
            {_selectWithItemsQueryBase}
            {whereClause}
            """;

        List<WarrantyCardDAO> output = new();

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        await dbConnection.QueryAsync<WarrantyCardDAO, WarrantyCardItemDAO, WarrantyCardDAO>(
            query,
            (warrantyCard, warrantyCardItem) =>
            {
                WarrantyCardDAO? existingWarrantyCard = output.FirstOrDefault(x => x.ExportId == warrantyCard.ExportId);

                if (existingWarrantyCard is null)
                {
                    warrantyCard.WarrantyCardItems = new();

                    output.Add(warrantyCard);

                    existingWarrantyCard = warrantyCard;
                }

                if (warrantyCardItem is not null)
                {
                    existingWarrantyCard.WarrantyCardItems!.Add(warrantyCardItem);
                }

                return existingWarrantyCard;
            },
            parameters,
            splitOn: $"{WarrantyCardItemsTable.ExportedItemIdColumn}",
            commandType: CommandType.Text);

        transactionScope.Complete();

        return MapRange(output);
    }

    public async Task<List<WarrantyCardCustomerData>> GetWarrantyCardCustomerInfosAsync(WarrantyCardSearchRequest warrantyCardSearchRequest)
    {
        string selectClause =
            $"""
            SELECT {CustomerNameColumn}, MIN({CustomerBIDColumn}) AS {CustomerBIDColumn}
            FROM {WarrantyCardsTableName}
            """;

        string groupByClause = $"GROUP BY {CustomerNameColumn}";

        (string whereClause, DynamicParameters parametersForQueryWithConditions) = GetQueryAndAssignParametersFromSearchData(warrantyCardSearchRequest, true);

        string fullQueryWithConditions =
            $"""
            {selectClause}
            {whereClause}
            {groupByClause}
            """;

        using TransactionScope transactionScopeForQueryWithConditions = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnectionForQueryWithConditions = new(_connectionStringProvider.ConnectionString);

        IEnumerable<WarrantyCardCustomerData> dataForQueryWithConditions = await dbConnectionForQueryWithConditions.QueryAsync<WarrantyCardCustomerData>(
            fullQueryWithConditions, parametersForQueryWithConditions, commandType: CommandType.Text);

        transactionScopeForQueryWithConditions.Complete();

        return dataForQueryWithConditions.AsList();
    }

    public async Task<List<WarrantyCardCustomerData>> GetWarrantyCardCustomerInfosByNameAsync(string keyword, WarrantyCardSearchRequest? warrantyCardSearchRequest = null)
    {
        const string KeywordParameterName = "@Keyword";

        string selectClause =
            $"""
            SELECT {CustomerNameColumn}, MIN({CustomerBIDColumn}) AS {CustomerBIDColumn}
            FROM {WarrantyCardsTableName}
            """;

        string startingWhereClause = $"WHERE {CustomerNameColumn} LIKE {KeywordParameterName} + '%'";

        string groupByClause = $"GROUP BY {CustomerNameColumn}";

        if (warrantyCardSearchRequest is null)
        {
            string fullQuery =
                $"""
                {selectClause}
                {startingWhereClause}
                {groupByClause}
                """;

            DynamicParameters parameters = new();

            parameters.Add(KeywordParameterName, keyword, DbType.String);

            using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

            using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

            IEnumerable<WarrantyCardCustomerData> data = await dbConnection.QueryAsync<WarrantyCardCustomerData>(
                fullQuery, parameters, commandType: CommandType.Text);

            transactionScope.Complete();

            return data.AsList();
        }

        (string whereClause, DynamicParameters parametersForQueryWithConditions) = GetQueryAndAssignParametersFromSearchData(warrantyCardSearchRequest, false);

        parametersForQueryWithConditions.Add(KeywordParameterName, keyword, DbType.String);

        if (!string.IsNullOrWhiteSpace(whereClause))
        {
            whereClause = "AND " + whereClause;
        }

        string fullQueryWithConditions =
            $"""
            {selectClause}
            {startingWhereClause}
            {whereClause}
            {groupByClause}
            """;

        using TransactionScope transactionScopeForQueryWithConditions = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnectionForQueryWithConditions = new(_connectionStringProvider.ConnectionString);

        IEnumerable<WarrantyCardCustomerData> dataForQueryWithConditions = await dbConnectionForQueryWithConditions.QueryAsync<WarrantyCardCustomerData>(
            fullQueryWithConditions, parametersForQueryWithConditions, commandType: CommandType.Text);

        transactionScopeForQueryWithConditions.Complete();

        return dataForQueryWithConditions.AsList();
    }

    public async Task<List<WarrantyCard>> GetWarrantyCardByOrderIdsAsync(List<int> warrantyCardOrderIds)
    {
        const string query =
            $"""
            {_selectWithItemsQueryBase}
            WHERE warrantyCards.{OrderIdColumn} IN @warrantyCardOrderIds;
            """;

        var parameters = new
        {
            warrantyCardOrderIds = warrantyCardOrderIds,
        };

        List<WarrantyCardDAO> output = new();

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        await dbConnection.QueryAsync<WarrantyCardDAO, WarrantyCardItemDAO, WarrantyCardDAO>(
            query,
            (warrantyCard, warrantyCardItem) =>
            {
                WarrantyCardDAO? existingWarrantyCard = output.FirstOrDefault(x => x.ExportId == warrantyCard.ExportId);

                if (existingWarrantyCard is null)
                {
                    warrantyCard.WarrantyCardItems = new();

                    output.Add(warrantyCard);

                    existingWarrantyCard = warrantyCard;
                }

                if (warrantyCardItem is not null)
                {
                    existingWarrantyCard.WarrantyCardItems!.Add(warrantyCardItem);
                }

                return existingWarrantyCard;
            },
            parameters,
            splitOn: $"{WarrantyCardItemsTable.ExportedItemIdColumn}",
            commandType: CommandType.Text);

        transactionScope.Complete();

        return MapRange(output);
    }

    public async Task<WarrantyCard?> GetWarrantyCardByOrderIdAsync(int warrantyCardOrderId)
    {
        const string query =
            $"""
            {_selectWithItemsQueryBase}
            WHERE warrantyCards.{OrderIdColumn} = @warrantyCardOrderId;
            """;

        var parameters = new
        {
            warrantyCardOrderId = warrantyCardOrderId,
        };

        WarrantyCardDAO? output = null;

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        await dbConnection.QueryAsync<WarrantyCardDAO, WarrantyCardItemDAO, WarrantyCardDAO>(
            query,
            (warrantyCard, warrantyCardItem) =>
            {
                if (output is null)
                {
                    output = warrantyCard;

                    output.WarrantyCardItems = new();
                }

                if (warrantyCardItem is not null)
                {
                    output.WarrantyCardItems!.Add(warrantyCardItem);
                }

                return output;
            },
            parameters,
            splitOn: $"{WarrantyCardItemsTable.ExportedItemIdColumn}",
            commandType: CommandType.Text);

        transactionScope.Complete();

        return (output is not null) ? Map(output) : null;
    }

    private static (string whereClause, DynamicParameters parameters) GetQueryAndAssignParametersFromSearchData(
        WarrantyCardSearchRequest warrantyCardSearchRequest,
        bool includeStartingWhereClause = true)
    {
        const string warrantyCardSearchIdParameterNameBase = "WarrantyCardByIdSearchKeyword";
        const string warrantyCardSearchKeywordParameterNameBase = "WarrantyCardByStringSearchKeyword";
        const string warrantyCardSearchFromDateParameterNameBase = "WarrantyCardByDateSearchFromDate";
        const string warrantyCardSearchToDateParameterNameBase = "WarrantyCardByDateSearchToDate";

        DynamicParameters parameters = new();

        List<string> whereStatements = new();

        if (warrantyCardSearchRequest.WarrantyCardByIdSearchRequests is not null)
        {
            List<string> whereStatementsInIdSearchRequests = new();

            for (int i = 0; i < warrantyCardSearchRequest.WarrantyCardByIdSearchRequests.Count; i++)
            {
                WarrantyCardByIdSearchRequest warrantyCardByIdSearchRequest = warrantyCardSearchRequest.WarrantyCardByIdSearchRequests[i];

                string parameterName = warrantyCardSearchIdParameterNameBase + i.ToString();

                parameters.Add(parameterName, warrantyCardByIdSearchRequest.Id, DbType.Int32);

                string whereStatement = GetWarrantyCardQueryWhereStatementForIdSearch(
                    warrantyCardByIdSearchRequest.SearchOption, parameterName);

                whereStatementsInIdSearchRequests.Add(whereStatement);
            }

            if (whereStatementsInIdSearchRequests.Count >= 0)
            {
                string whereStatementForIdSearchRequests = JoinConditionalStatements(
                    whereStatementsInIdSearchRequests, ConditionalStatementJoinType.Or);

                whereStatements.Add(whereStatementForIdSearchRequests);
            }
        }

        if (warrantyCardSearchRequest.WarrantyCardByStringSearchRequests is not null)
        {
            List<string> whereStatementsInStringSearchRequests = new();

            for (int i = 0; i < warrantyCardSearchRequest.WarrantyCardByStringSearchRequests.Count; i++)
            {
                WarrantyCardByStringSearchRequest warrantyCardByStringSearchRequest = warrantyCardSearchRequest.WarrantyCardByStringSearchRequests[i];

                string parameterName = warrantyCardSearchKeywordParameterNameBase + i.ToString();

                parameters.Add(parameterName, warrantyCardByStringSearchRequest.Keyword, DbType.String);

                string whereStatement = GetWarrantyCardQueryWhereStatementForKeywordSearch(
                    warrantyCardByStringSearchRequest.SearchOption, parameterName);

                whereStatementsInStringSearchRequests.Add(whereStatement);
            }

            if (whereStatementsInStringSearchRequests.Count >= 0)
            {
                string whereStatementForStringSearchRequests = JoinConditionalStatements(
                    whereStatementsInStringSearchRequests, ConditionalStatementJoinType.Or);

                whereStatements.Add(whereStatementForStringSearchRequests);
            }
        }

        if (warrantyCardSearchRequest.WarrantyCardByDateFilterRequests is not null)
        {
            List<string> whereStatementsInDateSearchRequests = new();

            for (int i = 0; i < warrantyCardSearchRequest.WarrantyCardByDateFilterRequests.Count; i++)
            {
                WarrantyCardByDateFilterRequest warrantyCardByDateSearchRequest = warrantyCardSearchRequest.WarrantyCardByDateFilterRequests[i];

                string? fromDateParameterName = null;
                string? toDateParameterName = null;

                if (warrantyCardByDateSearchRequest.FromDate is not null)
                {
                    fromDateParameterName = warrantyCardSearchFromDateParameterNameBase + i.ToString();

                    parameters.Add(fromDateParameterName, warrantyCardByDateSearchRequest.FromDate, DbType.DateTime);
                }

                if (warrantyCardByDateSearchRequest.ToDate is not null)
                {
                    toDateParameterName = warrantyCardSearchToDateParameterNameBase + i.ToString();

                    parameters.Add(toDateParameterName, warrantyCardByDateSearchRequest.ToDate, DbType.DateTime);
                }

                string? whereStatement = GetWarrantyCardQueryWhereStatementForDateSearch(
                    warrantyCardByDateSearchRequest.SearchOption, fromDateParameterName, toDateParameterName);

                if (whereStatement is null) continue;

                whereStatement = $"({whereStatement})";

                whereStatementsInDateSearchRequests.Add(whereStatement);
            }

            if (whereStatementsInDateSearchRequests.Count >= 0)
            {
                string whereStatementForDateSearchRequests = JoinConditionalStatements(
                    whereStatementsInDateSearchRequests, ConditionalStatementJoinType.And);

                whereStatements.Add(whereStatementForDateSearchRequests);
            }
        }

        if (whereStatements.Count == 0) return (string.Empty, parameters);

        string whereClause;

        if (includeStartingWhereClause)
        {
            whereClause = GetWhereClauseFromStatements(whereStatements, ConditionalStatementJoinType.And);
        }
        else
        {
            whereClause = JoinConditionalStatements(whereStatements, ConditionalStatementJoinType.And);
        }

        return (whereClause, parameters);
    }

    private static string GetWarrantyCardQueryWhereStatementForIdSearch(
       WarrantyCardByIdSearchOptions invoiceByIdSearchOptions,
       string idParameterName)
    {
        idParameterName = GetParameterNameWithStartingSymbol(idParameterName);

        string idToCheckInQuery = invoiceByIdSearchOptions switch
        {
            WarrantyCardByIdSearchOptions.ByExportId => ExportIdColumn,
            WarrantyCardByIdSearchOptions.ByExportUserId => ExportUserIdColumn,
            WarrantyCardByIdSearchOptions.ByOrderId => OrderIdColumn,
            WarrantyCardByIdSearchOptions.ByCustomerBID => CustomerBIDColumn,

            _ => throw new NotSupportedException($"Value {invoiceByIdSearchOptions} is not supported"),
        };

        return $"{idToCheckInQuery} = {idParameterName}\n";
    }

    private static string GetWarrantyCardQueryWhereStatementForKeywordSearch(
        WarrantyCardByStringSearchOptions warrantyCardByStringSearchOptions,
        string parameterName)
    {
        parameterName = GetParameterNameWithStartingSymbol(parameterName);

        return warrantyCardByStringSearchOptions switch
        {
            WarrantyCardByStringSearchOptions.ByExportUser => $"CHARINDEX({parameterName}, {ExportUserColumn}) > 0",
            WarrantyCardByStringSearchOptions.ByCustomerName => $"CHARINDEX({parameterName}, {CustomerNameColumn}) > 0",

            _ => throw new NotSupportedException($"Value {warrantyCardByStringSearchOptions} is not supported"),
        };
    }

    private static string? GetWarrantyCardQueryWhereStatementForDateSearch(
        WarrantyCardByDateSearchOptions warrantyCardByDateSearchOptions,
        string? fromDateParameterName = null,
        string? toDateParameterName = null)
    {
        if (string.IsNullOrWhiteSpace(fromDateParameterName) && string.IsNullOrWhiteSpace(toDateParameterName))
        {
            return null;
        }

        if (fromDateParameterName is not null)
        {
            fromDateParameterName = GetParameterNameWithStartingSymbol(fromDateParameterName);
        }

        if (toDateParameterName is not null)
        {
            toDateParameterName = GetParameterNameWithStartingSymbol(toDateParameterName);
        }

        string dateToCheckInQuery = warrantyCardByDateSearchOptions switch
        {
            WarrantyCardByDateSearchOptions.ByExportDate => ExportDateColumn,
            WarrantyCardByDateSearchOptions.ByWarrantyCardDate => WarrantyCardDateColumn,

            _ => throw new NotSupportedException($"Value {warrantyCardByDateSearchOptions} is not supported"),
        };

        StringBuilder getWarrantyCardsByDateWhereStatementBuilder = new();

        if (fromDateParameterName is not null)
        {
            getWarrantyCardsByDateWhereStatementBuilder.Append(
                $"({fromDateParameterName} IS NULL OR {dateToCheckInQuery} >= {fromDateParameterName})\n");
        }

        if (toDateParameterName is not null)
        {
            if (fromDateParameterName is not null)
            {
                getWarrantyCardsByDateWhereStatementBuilder.Append(" AND ");
            }

            getWarrantyCardsByDateWhereStatementBuilder.Append(
                $"({toDateParameterName} IS NULL OR {dateToCheckInQuery} <= {toDateParameterName})");
        }

        return getWarrantyCardsByDateWhereStatementBuilder.ToString();
    }
}