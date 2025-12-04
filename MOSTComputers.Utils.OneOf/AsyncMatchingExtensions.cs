using OneOf;

namespace MOSTComputers.Utils.OneOf;
public static class AsyncMatchingExtensions
{
    #region 2 types

    public static async Task<TResult> MatchAsync<T1, T2, TResult>(
        this OneOf<T1, T2> oneOf, Func<T1, Task<TResult>> f0, Func<T2, TResult> f1)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return await f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return f1(oneOf.AsT1);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, TResult>(this OneOf<T1, T2> oneOf, Func<T1, TResult> f0, Func<T2, Task<TResult>> f1)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return await f1(oneOf.AsT1);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, TResult>(this OneOf<T1, T2> oneOf, Func<T1, Task<TResult>> f0, Func<T2, Task<TResult>> f1)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return await f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return await f1(oneOf.AsT1);
        }

        throw new InvalidOperationException();
    }

    #endregion 2 types

    #region 3 types

    public static async Task<TResult> MatchAsync<T1, T2, T3, TResult>(this OneOf<T1, T2, T3> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, TResult> f1, Func<T3, TResult> f2)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return await f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return f2(oneOf.AsT2);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, TResult>(this OneOf<T1, T2, T3> oneOf,
        Func<T1, TResult> f0, Func<T2, Task<TResult>> f1, Func<T3, TResult> f2)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return await f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return f2(oneOf.AsT2);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, TResult>(this OneOf<T1, T2, T3> oneOf,
        Func<T1, TResult> f0, Func<T2, TResult> f1, Func<T3, Task<TResult>> f2)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return await f2(oneOf.AsT2);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, TResult>(this OneOf<T1, T2, T3> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, Task<TResult>> f1, Func<T3, TResult> f2)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return await f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return await f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return f2(oneOf.AsT2);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, TResult>(this OneOf<T1, T2, T3> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, TResult> f1, Func<T3, Task<TResult>> f2)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return await f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return await f2(oneOf.AsT2);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, TResult>(this OneOf<T1, T2, T3> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, Task<TResult>> f1, Func<T3, Task<TResult>> f2)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return await f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return await f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return await f2(oneOf.AsT2);
        }

        throw new InvalidOperationException();
    }

    #endregion 3 types

    #region 4 types

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, TResult>(this OneOf<T1, T2, T3, T4> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, TResult> f1, Func<T3, TResult> f2, Func<T4, TResult> f3)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return await f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return f3(oneOf.AsT3);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, TResult>(this OneOf<T1, T2, T3, T4> oneOf,
        Func<T1, TResult> f0, Func<T2, Task<TResult>> f1, Func<T3, TResult> f2, Func<T4, TResult> f3)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return await f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return f3(oneOf.AsT3);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, TResult>(this OneOf<T1, T2, T3, T4> oneOf,
        Func<T1, TResult> f0, Func<T2, TResult> f1, Func<T3, Task<TResult>> f2, Func<T4, TResult> f3)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return await f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return f3(oneOf.AsT3);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, TResult>(this OneOf<T1, T2, T3, T4> oneOf,
        Func<T1, TResult> f0, Func<T2, TResult> f1, Func<T3, TResult> f2, Func<T4, Task<TResult>> f3)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return await f3(oneOf.AsT3);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, TResult>(this OneOf<T1, T2, T3, T4> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, Task<TResult>> f1, Func<T3, TResult> f2, Func<T4, TResult> f3)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return await f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return await f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return f3(oneOf.AsT3);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, TResult>(this OneOf<T1, T2, T3, T4> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, TResult> f1, Func<T3, Task<TResult>> f2, Func<T4, TResult> f3)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return await f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return await f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return f3(oneOf.AsT3);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, TResult>(this OneOf<T1, T2, T3, T4> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, TResult> f1, Func<T3, TResult> f2, Func<T4, Task<TResult>> f3)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return await f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return await f3(oneOf.AsT3);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, TResult>(this OneOf<T1, T2, T3, T4> oneOf,
        Func<T1, TResult> f0, Func<T2, Task<TResult>> f1, Func<T3, Task<TResult>> f2, Func<T4, TResult> f3)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return await f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return await f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return f3(oneOf.AsT3);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, TResult>(this OneOf<T1, T2, T3, T4> oneOf,
        Func<T1, TResult> f0, Func<T2, Task<TResult>> f1, Func<T3, TResult> f2, Func<T4, Task<TResult>> f3)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return await f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return await f3(oneOf.AsT3);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, TResult>(this OneOf<T1, T2, T3, T4> oneOf,
        Func<T1, TResult> f0, Func<T2, TResult> f1, Func<T3, Task<TResult>> f2, Func<T4, Task<TResult>> f3)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return await f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return await f3(oneOf.AsT3);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, TResult>(this OneOf<T1, T2, T3, T4> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, Task<TResult>> f1, Func<T3, Task<TResult>> f2, Func<T4, TResult> f3)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return await f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return await f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return await f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return f3(oneOf.AsT3);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, TResult>(this OneOf<T1, T2, T3, T4> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, Task<TResult>> f1, Func<T3, TResult> f2, Func<T4, Task<TResult>> f3)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return await f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return await f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return await f3(oneOf.AsT3);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, TResult>(this OneOf<T1, T2, T3, T4> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, TResult> f1, Func<T3, Task<TResult>> f2, Func<T4, Task<TResult>> f3)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return await f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return await f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return await f3(oneOf.AsT3);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, TResult>(this OneOf<T1, T2, T3, T4> oneOf,
        Func<T1, TResult> f0, Func<T2, Task<TResult>> f1, Func<T3, Task<TResult>> f2, Func<T4, Task<TResult>> f3)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return await f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return await f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return await f3(oneOf.AsT3);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, TResult>(this OneOf<T1, T2, T3, T4> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, Task<TResult>> f1, Func<T3, Task<TResult>> f2, Func<T4, Task<TResult>> f3)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return await f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return await f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return await f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return await f3(oneOf.AsT3);
        }

        throw new InvalidOperationException();
    }

    #endregion 4 types

    #region 5 types

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(this OneOf<T1, T2, T3, T4, T5> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, TResult> f1, Func<T3, TResult> f2, Func<T4, TResult> f3, Func<T5, TResult> f4)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return await f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return f3(oneOf.AsT3);
        }

        if (oneOf.Index == 4 && f4 != null)
        {
            return f4(oneOf.AsT4);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(this OneOf<T1, T2, T3, T4, T5> oneOf,
        Func<T1, TResult> f0, Func<T2, Task<TResult>> f1, Func<T3, TResult> f2, Func<T4, TResult> f3, Func<T5, TResult> f4)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return await f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return f3(oneOf.AsT3);
        }

        if (oneOf.Index == 4 && f4 != null)
        {
            return f4(oneOf.AsT4);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(this OneOf<T1, T2, T3, T4, T5> oneOf,
        Func<T1, TResult> f0, Func<T2, TResult> f1, Func<T3, Task<TResult>> f2, Func<T4, TResult> f3, Func<T5, TResult> f4)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return await f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return f3(oneOf.AsT3);
        }

        if (oneOf.Index == 4 && f4 != null)
        {
            return f4(oneOf.AsT4);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(this OneOf<T1, T2, T3, T4, T5> oneOf,
        Func<T1, TResult> f0, Func<T2, TResult> f1, Func<T3, TResult> f2, Func<T4, Task<TResult>> f3, Func<T5, TResult> f4)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return await f3(oneOf.AsT3);
        }

        if (oneOf.Index == 4 && f4 != null)
        {
            return f4(oneOf.AsT4);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(this OneOf<T1, T2, T3, T4, T5> oneOf,
        Func<T1, TResult> f0, Func<T2, TResult> f1, Func<T3, TResult> f2, Func<T4, TResult> f3, Func<T5, Task<TResult>> f4)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return f3(oneOf.AsT3);
        }

        if (oneOf.Index == 4 && f4 != null)
        {
            return await f4(oneOf.AsT4);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(this OneOf<T1, T2, T3, T4, T5> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, Task<TResult>> f1, Func<T3, TResult> f2, Func<T4, TResult> f3, Func<T5, TResult> f4)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return await f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return await f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return f3(oneOf.AsT3);
        }

        if (oneOf.Index == 4 && f4 != null)
        {
            return f4(oneOf.AsT4);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(this OneOf<T1, T2, T3, T4, T5> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, TResult> f1, Func<T3, Task<TResult>> f2, Func<T4, TResult> f3, Func<T5, TResult> f4)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return await f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return await f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return f3(oneOf.AsT3);
        }

        if (oneOf.Index == 4 && f4 != null)
        {
            return f4(oneOf.AsT4);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(this OneOf<T1, T2, T3, T4, T5> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, TResult> f1, Func<T3, TResult> f2, Func<T4, Task<TResult>> f3, Func<T5, TResult> f4)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return await f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return await f3(oneOf.AsT3);
        }

        if (oneOf.Index == 4 && f4 != null)
        {
            return f4(oneOf.AsT4);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(this OneOf<T1, T2, T3, T4, T5> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, TResult> f1, Func<T3, TResult> f2, Func<T4, TResult> f3, Func<T5, Task<TResult>> f4)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return await f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return f3(oneOf.AsT3);
        }

        if (oneOf.Index == 4 && f4 != null)
        {
            return await f4(oneOf.AsT4);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(this OneOf<T1, T2, T3, T4, T5> oneOf,
        Func<T1, TResult> f0, Func<T2, Task<TResult>> f1, Func<T3, Task<TResult>> f2, Func<T4, TResult> f3, Func<T5, TResult> f4)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return await f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return await f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return f3(oneOf.AsT3);
        }

        if (oneOf.Index == 4 && f4 != null)
        {
            return f4(oneOf.AsT4);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(this OneOf<T1, T2, T3, T4, T5> oneOf,
        Func<T1, TResult> f0, Func<T2, Task<TResult>> f1, Func<T3, TResult> f2, Func<T4, Task<TResult>> f3, Func<T5, TResult> f4)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return await f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return await f3(oneOf.AsT3);
        }

        if (oneOf.Index == 4 && f4 != null)
        {
            return f4(oneOf.AsT4);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(this OneOf<T1, T2, T3, T4, T5> oneOf,
        Func<T1, TResult> f0, Func<T2, Task<TResult>> f1, Func<T3, TResult> f2, Func<T4, TResult> f3, Func<T5, Task<TResult>> f4)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return await f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return f3(oneOf.AsT3);
        }

        if (oneOf.Index == 4 && f4 != null)
        {
            return await f4(oneOf.AsT4);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(this OneOf<T1, T2, T3, T4, T5> oneOf,
        Func<T1, TResult> f0, Func<T2, TResult> f1, Func<T3, Task<TResult>> f2, Func<T4, Task<TResult>> f3, Func<T5, TResult> f4)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return await f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return await f3(oneOf.AsT3);
        }

        if (oneOf.Index == 4 && f4 != null)
        {
            return f4(oneOf.AsT4);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(this OneOf<T1, T2, T3, T4, T5> oneOf,
        Func<T1, TResult> f0, Func<T2, TResult> f1, Func<T3, Task<TResult>> f2, Func<T4, TResult> f3, Func<T5, Task<TResult>> f4)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return await f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return f3(oneOf.AsT3);
        }

        if (oneOf.Index == 4 && f4 != null)
        {
            return await f4(oneOf.AsT4);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(this OneOf<T1, T2, T3, T4, T5> oneOf,
        Func<T1, TResult> f0, Func<T2, TResult> f1, Func<T3, TResult> f2, Func<T4, Task<TResult>> f3, Func<T5, Task<TResult>> f4)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return await f3(oneOf.AsT3);
        }

        if (oneOf.Index == 4 && f4 != null)
        {
            return await f4(oneOf.AsT4);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(this OneOf<T1, T2, T3, T4, T5> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, Task<TResult>> f1, Func<T3, Task<TResult>> f2, Func<T4, TResult> f3, Func<T5, TResult> f4)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return await f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return await f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return await f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return f3(oneOf.AsT3);
        }

        if (oneOf.Index == 4 && f4 != null)
        {
            return f4(oneOf.AsT4);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(this OneOf<T1, T2, T3, T4, T5> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, Task<TResult>> f1, Func<T3, TResult> f2, Func<T4, Task<TResult>> f3, Func<T5, TResult> f4)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return await f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return await f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return await f3(oneOf.AsT3);
        }

        if (oneOf.Index == 4 && f4 != null)
        {
            return f4(oneOf.AsT4);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(this OneOf<T1, T2, T3, T4, T5> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, Task<TResult>> f1, Func<T3, TResult> f2, Func<T4, TResult> f3, Func<T5, Task<TResult>> f4)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return await f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return await f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return f3(oneOf.AsT3);
        }

        if (oneOf.Index == 4 && f4 != null)
        {
            return await f4(oneOf.AsT4);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(this OneOf<T1, T2, T3, T4, T5> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, TResult> f1, Func<T3, Task<TResult>> f2, Func<T4, Task<TResult>> f3, Func<T5, TResult> f4)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return await f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return await f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return await f3(oneOf.AsT3);
        }

        if (oneOf.Index == 4 && f4 != null)
        {
            return f4(oneOf.AsT4);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(this OneOf<T1, T2, T3, T4, T5> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, TResult> f1, Func<T3, Task<TResult>> f2, Func<T4, TResult> f3, Func<T5, Task<TResult>> f4)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return await f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return await f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return f3(oneOf.AsT3);
        }

        if (oneOf.Index == 4 && f4 != null)
        {
            return await f4(oneOf.AsT4);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(this OneOf<T1, T2, T3, T4, T5> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, TResult> f1, Func<T3, TResult> f2, Func<T4, Task<TResult>> f3, Func<T5, Task<TResult>> f4)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return await f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return await f3(oneOf.AsT3);
        }

        if (oneOf.Index == 4 && f4 != null)
        {
            return await f4(oneOf.AsT4);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(this OneOf<T1, T2, T3, T4, T5> oneOf,
        Func<T1, TResult> f0, Func<T2, Task<TResult>> f1, Func<T3, Task<TResult>> f2, Func<T4, Task<TResult>> f3, Func<T5, TResult> f4)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return await f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return await f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return await f3(oneOf.AsT3);
        }

        if (oneOf.Index == 4 && f4 != null)
        {
            return f4(oneOf.AsT4);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(this OneOf<T1, T2, T3, T4, T5> oneOf,
        Func<T1, TResult> f0, Func<T2, Task<TResult>> f1, Func<T3, Task<TResult>> f2, Func<T4, TResult> f3, Func<T5, Task<TResult>> f4)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return await f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return await f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return f3(oneOf.AsT3);
        }

        if (oneOf.Index == 4 && f4 != null)
        {
            return await f4(oneOf.AsT4);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(this OneOf<T1, T2, T3, T4, T5> oneOf,
        Func<T1, TResult> f0, Func<T2, Task<TResult>> f1, Func<T3, TResult> f2, Func<T4, Task<TResult>> f3, Func<T5, Task<TResult>> f4)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return await f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return await f3(oneOf.AsT3);
        }

        if (oneOf.Index == 4 && f4 != null)
        {
            return await f4(oneOf.AsT4);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(this OneOf<T1, T2, T3, T4, T5> oneOf,
        Func<T1, TResult> f0, Func<T2, TResult> f1, Func<T3, Task<TResult>> f2, Func<T4, Task<TResult>> f3, Func<T5, Task<TResult>> f4)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return await f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return await f3(oneOf.AsT3);
        }

        if (oneOf.Index == 4 && f4 != null)
        {
            return await f4(oneOf.AsT4);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(this OneOf<T1, T2, T3, T4, T5> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, Task<TResult>> f1, Func<T3, Task<TResult>> f2, Func<T4, Task<TResult>> f3, Func<T5, TResult> f4)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return await f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return await f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return await f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return await f3(oneOf.AsT3);
        }

        if (oneOf.Index == 4 && f4 != null)
        {
            return f4(oneOf.AsT4);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(this OneOf<T1, T2, T3, T4, T5> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, Task<TResult>> f1, Func<T3, Task<TResult>> f2, Func<T4, TResult> f3, Func<T5, Task<TResult>> f4)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return await f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return await f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return await f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return f3(oneOf.AsT3);
        }

        if (oneOf.Index == 4 && f4 != null)
        {
            return await f4(oneOf.AsT4);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(this OneOf<T1, T2, T3, T4, T5> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, Task<TResult>> f1, Func<T3, TResult> f2, Func<T4, Task<TResult>> f3, Func<T5, Task<TResult>> f4)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return await f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return await f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return await f3(oneOf.AsT3);
        }

        if (oneOf.Index == 4 && f4 != null)
        {
            return await f4(oneOf.AsT4);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(this OneOf<T1, T2, T3, T4, T5> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, TResult> f1, Func<T3, Task<TResult>> f2, Func<T4, Task<TResult>> f3, Func<T5, Task<TResult>> f4)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return await f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return await f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return await f3(oneOf.AsT3);
        }

        if (oneOf.Index == 4 && f4 != null)
        {
            return await f4(oneOf.AsT4);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(this OneOf<T1, T2, T3, T4, T5> oneOf,
        Func<T1, TResult> f0, Func<T2, Task<TResult>> f1, Func<T3, Task<TResult>> f2, Func<T4, Task<TResult>> f3, Func<T5, Task<TResult>> f4)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return await f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return await f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return await f3(oneOf.AsT3);
        }

        if (oneOf.Index == 4 && f4 != null)
        {
            return await f4(oneOf.AsT4);
        }

        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(this OneOf<T1, T2, T3, T4, T5> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, Task<TResult>> f1, Func<T3, Task<TResult>> f2, Func<T4, Task<TResult>> f3, Func<T5, Task<TResult>> f4)
    {
        if (oneOf.Index == 0 && f0 != null)
        {
            return await f0(oneOf.AsT0);
        }

        if (oneOf.Index == 1 && f1 != null)
        {
            return await f1(oneOf.AsT1);
        }

        if (oneOf.Index == 2 && f2 != null)
        {
            return await f2(oneOf.AsT2);
        }

        if (oneOf.Index == 3 && f3 != null)
        {
            return await f3(oneOf.AsT3);
        }

        if (oneOf.Index == 4 && f4 != null)
        {
            return await f4(oneOf.AsT4);
        }

        throw new InvalidOperationException();
    }

    #endregion 5 types

    #region 6 types

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, TResult> f1, Func<T3, TResult> f2, Func<T4, TResult> f3, Func<T5, TResult> f4, Func<T6, TResult> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return await f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, TResult> f0, Func<T2, Task<TResult>> f1, Func<T3, TResult> f2, Func<T4, TResult> f3, Func<T5, TResult> f4, Func<T6, TResult> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return await f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, Task<TResult>> f1, Func<T3, TResult> f2, Func<T4, TResult> f3, Func<T5, TResult> f4, Func<T6, TResult> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return await f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return await f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, TResult> f0, Func<T2, TResult> f1, Func<T3, Task<TResult>> f2, Func<T4, TResult> f3, Func<T5, TResult> f4, Func<T6, TResult> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return await f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, TResult> f1, Func<T3, Task<TResult>> f2, Func<T4, TResult> f3, Func<T5, TResult> f4, Func<T6, TResult> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return await f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return await f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, TResult> f0, Func<T2, Task<TResult>> f1, Func<T3, Task<TResult>> f2, Func<T4, TResult> f3, Func<T5, TResult> f4, Func<T6, TResult> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return await f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return await f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, Task<TResult>> f1, Func<T3, Task<TResult>> f2, Func<T4, TResult> f3, Func<T5, TResult> f4, Func<T6, TResult> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return await f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return await f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return await f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, TResult> f0, Func<T2, TResult> f1, Func<T3, TResult> f2, Func<T4, Task<TResult>> f3, Func<T5, TResult> f4, Func<T6, TResult> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return await f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, TResult> f1, Func<T3, TResult> f2, Func<T4, Task<TResult>> f3, Func<T5, TResult> f4, Func<T6, TResult> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return await f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return await f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, TResult> f0, Func<T2, Task<TResult>> f1, Func<T3, TResult> f2, Func<T4, Task<TResult>> f3, Func<T5, TResult> f4, Func<T6, TResult> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return await f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return await f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, Task<TResult>> f1, Func<T3, TResult> f2, Func<T4, Task<TResult>> f3, Func<T5, TResult> f4, Func<T6, TResult> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return await f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return await f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return await f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, TResult> f0, Func<T2, TResult> f1, Func<T3, Task<TResult>> f2, Func<T4, Task<TResult>> f3, Func<T5, TResult> f4, Func<T6, TResult> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return await f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return await f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, TResult> f1, Func<T3, Task<TResult>> f2, Func<T4, Task<TResult>> f3, Func<T5, TResult> f4, Func<T6, TResult> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return await f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return await f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return await f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, TResult> f0, Func<T2, Task<TResult>> f1, Func<T3, Task<TResult>> f2, Func<T4, Task<TResult>> f3, Func<T5, TResult> f4, Func<T6, TResult> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return await f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return await f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return await f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, Task<TResult>> f1, Func<T3, Task<TResult>> f2, Func<T4, Task<TResult>> f3, Func<T5, TResult> f4, Func<T6, TResult> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return await f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return await f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return await f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return await f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, TResult> f0, Func<T2, TResult> f1, Func<T3, TResult> f2, Func<T4, TResult> f3, Func<T5, Task<TResult>> f4, Func<T6, TResult> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return await f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, TResult> f1, Func<T3, TResult> f2, Func<T4, TResult> f3, Func<T5, Task<TResult>> f4, Func<T6, TResult> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return await f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return await f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, TResult> f0, Func<T2, Task<TResult>> f1, Func<T3, TResult> f2, Func<T4, TResult> f3, Func<T5, Task<TResult>> f4, Func<T6, TResult> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return await f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return await f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, Task<TResult>> f1, Func<T3, TResult> f2, Func<T4, TResult> f3, Func<T5, Task<TResult>> f4, Func<T6, TResult> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return await f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return await f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return await f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, TResult> f0, Func<T2, TResult> f1, Func<T3, Task<TResult>> f2, Func<T4, TResult> f3, Func<T5, Task<TResult>> f4, Func<T6, TResult> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return await f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return await f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, TResult> f1, Func<T3, Task<TResult>> f2, Func<T4, TResult> f3, Func<T5, Task<TResult>> f4, Func<T6, TResult> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return await f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return await f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return await f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, TResult> f0, Func<T2, Task<TResult>> f1, Func<T3, Task<TResult>> f2, Func<T4, TResult> f3, Func<T5, Task<TResult>> f4, Func<T6, TResult> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return await f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return await f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return await f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, Task<TResult>> f1, Func<T3, Task<TResult>> f2, Func<T4, TResult> f3, Func<T5, Task<TResult>> f4, Func<T6, TResult> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return await f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return await f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return await f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return await f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, TResult> f0, Func<T2, TResult> f1, Func<T3, TResult> f2, Func<T4, Task<TResult>> f3, Func<T5, Task<TResult>> f4, Func<T6, TResult> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return await f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return await f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, TResult> f1, Func<T3, TResult> f2, Func<T4, Task<TResult>> f3, Func<T5, Task<TResult>> f4, Func<T6, TResult> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return await f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return await f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return await f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, TResult> f0, Func<T2, Task<TResult>> f1, Func<T3, TResult> f2, Func<T4, Task<TResult>> f3, Func<T5, Task<TResult>> f4, Func<T6, TResult> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return await f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return await f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return await f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, Task<TResult>> f1, Func<T3, TResult> f2, Func<T4, Task<TResult>> f3, Func<T5, Task<TResult>> f4, Func<T6, TResult> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return await f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return await f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return await f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return await f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, TResult> f0, Func<T2, TResult> f1, Func<T3, Task<TResult>> f2, Func<T4, Task<TResult>> f3, Func<T5, Task<TResult>> f4, Func<T6, TResult> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return await f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return await f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return await f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, TResult> f1, Func<T3, Task<TResult>> f2, Func<T4, Task<TResult>> f3, Func<T5, Task<TResult>> f4, Func<T6, TResult> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return await f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return await f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return await f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return await f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, TResult> f0, Func<T2, Task<TResult>> f1, Func<T3, Task<TResult>> f2, Func<T4, Task<TResult>> f3, Func<T5, Task<TResult>> f4, Func<T6, TResult> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return await f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return await f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return await f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return await f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, Task<TResult>> f1, Func<T3, Task<TResult>> f2, Func<T4, Task<TResult>> f3, Func<T5, Task<TResult>> f4, Func<T6, TResult> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return await f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return await f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return await f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return await f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return await f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, TResult> f0, Func<T2, TResult> f1, Func<T3, TResult> f2, Func<T4, TResult> f3, Func<T5, TResult> f4, Func<T6, Task<TResult>> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return await f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, TResult> f1, Func<T3, TResult> f2, Func<T4, TResult> f3, Func<T5, TResult> f4, Func<T6, Task<TResult>> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return await f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return await f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, TResult> f0, Func<T2, Task<TResult>> f1, Func<T3, TResult> f2, Func<T4, TResult> f3, Func<T5, TResult> f4, Func<T6, Task<TResult>> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return await f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return await f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, Task<TResult>> f1, Func<T3, TResult> f2, Func<T4, TResult> f3, Func<T5, TResult> f4, Func<T6, Task<TResult>> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return await f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return await f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return await f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, TResult> f0, Func<T2, TResult> f1, Func<T3, Task<TResult>> f2, Func<T4, TResult> f3, Func<T5, TResult> f4, Func<T6, Task<TResult>> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return await f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return await f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, TResult> f1, Func<T3, Task<TResult>> f2, Func<T4, TResult> f3, Func<T5, TResult> f4, Func<T6, Task<TResult>> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return await f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return await f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return await f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, TResult> f0, Func<T2, Task<TResult>> f1, Func<T3, Task<TResult>> f2, Func<T4, TResult> f3, Func<T5, TResult> f4, Func<T6, Task<TResult>> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return await f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return await f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return await f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, Task<TResult>> f1, Func<T3, Task<TResult>> f2, Func<T4, TResult> f3, Func<T5, TResult> f4, Func<T6, Task<TResult>> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return await f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return await f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return await f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return await f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, TResult> f0, Func<T2, TResult> f1, Func<T3, TResult> f2, Func<T4, Task<TResult>> f3, Func<T5, TResult> f4, Func<T6, Task<TResult>> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return await f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return await f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, TResult> f1, Func<T3, TResult> f2, Func<T4, Task<TResult>> f3, Func<T5, TResult> f4, Func<T6, Task<TResult>> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return await f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return await f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return await f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, TResult> f0, Func<T2, Task<TResult>> f1, Func<T3, TResult> f2, Func<T4, Task<TResult>> f3, Func<T5, TResult> f4, Func<T6, Task<TResult>> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return await f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return await f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return await f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, Task<TResult>> f1, Func<T3, TResult> f2, Func<T4, Task<TResult>> f3, Func<T5, TResult> f4, Func<T6, Task<TResult>> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return await f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return await f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return await f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return await f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, TResult> f0, Func<T2, TResult> f1, Func<T3, Task<TResult>> f2, Func<T4, Task<TResult>> f3, Func<T5, TResult> f4, Func<T6, Task<TResult>> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return await f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return await f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return await f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, TResult> f1, Func<T3, Task<TResult>> f2, Func<T4, Task<TResult>> f3, Func<T5, TResult> f4, Func<T6, Task<TResult>> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return await f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return await f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return await f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return await f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, TResult> f0, Func<T2, Task<TResult>> f1, Func<T3, Task<TResult>> f2, Func<T4, Task<TResult>> f3, Func<T5, TResult> f4, Func<T6, Task<TResult>> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return await f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return await f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return await f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return await f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, Task<TResult>> f1, Func<T3, Task<TResult>> f2, Func<T4, Task<TResult>> f3, Func<T5, TResult> f4, Func<T6, Task<TResult>> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return await f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return await f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return await f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return await f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return await f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, TResult> f0, Func<T2, TResult> f1, Func<T3, TResult> f2, Func<T4, TResult> f3, Func<T5, Task<TResult>> f4, Func<T6, Task<TResult>> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return await f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return await f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, TResult> f1, Func<T3, TResult> f2, Func<T4, TResult> f3, Func<T5, Task<TResult>> f4, Func<T6, Task<TResult>> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return await f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return await f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return await f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, TResult> f0, Func<T2, Task<TResult>> f1, Func<T3, TResult> f2, Func<T4, TResult> f3, Func<T5, Task<TResult>> f4, Func<T6, Task<TResult>> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return await f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return await f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return await f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, Task<TResult>> f1, Func<T3, TResult> f2, Func<T4, TResult> f3, Func<T5, Task<TResult>> f4, Func<T6, Task<TResult>> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return await f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return await f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return await f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return await f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, TResult> f0, Func<T2, TResult> f1, Func<T3, Task<TResult>> f2, Func<T4, TResult> f3, Func<T5, Task<TResult>> f4, Func<T6, Task<TResult>> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return await f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return await f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return await f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, TResult> f1, Func<T3, Task<TResult>> f2, Func<T4, TResult> f3, Func<T5, Task<TResult>> f4, Func<T6, Task<TResult>> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return await f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return await f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return await f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return await f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, TResult> f0, Func<T2, Task<TResult>> f1, Func<T3, Task<TResult>> f2, Func<T4, TResult> f3, Func<T5, Task<TResult>> f4, Func<T6, Task<TResult>> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return await f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return await f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return await f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return await f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, Task<TResult>> f1, Func<T3, Task<TResult>> f2, Func<T4, TResult> f3, Func<T5, Task<TResult>> f4, Func<T6, Task<TResult>> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return await f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return await f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return await f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return await f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return await f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, TResult> f0, Func<T2, TResult> f1, Func<T3, TResult> f2, Func<T4, Task<TResult>> f3, Func<T5, Task<TResult>> f4, Func<T6, Task<TResult>> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return await f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return await f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return await f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, TResult> f1, Func<T3, TResult> f2, Func<T4, Task<TResult>> f3, Func<T5, Task<TResult>> f4, Func<T6, Task<TResult>> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return await f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return await f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return await f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return await f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, TResult> f0, Func<T2, Task<TResult>> f1, Func<T3, TResult> f2, Func<T4, Task<TResult>> f3, Func<T5, Task<TResult>> f4, Func<T6, Task<TResult>> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return await f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return await f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return await f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return await f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, Task<TResult>> f1, Func<T3, TResult> f2, Func<T4, Task<TResult>> f3, Func<T5, Task<TResult>> f4, Func<T6, Task<TResult>> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return await f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return await f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return await f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return await f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return await f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, TResult> f0, Func<T2, TResult> f1, Func<T3, Task<TResult>> f2, Func<T4, Task<TResult>> f3, Func<T5, Task<TResult>> f4, Func<T6, Task<TResult>> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return await f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return await f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return await f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return await f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, TResult> f1, Func<T3, Task<TResult>> f2, Func<T4, Task<TResult>> f3, Func<T5, Task<TResult>> f4, Func<T6, Task<TResult>> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return await f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return await f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return await f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return await f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return await f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, TResult> f0, Func<T2, Task<TResult>> f1, Func<T3, Task<TResult>> f2, Func<T4, Task<TResult>> f3, Func<T5, Task<TResult>> f4, Func<T6, Task<TResult>> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return await f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return await f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return await f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return await f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return await f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
        Func<T1, Task<TResult>> f0, Func<T2, Task<TResult>> f1, Func<T3, Task<TResult>> f2, Func<T4, Task<TResult>> f3, Func<T5, Task<TResult>> f4, Func<T6, Task<TResult>> f5)
    {
        if (oneOf.Index == 0 && f0 != null) return await f0(oneOf.AsT0);
        if (oneOf.Index == 1 && f1 != null) return await f1(oneOf.AsT1);
        if (oneOf.Index == 2 && f2 != null) return await f2(oneOf.AsT2);
        if (oneOf.Index == 3 && f3 != null) return await f3(oneOf.AsT3);
        if (oneOf.Index == 4 && f4 != null) return await f4(oneOf.AsT4);
        if (oneOf.Index == 5 && f5 != null) return await f5(oneOf.AsT5);
        throw new InvalidOperationException();
    }

    #endregion 6 types
}