using System.Text;

namespace MOSTComputers.Services.DataAccess.Documents.Utils;
internal static class SqlGenerateUtils
{
    private const char _sqlParameterStartingSymbol = '@';

    internal enum ConditionalStatementJoinType
    {
        And,
        Or
    }

    internal static string GetWhereClauseFromStatements(IEnumerable<string> whereStatements, ConditionalStatementJoinType whereStatementType)
    {
        if (!whereStatements.Any())
        {
            return string.Empty;
        }

        StringBuilder whereClauseBuilder = new("WHERE ");

        string statementsAsString = JoinConditionalStatements(whereStatements, whereStatementType);

        whereClauseBuilder.Append(statementsAsString);

        return whereClauseBuilder.ToString();
    }

    internal static string JoinConditionalStatements(IEnumerable<string> whereStatements, ConditionalStatementJoinType whereStatementType)
    {
        string joinKeyword = whereStatementType switch
        {
            ConditionalStatementJoinType.And => "AND",
            ConditionalStatementJoinType.Or => "OR",

            _ => throw new NotSupportedException($"Value {whereStatementType} is not supported", null)
        };

        string joinKeywordWithSpacing = $" {joinKeyword} ";

        return string.Join(joinKeywordWithSpacing, whereStatements);
    }

    internal static string GetParameterNameWithoutStartingSymbol(string parameterName)
    {
        if (parameterName.StartsWith(_sqlParameterStartingSymbol))
        {
            return parameterName[1..];
        }

        return parameterName;
    }

    internal static string GetParameterNameWithStartingSymbol(string parameterName)
    {
        if (parameterName.StartsWith(_sqlParameterStartingSymbol))
        {
            return parameterName;
        }

        return _sqlParameterStartingSymbol + parameterName;
    }
}