namespace MOSTComputers.Services.DataAccess.Products.Utils;
internal static class QueryUtils
{
    internal const int MaxParametersInCollectionForOneQuery = 2000;

    public static List<TOutputChunk> ExecuteQueryWithParametersInChunks<TParameterInList, TOutputChunk>(
        Func<List<TParameterInList>, TOutputChunk> query,
        List<TParameterInList> parametersInList)
    {
        List<TOutputChunk> outputChunks = new();

        if (parametersInList.Count <= MaxParametersInCollectionForOneQuery)
        {
            TOutputChunk outputChunk = query(parametersInList);

            outputChunks.Add(outputChunk);

            return outputChunks;
        }

        for (int i = 0; i < parametersInList.Count; i += MaxParametersInCollectionForOneQuery)
        {
            int remainingParameters = Math.Min(MaxParametersInCollectionForOneQuery, parametersInList.Count - i);

            List<TParameterInList> parameterChunk = parametersInList.GetRange(i, remainingParameters);

            TOutputChunk outputChunk = query(parameterChunk);

            outputChunks.Add(outputChunk);
        }

        return outputChunks;
    }

    public static async Task<List<TOutputChunk>> ExecuteQueryWithParametersInChunksAsync<TParameterInList, TOutputChunk>(
        Func<List<TParameterInList>, Task<TOutputChunk>> query,
        List<TParameterInList> parametersInList)
    {
        List<TOutputChunk> outputChunks = new();

        if (parametersInList.Count <= MaxParametersInCollectionForOneQuery)
        {
            TOutputChunk outputChunk = await query(parametersInList);

            outputChunks.Add(outputChunk);

            return outputChunks;
        }

        for (int i = 0; i < parametersInList.Count; i += MaxParametersInCollectionForOneQuery)
        {
            int remainingParameters = Math.Min(MaxParametersInCollectionForOneQuery, parametersInList.Count - i);

            List<TParameterInList> parameterChunk = parametersInList.GetRange(i, remainingParameters);

            TOutputChunk outputChunk = await query(parameterChunk);

            outputChunks.Add(outputChunk);
        }

        return outputChunks;
    }

    public static List<TOutput> ExecuteListQueryWithParametersInChunks<TParameterInList, TOutput>(
        Func<List<TParameterInList>, IEnumerable<TOutput>> query,
        List<TParameterInList> parametersInList)
    {
        List<TOutput> outputChunks = new();
                
        if (parametersInList.Count <= MaxParametersInCollectionForOneQuery)
        {
            IEnumerable<TOutput> outputChunk = query(parametersInList);

            outputChunks.AddRange(outputChunk);

            return outputChunks;
        }

        for (int i = 0; i < parametersInList.Count; i += MaxParametersInCollectionForOneQuery)
        {
            int remainingParameters = Math.Min(MaxParametersInCollectionForOneQuery, parametersInList.Count - i);

            List<TParameterInList> parameterChunk = parametersInList.GetRange(i, remainingParameters);

            IEnumerable<TOutput> outputChunk = query(parameterChunk);

            outputChunks.AddRange(outputChunk);
        }

        return outputChunks;
    }

    public static async Task<List<TOutput>> ExecuteListQueryWithParametersInChunksAsync<TParameterInList, TOutput>(
        Func<List<TParameterInList>, Task<IEnumerable<TOutput>>> query,
        List<TParameterInList> parametersInList)
    {
        List<TOutput> outputChunks = new();

        if (parametersInList.Count <= MaxParametersInCollectionForOneQuery)
        {
            IEnumerable<TOutput> outputChunk = await query(parametersInList);

            outputChunks.AddRange(outputChunk);

            return outputChunks;
        }

        for (int i = 0; i < parametersInList.Count; i += MaxParametersInCollectionForOneQuery)
        {
            int remainingParameters = Math.Min(MaxParametersInCollectionForOneQuery, parametersInList.Count - i);

            List<TParameterInList> parameterChunk = parametersInList.GetRange(i, remainingParameters);

            IEnumerable<TOutput> outputChunk = await query(parameterChunk);

            outputChunks.AddRange(outputChunk);
        }

        return outputChunks;
    }

    public static List<TOutput> SelectAsList<TInput, TOutput>(
        this IEnumerable<TInput> inputs,
        Func<TInput, TOutput> outputFunc)
    {
        List<TOutput> outputs = new();

        foreach (TInput input in inputs)
        {
            TOutput output = outputFunc(input);

            outputs.Add(output);
        }

        return outputs;
    }

    public static List<TOutput> SelectManyAsList<TInput, TInputEnumerable, TOutput>(
        this IEnumerable<TInputEnumerable> inputs,
        Func<TInput, TOutput> outputFunc)
        where TInputEnumerable : IEnumerable<TInput>
    {
        List<TOutput> outputs = new();

        foreach (TInputEnumerable inputPart in inputs)
        {
            foreach (TInput input in inputPart)
            {
                TOutput output = outputFunc(input);

                outputs.Add(output);
            }
        }

        return outputs;
    }
}