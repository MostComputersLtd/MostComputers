using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Documents.Configuration;
using MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.Services.DataAccess.Documents.Models.DAO;
using MOSTComputers.Services.DataAccess.Documents.Models.Requests.Invoice;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Transactions;

using static MOSTComputers.Services.DataAccess.Documents.Mapping.ResponseMapper;
using static MOSTComputers.Services.DataAccess.Documents.Utils.SqlGenerateUtils;
using static MOSTComputers.Services.DataAccess.Documents.Utils.TableAndColumnNameUtils;
using static MOSTComputers.Services.DataAccess.Documents.Utils.TableAndColumnNameUtils.InvoicesTable;

namespace MOSTComputers.Services.DataAccess.Documents.DataAccess;
internal class InvoiceRepository : IInvoiceRepository
{
    public InvoiceRepository([FromKeyedServices(ConfigureServices.DocumentsDataAccessServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    const string _selectWithItemsQueryBase =
        $"""
        SELECT invoices.{ExportIdColumn},
            {ExportDateColumn},
            {ExportUserIDColumn},
            {ExportUserColumn},
            invoices.{InvoiceIdColumn},
            {FirmIdColumn},
            {CustomerBIDColumn},
            {InvoiceDirectionColumn},
            {CustomerNameColumn},
            {MPersonColumn},
            {CustomerAddressColumn},
            {InvoiceDateColumn},
            {PDDCColumn},
            {UserNameColumn},
            {StatusColumn},
            {InvoiceNumberColumn},
            {PayTypeColumn},
            {RPersonColumn},
            {RDATEColumn},
            {BulstatColumn},
            {PratkaIdColumn},
            {InvTypeColumn},
            {InvBasisColumn},
            {RelatedInvNoColumn},
            {IsVATRegisteredColumn},
            {PrintedNETAmountColumn},
            {DueDateColumn},
            {CustomerBankNameColumn},
            {CustomerBankIBANColumn},
            {CustomerBankBICColumn},
            {PaymentStatusColumn},
            {PaymentStatusDateColumn},
            {PaymentStatusUserNameColumn},
            {InvoiceCurrencyColumn},

            invoiceItems.{InvoiceItemsTable.ExportedItemIdColumn},
            invoiceItems.{InvoiceItemsTable.ExportIdColumn} AS {InvoiceItemsTable.ExportIdAlias},
            invoiceItems.{InvoiceItemsTable.IEIDColumn},
            invoiceItems.{InvoiceItemsTable.InvoiceIdColumn} AS {InvoiceItemsTable.InvoiceIdAlias},
            invoiceItems.{InvoiceItemsTable.NameColumn},
            invoiceItems.{InvoiceItemsTable.PriceInLevaColumn},
            invoiceItems.{InvoiceItemsTable.QuantityColumn},
            invoiceItems.{InvoiceItemsTable.DisplayOrderColumn}
            
        FROM {InvoicesTableName} invoices WITH (NOLOCK)
        LEFT JOIN {InvoiceItemsTableName} invoiceItems WITH (NOLOCK)
        ON invoiceItems.{InvoiceItemsTable.InvoiceIdColumn} = invoices.{InvoiceIdColumn}
        """;

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<List<Invoice>> GetAllMatchingAsync(InvoiceSearchRequest invoiceSearchRequest)
    {
        (string whereClause, DynamicParameters parameters) = GetQueryAndAssignParametersFromSearchData(
            true,
            invoiceSearchRequest.InvoiceByIdSearchRequests,
            invoiceSearchRequest.InvoiceByStringSearchRequests,
            invoiceSearchRequest.InvoiceByDateFilterRequests);

        string query =
            $"""
            {_selectWithItemsQueryBase}
            {whereClause}
            """;

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        List<InvoiceDAO> output = new();

        await dbConnection.QueryAsync<InvoiceDAO, InvoiceItemDAO, InvoiceDAO>(
            query,
            (invoice, invoiceItem) =>
            {
                InvoiceDAO? existingInvoice = output.FirstOrDefault(x => x.InvoiceId == invoice.InvoiceId);

                if (existingInvoice is null)
                {
                    invoice.InvoiceItems = new();

                    output.Add(invoice);

                    existingInvoice = invoice;
                }

                if (invoiceItem is not null)
                {
                    existingInvoice.InvoiceItems!.Add(invoiceItem);
                }

                return existingInvoice;
            },
            parameters,
            splitOn: $"{InvoiceItemsTable.ExportedItemIdColumn}",
            commandType: CommandType.Text);

        transactionScope.Complete();

        return MapRange(output);
    }

    public async Task<List<InvoiceCustomerData>> GetInvoiceCustomerInfosAsync(InvoiceSearchRequest invoiceSearchRequest)
    {
        string selectClause =
            $"""
            SELECT {CustomerNameColumn}, MIN({CustomerBIDColumn}) AS {CustomerBIDColumn}
            FROM {InvoicesTableName}
            """;

        string groupByClause = $"GROUP BY {CustomerNameColumn}";

        (string whereClause, DynamicParameters parametersForQueryWithConditions) = GetQueryAndAssignParametersFromSearchData(
            true,
            invoiceSearchRequest?.InvoiceByIdSearchRequests,
            invoiceSearchRequest?.InvoiceByStringSearchRequests,
            invoiceSearchRequest?.InvoiceByDateFilterRequests);

        string fullQueryWithConditions =
            $"""
            {selectClause}
            {whereClause}
            {groupByClause}
            """;

        using TransactionScope transactionScopeForQueryWithConditions = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnectionForQueryWithConditions = new(_connectionStringProvider.ConnectionString);

        IEnumerable<InvoiceCustomerData> dataForQueryWithConditions = await dbConnectionForQueryWithConditions.QueryAsync<InvoiceCustomerData>(
            fullQueryWithConditions, parametersForQueryWithConditions, commandType: CommandType.Text);

        transactionScopeForQueryWithConditions.Complete();

        return dataForQueryWithConditions.AsList();
    }

    public async Task<List<InvoiceCustomerData>> GetInvoiceCustomerInfosByNameAsync(string keyword, InvoiceSearchRequest? invoiceSearchRequest = null)
    {
        const string KeywordParameterName = "@Keyword";

        string selectClause =
            $"""
            SELECT {CustomerNameColumn}, MIN({CustomerBIDColumn}) AS {CustomerBIDColumn}
            FROM {InvoicesTableName}
            """;

        string startingWhereClause = $"WHERE {CustomerNameColumn} LIKE {KeywordParameterName} + '%'";

        string groupByClause = $"GROUP BY {CustomerNameColumn}";

        if (invoiceSearchRequest is null)
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

            IEnumerable<InvoiceCustomerData> data = await dbConnection.QueryAsync<InvoiceCustomerData>(
                fullQuery, parameters, commandType: CommandType.Text);

            transactionScope.Complete();

            return data.AsList();
        }

        (string whereClause, DynamicParameters parametersForQueryWithConditions) = GetQueryAndAssignParametersFromSearchData(
            false,
            invoiceSearchRequest?.InvoiceByIdSearchRequests,
            invoiceSearchRequest?.InvoiceByStringSearchRequests,
            invoiceSearchRequest?.InvoiceByDateFilterRequests);

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

        IEnumerable<InvoiceCustomerData> dataForQueryWithConditions = await dbConnectionForQueryWithConditions.QueryAsync<InvoiceCustomerData>(
            fullQueryWithConditions, parametersForQueryWithConditions, commandType: CommandType.Text);

        transactionScopeForQueryWithConditions.Complete();

        return dataForQueryWithConditions.AsList();
    }

    public async Task<List<Invoice>> GetInvoicesByIdsAsync(List<int> invoiceIds)
    {
        invoiceIds = invoiceIds.Distinct().Where(x => x > 0).ToList();

        if (invoiceIds.Count == 0) return new();

        string query =
            $"""
            {_selectWithItemsQueryBase}
            WHERE invoices.{InvoiceIdColumn} IN @invoiceIds;
            """;

        var parameters = new
        {
            invoiceIds = invoiceIds,
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        List<InvoiceDAO> output = new();

        await dbConnection.QueryAsync<InvoiceDAO, InvoiceItemDAO, InvoiceDAO>(
            query,
            (invoice, invoiceItem) =>
            {
                InvoiceDAO? existingInvoice = output.FirstOrDefault(x => x.InvoiceId == invoice.InvoiceId);

                if (existingInvoice is null)
                {
                    invoice.InvoiceItems = new();

                    output.Add(invoice);

                    existingInvoice = invoice;
                }

                if (invoiceItem is not null)
                {
                    existingInvoice.InvoiceItems!.Add(invoiceItem);
                }

                return existingInvoice;
            },
            parameters,
            splitOn: $"{InvoiceItemsTable.ExportedItemIdColumn}",
            commandType: CommandType.Text);

        transactionScope.Complete();

        return MapRange(output);
    }

    public async Task<Invoice?> GetInvoiceByIdAsync(int invoiceId)
    {
        string query =
            $"""
            {_selectWithItemsQueryBase}
            WHERE invoices.{InvoiceIdColumn} = @invoiceId;
            """;

        var parameters = new
        {
            invoiceId = invoiceId,
        };

        InvoiceDAO? output = null;

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        await dbConnection.QueryAsync<InvoiceDAO, InvoiceItemDAO, InvoiceDAO>(
            query,
            (invoice, invoiceItem) =>
            {
                if (output is null)
                {
                    output = invoice;

                    output.InvoiceItems = new();
                }

                if (invoiceItem is not null)
                {
                    output.InvoiceItems!.Add(invoiceItem);
                }

                return output;
            },
            parameters,
            splitOn: $"{InvoiceItemsTable.ExportedItemIdColumn}",
            commandType: CommandType.Text);

        transactionScope.Complete();

        return (output is not null) ? Map(output) : null;
    }

    public async Task<Invoice?> GetInvoiceByNumberAsync(string invoiceNumber)
    {
        const string query =
            $"""
            {_selectWithItemsQueryBase}
            WHERE invoices.{InvoiceNumberColumn} = @invoiceNumber;
            """;

        var parameters = new
        {
            invoiceNumber = invoiceNumber,
        };

        InvoiceDAO? output = null;

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        await dbConnection.QueryAsync<InvoiceDAO, InvoiceItemDAO, InvoiceDAO>(
            query,
            (invoice, invoiceItem) =>
            {
                if (output is null)
                {
                    output = invoice;

                    output.InvoiceItems = new();
                }

                if (invoiceItem is not null)
                {
                    output.InvoiceItems!.Add(invoiceItem);
                }

                return output;
            },
            parameters,
            splitOn: $"{InvoiceItemsTable.ExportedItemIdColumn}",
            commandType: CommandType.Text);

        transactionScope.Complete();

        return (output is not null) ? Map(output) : null;
    }

    private static (string whereClause, DynamicParameters parameters) GetQueryAndAssignParametersFromSearchData(
        bool includeStartingWhereClause = true,
        List<InvoiceByIdSearchRequest>? invoiceByIdSearchRequests = null,
        List<InvoiceByStringSearchRequest>? invoiceByStringSearchRequests = null,
        List<InvoiceByDateFilterRequest>? invoiceByDateFilterRequests = null)
    {
        const string invoiceSearchIdParameterNameBase = "InvoiceByIdSearchKeyword";
        const string invoiceSearchKeywordParameterNameBase = "InvoiceByStringSearchKeyword";
        const string invoiceSearchFromDateParameterNameBase = "InvoiceByDateSearchFromDate";
        const string invoiceSearchToDateParameterNameBase = "InvoiceByDateSearchToDate";

        DynamicParameters parameters = new();
        List<string> whereStatements = new();

        if (invoiceByIdSearchRequests is not null)
        {
            List<string> whereStatementsInIdSearchRequests = new();

            for (int i = 0; i < invoiceByIdSearchRequests.Count; i++)
            {
                InvoiceByIdSearchRequest invoiceByIdSearchRequest = invoiceByIdSearchRequests[i];

                string parameterName = invoiceSearchIdParameterNameBase + i.ToString();

                parameters.Add(parameterName, invoiceByIdSearchRequest.Id, DbType.Int32);

                string whereStatement = GetInvoiceQueryWhereStatementForIdSearch(
                    invoiceByIdSearchRequest.SearchOption, parameterName);

                whereStatementsInIdSearchRequests.Add(whereStatement);
            }

            if (whereStatementsInIdSearchRequests.Count >= 0)
            {
                string whereStatementForIdSearchRequests = JoinConditionalStatements(
                    whereStatementsInIdSearchRequests, ConditionalStatementJoinType.Or);

                whereStatements.Add(whereStatementForIdSearchRequests);
            }
        }

        if (invoiceByStringSearchRequests is not null)
        {
            List<string> whereStatementsInStringSearchRequests = new();

            for (int i = 0; i < invoiceByStringSearchRequests.Count; i++)
            {
                InvoiceByStringSearchRequest invoiceByStringSearchRequest = invoiceByStringSearchRequests[i];

                string parameterName = invoiceSearchKeywordParameterNameBase + i.ToString();

                parameters.Add(parameterName, invoiceByStringSearchRequest.Keyword, DbType.String);

                string whereStatement = GetInvoiceQueryWhereStatementForKeywordSearch(
                    invoiceByStringSearchRequest.SearchOption, invoiceByStringSearchRequest.SearchType, parameterName);

                whereStatementsInStringSearchRequests.Add(whereStatement);
            }

            if (whereStatementsInStringSearchRequests.Count >= 0)
            {
                string whereStatementForStringSearchRequests = JoinConditionalStatements(
                    whereStatementsInStringSearchRequests, ConditionalStatementJoinType.Or);

                whereStatements.Add(whereStatementForStringSearchRequests);
            }
        }

        if (invoiceByDateFilterRequests is not null)
        {
            List<string> whereStatementsInDateSearchRequests = new();

            for (int i = 0; i < invoiceByDateFilterRequests.Count; i++)
            {
                InvoiceByDateFilterRequest invoiceByDateSearchRequest = invoiceByDateFilterRequests[i];

                string? fromDateParameterName = null;
                string? toDateParameterName = null;

                if (invoiceByDateSearchRequest.FromDate is not null)
                {
                    fromDateParameterName = invoiceSearchFromDateParameterNameBase + i.ToString();

                    parameters.Add(fromDateParameterName, invoiceByDateSearchRequest.FromDate, DbType.DateTime);
                }

                if (invoiceByDateSearchRequest.ToDate is not null)
                {
                    toDateParameterName = invoiceSearchToDateParameterNameBase + i.ToString();

                    parameters.Add(toDateParameterName, invoiceByDateSearchRequest.ToDate, DbType.DateTime);
                }

                string? whereStatement = GetInvoiceQueryWhereStatementForDateSearch(
                    invoiceByDateSearchRequest.SearchOption, fromDateParameterName, toDateParameterName);

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

    private static string GetInvoiceQueryWhereStatementForIdSearch(
        InvoiceByIdSearchOptions invoiceByIdSearchOptions,
        string idParameterName)
    {
        idParameterName = GetParameterNameWithStartingSymbol(idParameterName);

        string idToCheckInQuery = invoiceByIdSearchOptions switch
        {
            InvoiceByIdSearchOptions.ByExportId => ExportIdColumn,
            InvoiceByIdSearchOptions.ByExportUserId => ExportUserIDColumn,
            InvoiceByIdSearchOptions.ByFirmId => FirmIdColumn,
            InvoiceByIdSearchOptions.ByPratkaId => PratkaIdColumn,
            InvoiceByIdSearchOptions.ByCustomerBID => CustomerBIDColumn,

            _ => throw new NotSupportedException($"Value {invoiceByIdSearchOptions} is not supported"),
        };

        return $"{idToCheckInQuery} = {idParameterName}\n";
    }

    private static string GetInvoiceQueryWhereStatementForKeywordSearch(
        InvoiceByStringSearchOptions invoiceByStringSearchOptions,
        InvoiceByStringSearchType invoiceByStringSearchType,
        string parameterName)
    {
        parameterName = GetParameterNameWithStartingSymbol(parameterName);

        string columnNameToCompareAgainst = invoiceByStringSearchOptions switch
        {
            InvoiceByStringSearchOptions.ByExportUser => ExportUserColumn,
            InvoiceByStringSearchOptions.ByCustomerName => CustomerNameColumn,
            InvoiceByStringSearchOptions.ByMPerson => MPersonColumn,
            InvoiceByStringSearchOptions.ByCustomerAddress => CustomerAddressColumn,
            InvoiceByStringSearchOptions.ByInvoiceNumber => InvoiceNumberColumn,
            InvoiceByStringSearchOptions.ByRPerson => RPersonColumn,
            InvoiceByStringSearchOptions.ByBulstat => BulstatColumn,
            InvoiceByStringSearchOptions.ByRelatedInvNo => RelatedInvNoColumn,
            InvoiceByStringSearchOptions.ByCustomerBankNameAndId => CustomerBankNameColumn,
            InvoiceByStringSearchOptions.ByCustomerBankIBAN => CustomerBankIBANColumn,
            InvoiceByStringSearchOptions.ByCustomerBankBIC => CustomerBankBICColumn,
            InvoiceByStringSearchOptions.ByPaymentStatusUserName => PaymentStatusUserNameColumn,

            _ => throw new NotSupportedException($"Value {invoiceByStringSearchOptions} is not supported"),
        };

        return invoiceByStringSearchType switch
        {
            InvoiceByStringSearchType.DataContainsValue => $"CHARINDEX({parameterName}, {columnNameToCompareAgainst}) > 0",
            InvoiceByStringSearchType.DataExactlyMatchesValue => $"{parameterName} = {columnNameToCompareAgainst}",

            _ => throw new NotSupportedException($"Value {invoiceByStringSearchType} is not supported"),
        };
    }

    private static string? GetInvoiceQueryWhereStatementForDateSearch(
        InvoiceByDateSearchOptions invoiceByDateSearchOptions,
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

        string dateToCheckInQuery = invoiceByDateSearchOptions switch
        {
            InvoiceByDateSearchOptions.ByExportDate => ExportDateColumn,
            InvoiceByDateSearchOptions.ByInvoiceDate => InvoiceDateColumn,
            InvoiceByDateSearchOptions.ByRDATE => RDATEColumn,
            InvoiceByDateSearchOptions.ByDueDate => DueDateColumn,
            InvoiceByDateSearchOptions.ByPaymentStatusDate => PaymentStatusDateColumn,
            
            _ => throw new NotSupportedException($"Value {invoiceByDateSearchOptions} is not supported"),
        };

        StringBuilder getInvoicesByDateWhereStatementBuilder = new();

        if (fromDateParameterName is not null)
        {
            getInvoicesByDateWhereStatementBuilder.Append(
                $"({fromDateParameterName} IS NULL OR {dateToCheckInQuery} >= {fromDateParameterName})\n");
        }

        if (toDateParameterName is not null)
        {
            if (fromDateParameterName is not null)
            {
                getInvoicesByDateWhereStatementBuilder.Append(" AND ");
            }

            getInvoicesByDateWhereStatementBuilder.Append(
                $"({toDateParameterName} IS NULL OR {dateToCheckInQuery} <= {toDateParameterName})");
        }

        return getInvoicesByDateWhereStatementBuilder.ToString();
    }
}