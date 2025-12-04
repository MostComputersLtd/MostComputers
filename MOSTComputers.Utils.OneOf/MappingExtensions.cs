using OneOf;

namespace MOSTComputers.Utils.OneOf;
public static class MappingExtensions
{

    #region 2 types

    #region 2 types, 1 type input

    public static OneOf<T1, T2> Map<T1, T2>(this OneOf<T1> input)
    {
        return input.Match<OneOf<T1, T2>>(
            t1 => t1);
    }

    public static OneOf<T1, T2> Map<T1, T2>(this OneOf<T2> input)
    {
        return input.Match<OneOf<T1, T2>>(
            t2 => t2);
    }

    #endregion 2 types, 1 type input

    #endregion 2 types

    #region 3 types

    #region 3 types, 1 type input

    public static OneOf<T1, T2, T3> Map<T1, T2, T3>(this OneOf<T1> input)
    {
        return input.Match<OneOf<T1, T2, T3>>(
            t1 => t1);
    }

    public static OneOf<T1, T2, T3> Map<T1, T2, T3>(this OneOf<T2> input)
    {
        return input.Match<OneOf<T1, T2, T3>>(
            t2 => t2);
    }

    public static OneOf<T1, T2, T3> Map<T1, T2, T3>(this OneOf<T3> input)
    {
        return input.Match<OneOf<T1, T2, T3>>(
            t3 => t3);
    }

    #endregion 3 types, 1 type input

    #region 3 types, 2 types input

    #region 1, 2

    public static OneOf<T1, T2, T3> Map<T1, T2, T3>(this OneOf<T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3>>(
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3> Map<T1, T2, T3>(this OneOf<T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3>>(
            t2 => t2,
            t1 => t1);
    }

    #endregion 1, 2

    #region 1, 3

    public static OneOf<T1, T2, T3> Map<T1, T2, T3>(this OneOf<T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3>>(
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3> Map<T1, T2, T3>(this OneOf<T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3>>(
            t3 => t3,
            t1 => t1);
    }

    #endregion 1, 3

    #region 2, 3

    public static OneOf<T1, T2, T3> Map<T1, T2, T3>(this OneOf<T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3>>(
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3> Map<T1, T2, T3>(this OneOf<T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3>>(
            t3 => t3,
            t2 => t2);
    }

    #endregion 2, 3

    #endregion 3 types, 2 types input

    #endregion 3 types

    #region 4 types

    #region 4 types, 1 type input

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t4 => t4);
    }

    #endregion 4 types, 1 type input

    #region 4 types, 2 types input

    #region 1, 2

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t2 => t2,
            t1 => t1);
    }

    #endregion 1, 2

    #region 1, 3

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t3 => t3,
            t1 => t1);
    }

    #endregion 1, 3

    #region 1, 4

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t1 => t1,
            t4 => t4);
    }


    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t4 => t4,
            t1 => t1);
    }

    #endregion 1, 4

    #region 2, 3

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t3 => t3,
            t2 => t2);
    }

    #endregion 2, 3

    #region 2, 4

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t4 => t4,
            t2 => t2);
    }

    #endregion 2, 4

    #region 3, 4

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t4 => t4,
            t3 => t3);
    }

    #endregion 3, 4

    #endregion 4 types, 2 types input

    #region 4 types, 3 types input

    #region 1, 2, 3

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T1, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t1 => t1,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T1, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t1 => t1,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T2, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t2 => t2,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T3, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t3 => t3,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T2, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t2 => t2,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T3, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t3 => t3,
            t2 => t2,
            t1 => t1);
    }

    #endregion 1, 2, 3

    #region 1, 2, 4

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T1, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t1 => t1,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T1, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t1 => t1,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T2, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t2 => t2,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T4, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t4 => t4,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T2, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t2 => t2,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T4, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t4 => t4,
            t2 => t2,
            t1 => t1);
    }

    #endregion 1, 2, 4

    #region 1, 3, 4

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T1, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t1 => t1,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T1, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t1 => t1,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T3, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t3 => t3,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T4, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t4 => t4,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T3, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t3 => t3,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T4, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t4 => t4,
            t3 => t3,
            t1 => t1);
    }

    #endregion 1, 3, 4

    #region 2, 3, 4

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T2, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t2 => t2,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T2, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t2 => t2,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T3, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t3 => t3,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T4, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t4 => t4,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T3, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t3 => t3,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4> Map<T1, T2, T3, T4>(this OneOf<T4, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4>>(
            t4 => t4,
            t3 => t3,
            t2 => t2);
    }

    #endregion 2, 3, 4


    #endregion 4 types, 3 types input

    #endregion 4 types

    #region 5 types

    #region 5 types, 1 type input

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5);
    }

    #endregion 5 types, 1 type input

    #region 5 types, 2 types input

    #region 1, 2

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t1 => t1);
    }

    #endregion 1, 2

    #region 1, 3

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t1 => t1);
    }

    #endregion 1, 3

    #region 1, 4

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t1 => t1);
    }

    #endregion 1, 4

    #region 1, 5

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t1 => t1);
    }

    #endregion 1, 5

    #region 2, 3

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t2 => t2);
    }

    #endregion 2, 3

    #region 2, 4

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t2 => t2);
    }

    #endregion 2, 4

    #region 2, 5

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t2 => t2);
    }

    #endregion 2, 5

    #region 3, 4

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t3 => t3);
    }

    #endregion 3, 4

    #region 3, 5

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t3 => t3);
    }

    #endregion 3, 5

    #region 4, 5

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t4 => t4);
    }

    #endregion 4, 5


    #endregion 5 types, 2 types input

    #region 5 types, 3 types input

    #region 1, 2, 3

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t2 => t2,
            t1 => t1);
    }

    #endregion 1, 2, 3

    #region 1, 2, 4

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t2 => t2,
            t1 => t1);
    }

    #endregion 1, 2, 4

    #region 1, 2, 5

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t2 => t2,
            t1 => t1);
    }

    #endregion 1, 2, 5

    #region 1, 3, 4

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t3 => t3,
            t1 => t1);
    }

    #endregion 1, 3, 4

    #region 1, 3, 5

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t3 => t3,
            t1 => t1);
    }

    #endregion 1, 3, 5

    #region 1, 4, 5

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t4 => t4,
            t1 => t1);
    }

    #endregion 1, 4, 5

    #region 2, 3, 4

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t3 => t3,
            t2 => t2);
    }

    #endregion 2, 3, 4

    #region 2, 3, 5

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t3 => t3,
            t2 => t2);
    }

    #endregion 2, 3, 5

    #region 2, 4, 5

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t4 => t4,
            t2 => t2);
    }

    #endregion 2, 4, 5

    #region 3, 4, 5

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t4 => t4,
            t3 => t3);
    }

    #endregion 3, 4, 5

    #endregion 5 types, 3 types input

    #region 5 types, 4 types input

    #region 1, 2, 3, 4

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T2, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t2 => t2,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T2, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t2 => t2,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T3, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t3 => t3,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T4, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t4 => t4,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T3, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t3 => t3,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T4, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t4 => t4,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T1, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t1 => t1,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T1, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t1 => t1,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T3, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t3 => t3,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T4, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t4 => t4,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T3, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t3 => t3,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T4, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t4 => t4,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T1, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t1 => t1,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T1, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t1 => t1,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T2, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t2 => t2,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T4, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t4 => t4,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T2, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t2 => t2,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T4, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t4 => t4,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T1, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t1 => t1,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T1, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t1 => t1,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T2, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t2 => t2,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T3, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t3 => t3,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T2, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t2 => t2,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T3, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t3 => t3,
            t2 => t2,
            t1 => t1);
    }

    #endregion 1, 2, 3, 4

    #region 1, 2, 3, 5

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T2, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t2 => t2,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T2, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t2 => t2,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T3, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t3 => t3,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T5, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t5 => t5,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T3, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t3 => t3,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T5, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t5 => t5,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T1, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t1 => t1,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T1, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t1 => t1,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T3, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t3 => t3,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T5, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t5 => t5,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T3, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t3 => t3,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T5, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t5 => t5,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T1, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t1 => t1,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T1, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t1 => t1,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T2, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t2 => t2,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T5, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t5 => t5,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T2, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t2 => t2,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T5, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t5 => t5,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T1, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t1 => t1,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T1, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t1 => t1,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T2, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t2 => t2,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T3, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t3 => t3,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T2, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t2 => t2,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T3, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t3 => t3,
            t2 => t2,
            t1 => t1);
    }

    #endregion 1, 2, 3, 5

    #region 1, 2, 4, 5

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T2, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t2 => t2,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T2, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t2 => t2,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T4, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t4 => t4,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T5, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t5 => t5,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T4, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t4 => t4,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T5, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t5 => t5,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T1, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t1 => t1,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T1, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t1 => t1,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T4, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t4 => t4,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T5, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t5 => t5,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T4, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t4 => t4,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T5, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t5 => t5,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T1, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t1 => t1,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T1, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t1 => t1,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T2, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t2 => t2,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T5, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t5 => t5,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T2, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t2 => t2,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T5, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t5 => t5,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T1, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t1 => t1,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T1, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t1 => t1,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T2, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t2 => t2,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T4, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t4 => t4,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T2, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t2 => t2,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T4, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t4 => t4,
            t2 => t2,
            t1 => t1);
    }

    #endregion 1, 2, 4, 5

    #region 1, 3, 4, 5

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T3, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t3 => t3,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T3, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t3 => t3,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T4, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t4 => t4,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T5, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t5 => t5,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T4, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t4 => t4,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T1, T5, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t1 => t1,
            t5 => t5,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T1, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t1 => t1,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T1, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t1 => t1,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T4, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t4 => t4,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T5, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t5 => t5,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T4, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t4 => t4,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T5, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t5 => t5,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T1, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t1 => t1,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T1, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t1 => t1,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T3, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t3 => t3,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T5, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t5 => t5,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T3, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t3 => t3,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T5, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t5 => t5,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T1, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t1 => t1,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T1, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t1 => t1,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T3, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t3 => t3,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T4, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t4 => t4,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T3, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t3 => t3,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T4, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t4 => t4,
            t3 => t3,
            t1 => t1);
    }

    #endregion 1, 3, 4, 5

    #region 2, 3, 4, 5

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T3, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t3 => t3,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T3, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t3 => t3,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T4, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t4 => t4,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T5, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t5 => t5,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T4, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t4 => t4,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T2, T5, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t2 => t2,
            t5 => t5,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T2, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t2 => t2,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T2, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t2 => t2,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T4, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t4 => t4,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T5, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t5 => t5,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T4, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t4 => t4,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T3, T5, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t3 => t3,
            t5 => t5,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T2, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t2 => t2,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T2, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t2 => t2,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T3, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t3 => t3,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T5, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t5 => t5,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T3, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t3 => t3,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T4, T5, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t4 => t4,
            t5 => t5,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T2, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t2 => t2,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T2, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t2 => t2,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T3, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t3 => t3,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T4, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t4 => t4,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T3, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t3 => t3,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5> Map<T1, T2, T3, T4, T5>(this OneOf<T5, T4, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5>>(
            t5 => t5,
            t4 => t4,
            t3 => t3,
            t2 => t2);
    }

    #endregion 2, 3, 4, 5

    #endregion 5 types, 4 types input

    #endregion 5 types

    #region 6 types

    #region 6 types, 1 type input

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6);
    }

    #endregion 6 types, 1 type input

    #region 6 types, 2 type inputs

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5);
    }

    #endregion 6 types, 2 type inputs

    #region 6 types, 3 type inputs

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t4 => t4);
    }

    #endregion 6 types, 3 type inputs

    #region 6 types, 4 type inputs

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t4 => t4,
            t3 => t3);
    }

    #endregion 6 types, 4 type inputs

    #region 6 types, 5 type inputs

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T3, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t3 => t3,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T3, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t3 => t3,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T4, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t4 => t4,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T4, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t4 => t4,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T5, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t5 => t5,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T5, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t5 => t5,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T2, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t2 => t2,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T2, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t2 => t2,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T4, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t4 => t4,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T4, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t4 => t4,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T5, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t5 => t5,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T5, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t5 => t5,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T2, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t2 => t2,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T2, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t2 => t2,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T3, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t3 => t3,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T3, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t3 => t3,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T5, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t5 => t5,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T5, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t5 => t5,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T2, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t2 => t2,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T2, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t2 => t2,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T3, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t3 => t3,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T3, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t3 => t3,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T4, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t4 => t4,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T4, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t4 => t4,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T3, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t3 => t3,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T3, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t3 => t3,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T4, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t4 => t4,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T4, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t4 => t4,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T5, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t5 => t5,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T5, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t5 => t5,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T1, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t1 => t1,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T1, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t1 => t1,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T4, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t4 => t4,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T4, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t4 => t4,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T5, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t5 => t5,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T5, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t5 => t5,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T1, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t1 => t1,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T1, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t1 => t1,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T3, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t3 => t3,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T3, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t3 => t3,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T5, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t5 => t5,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T5, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t5 => t5,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T1, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t1 => t1,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T1, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t1 => t1,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T3, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t3 => t3,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T3, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t3 => t3,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T4, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t4 => t4,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T4, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t4 => t4,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T2, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t2 => t2,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T2, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t2 => t2,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T4, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t4 => t4,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T4, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t4 => t4,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T5, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t5 => t5,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T5, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t5 => t5,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T1, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t1 => t1,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T1, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t1 => t1,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T4, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t4 => t4,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T4, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t4 => t4,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T5, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t5 => t5,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T5, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t5 => t5,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T1, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t1 => t1,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T1, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t1 => t1,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T2, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t2 => t2,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T2, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t2 => t2,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T5, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t5 => t5,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T5, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t5 => t5,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T1, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t1 => t1,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T1, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t1 => t1,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T2, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t2 => t2,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T2, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t2 => t2,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T4, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t4 => t4,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T4, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t4 => t4,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T2, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t2 => t2,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T2, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t2 => t2,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T3, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t3 => t3,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T3, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t3 => t3,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T5, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t5 => t5,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T5, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t5 => t5,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T1, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t1 => t1,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T1, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t1 => t1,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T3, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t3 => t3,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T3, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t3 => t3,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T5, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t5 => t5,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T5, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t5 => t5,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T1, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t1 => t1,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T1, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t1 => t1,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T2, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t2 => t2,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T2, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t2 => t2,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T5, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t5 => t5,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T5, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t5 => t5,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T1, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t1 => t1,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T1, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t1 => t1,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T2, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t2 => t2,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T2, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t2 => t2,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T3, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t3 => t3,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T3, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t3 => t3,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T2, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t2 => t2,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T2, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t2 => t2,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T3, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t3 => t3,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T3, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t3 => t3,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T4, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t4 => t4,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T4, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t4 => t4,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T1, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t1 => t1,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T1, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t1 => t1,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T3, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t3 => t3,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T3, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t3 => t3,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T4, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t4 => t4,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T4, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t4 => t4,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T1, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t1 => t1,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T1, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t1 => t1,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T2, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t2 => t2,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T2, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t2 => t2,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T4, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t4 => t4,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T4, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t4 => t4,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T1, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t1 => t1,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T1, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t1 => t1,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T2, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t2 => t2,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T2, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t2 => t2,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T3, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t3 => t3,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T3, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t3 => t3,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T3, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t3 => t3,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T3, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t3 => t3,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T4, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t4 => t4,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T4, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t4 => t4,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T6, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t6 => t6,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T6, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t6 => t6,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T2, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t2 => t2,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T2, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t2 => t2,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T4, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t4 => t4,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T4, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t4 => t4,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T6, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t6 => t6,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T6, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t6 => t6,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T2, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t2 => t2,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T2, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t2 => t2,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T3, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t3 => t3,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T3, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t3 => t3,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T6, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t6 => t6,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T6, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t6 => t6,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T2, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t2 => t2,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T2, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t2 => t2,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T3, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t3 => t3,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T3, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t3 => t3,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T4, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t4 => t4,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T4, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t4 => t4,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T3, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t3 => t3,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T3, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t3 => t3,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T4, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t4 => t4,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T4, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t4 => t4,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T6, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t6 => t6,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T6, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t6 => t6,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T1, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t1 => t1,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T1, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t1 => t1,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T4, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t4 => t4,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T4, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t4 => t4,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T6, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t6 => t6,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T6, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t6 => t6,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T1, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t1 => t1,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T1, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t1 => t1,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T3, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t3 => t3,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T3, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t3 => t3,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T6, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t6 => t6,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T6, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t6 => t6,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T1, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t1 => t1,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T1, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t1 => t1,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T3, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t3 => t3,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T3, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t3 => t3,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T4, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t4 => t4,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T4, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t4 => t4,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T2, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t2 => t2,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T2, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t2 => t2,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T4, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t4 => t4,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T4, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t4 => t4,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T6, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t6 => t6,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T6, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t6 => t6,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T1, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t1 => t1,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T1, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t1 => t1,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T4, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t4 => t4,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T4, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t4 => t4,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T6, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t6 => t6,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T6, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t6 => t6,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T1, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t1 => t1,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T1, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t1 => t1,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T2, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t2 => t2,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T2, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t2 => t2,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T6, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t6 => t6,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T6, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t6 => t6,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T1, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t1 => t1,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T1, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t1 => t1,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T2, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t2 => t2,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T2, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t2 => t2,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T4, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t4 => t4,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T4, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t4 => t4,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T2, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t2 => t2,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T2, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t2 => t2,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T3, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t3 => t3,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T3, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t3 => t3,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T6, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t6 => t6,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T6, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t6 => t6,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T1, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t1 => t1,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T1, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t1 => t1,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T3, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t3 => t3,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T3, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t3 => t3,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T6, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t6 => t6,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T6, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t6 => t6,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T1, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t1 => t1,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T1, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t1 => t1,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T2, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t2 => t2,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T2, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t2 => t2,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T6, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t6 => t6,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T6, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t6 => t6,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T1, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t1 => t1,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T1, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t1 => t1,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T2, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t2 => t2,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T2, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t2 => t2,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T3, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t3 => t3,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T3, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t3 => t3,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T2, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t2 => t2,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T2, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t2 => t2,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T3, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t3 => t3,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T3, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t3 => t3,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T4, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t4 => t4,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T4, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t4 => t4,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T1, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t1 => t1,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T1, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t1 => t1,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T3, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t3 => t3,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T3, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t3 => t3,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T4, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t4 => t4,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T4, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t4 => t4,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T1, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t1 => t1,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T1, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t1 => t1,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T2, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t2 => t2,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T2, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t2 => t2,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T4, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t4 => t4,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T4, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t4 => t4,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T1, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t1 => t1,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T1, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t1 => t1,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T2, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t2 => t2,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T2, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t2 => t2,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T3, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t3 => t3,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T3, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t3 => t3,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T3, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t3 => t3,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T3, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t3 => t3,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T5, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t5 => t5,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T5, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t5 => t5,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T6, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t6 => t6,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T6, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t6 => t6,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T2, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t2 => t2,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T2, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t2 => t2,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T5, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t5 => t5,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T5, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t5 => t5,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T6, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t6 => t6,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T6, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t6 => t6,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T2, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t2 => t2,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T2, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t2 => t2,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T3, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t3 => t3,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T3, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t3 => t3,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T6, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t6 => t6,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T6, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t6 => t6,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T2, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t2 => t2,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T2, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t2 => t2,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T3, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t3 => t3,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T3, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t3 => t3,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T5, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t5 => t5,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T5, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t5 => t5,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T3, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t3 => t3,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T3, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t3 => t3,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T5, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t5 => t5,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T5, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t5 => t5,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T6, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t6 => t6,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T6, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t6 => t6,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T1, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t1 => t1,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T1, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t1 => t1,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T5, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t5 => t5,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T5, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t5 => t5,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T6, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t6 => t6,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T6, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t6 => t6,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T1, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t1 => t1,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T1, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t1 => t1,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T3, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t3 => t3,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T3, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t3 => t3,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T6, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t6 => t6,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T6, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t6 => t6,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T1, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t1 => t1,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T1, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t1 => t1,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T3, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t3 => t3,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T3, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t3 => t3,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T5, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t5 => t5,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T5, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t5 => t5,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T2, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t2 => t2,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T2, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t2 => t2,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T5, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t5 => t5,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T5, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t5 => t5,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T6, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t6 => t6,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T6, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t6 => t6,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T1, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t1 => t1,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T1, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t1 => t1,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T5, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t5 => t5,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T5, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t5 => t5,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T6, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t6 => t6,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T6, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t6 => t6,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T1, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t1 => t1,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T1, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t1 => t1,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T2, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t2 => t2,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T2, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t2 => t2,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T6, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t6 => t6,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T6, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t6 => t6,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T1, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t1 => t1,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T1, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t1 => t1,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T2, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t2 => t2,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T2, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t2 => t2,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T5, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t5 => t5,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T5, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t5 => t5,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T2, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t2 => t2,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T2, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t2 => t2,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T3, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t3 => t3,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T3, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t3 => t3,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T6, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t6 => t6,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T6, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t6 => t6,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T1, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t1 => t1,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T1, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t1 => t1,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T3, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t3 => t3,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T3, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t3 => t3,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T6, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t6 => t6,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T6, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t6 => t6,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T1, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t1 => t1,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T1, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t1 => t1,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T2, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t2 => t2,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T2, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t2 => t2,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T6, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t6 => t6,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T6, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t6 => t6,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T1, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t1 => t1,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T1, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t1 => t1,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T2, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t2 => t2,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T2, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t2 => t2,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T3, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t3 => t3,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T3, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t3 => t3,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T2, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t2 => t2,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T2, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t2 => t2,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T3, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t3 => t3,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T3, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t3 => t3,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T5, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t5 => t5,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T5, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t5 => t5,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T1, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t1 => t1,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T1, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t1 => t1,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T3, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t3 => t3,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T3, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t3 => t3,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T5, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t5 => t5,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T5, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t5 => t5,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T1, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t1 => t1,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T1, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t1 => t1,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T2, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t2 => t2,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T2, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t2 => t2,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T5, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t5 => t5,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T5, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t5 => t5,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T1, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t1 => t1,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T1, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t1 => t1,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T2, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t2 => t2,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T2, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t2 => t2,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T3, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t3 => t3,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T3, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t3 => t3,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T4, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t4 => t4,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T4, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t4 => t4,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T5, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t5 => t5,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T5, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t5 => t5,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T6, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t6 => t6,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T2, T6, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t2 => t2,
            t6 => t6,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T2, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t2 => t2,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T2, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t2 => t2,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T5, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t5 => t5,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T5, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t5 => t5,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T6, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t6 => t6,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T6, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t6 => t6,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T2, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t2 => t2,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T2, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t2 => t2,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T4, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t4 => t4,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T4, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t4 => t4,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T6, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t6 => t6,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T6, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t6 => t6,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T2, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t2 => t2,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T2, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t2 => t2,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T4, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t4 => t4,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T4, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t4 => t4,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T5, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t5 => t5,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T5, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t5 => t5,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T4, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t4 => t4,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T4, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t4 => t4,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T5, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t5 => t5,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T5, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t5 => t5,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T6, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t6 => t6,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T1, T6, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t1 => t1,
            t6 => t6,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T1, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t1 => t1,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T1, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t1 => t1,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T5, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t5 => t5,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T5, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t5 => t5,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T6, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t6 => t6,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T6, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t6 => t6,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T1, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t1 => t1,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T1, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t1 => t1,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T4, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t4 => t4,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T4, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t4 => t4,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T6, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t6 => t6,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T6, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t6 => t6,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T1, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t1 => t1,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T1, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t1 => t1,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T4, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t4 => t4,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T4, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t4 => t4,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T5, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t5 => t5,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T5, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t5 => t5,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T2, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t2 => t2,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T2, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t2 => t2,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T5, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t5 => t5,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T5, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t5 => t5,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T6, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t6 => t6,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T6, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t6 => t6,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T1, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t1 => t1,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T1, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t1 => t1,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T5, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t5 => t5,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T5, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t5 => t5,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T6, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t6 => t6,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T6, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t6 => t6,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T1, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t1 => t1,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T1, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t1 => t1,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T2, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t2 => t2,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T2, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t2 => t2,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T6, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t6 => t6,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T6, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t6 => t6,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T1, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t1 => t1,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T1, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t1 => t1,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T2, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t2 => t2,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T2, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t2 => t2,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T5, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t5 => t5,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T5, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t5 => t5,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T2, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t2 => t2,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T2, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t2 => t2,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T4, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t4 => t4,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T4, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t4 => t4,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T6, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t6 => t6,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T6, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t6 => t6,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T1, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t1 => t1,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T1, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t1 => t1,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T4, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t4 => t4,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T4, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t4 => t4,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T6, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t6 => t6,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T6, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t6 => t6,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T1, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t1 => t1,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T1, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t1 => t1,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T2, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t2 => t2,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T2, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t2 => t2,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T6, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t6 => t6,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T6, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t6 => t6,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T1, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t1 => t1,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T1, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t1 => t1,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T2, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t2 => t2,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T2, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t2 => t2,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T4, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t4 => t4,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T4, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t4 => t4,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T2, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t2 => t2,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T2, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t2 => t2,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T4, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t4 => t4,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T4, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t4 => t4,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T5, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t5 => t5,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T5, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t5 => t5,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T1, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t1 => t1,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T1, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t1 => t1,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T4, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t4 => t4,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T4, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t4 => t4,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T5, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t5 => t5,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T5, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t5 => t5,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T1, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t1 => t1,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T1, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t1 => t1,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T2, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t2 => t2,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T2, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t2 => t2,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T5, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t5 => t5,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T5, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t5 => t5,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T1, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t1 => t1,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T1, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t1 => t1,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T2, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t2 => t2,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T2, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t2 => t2,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T4, T1, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t4 => t4,
            t1 => t1,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T4, T2, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t4 => t4,
            t2 => t2,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T4, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t4 => t4,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T4, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t4 => t4,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T5, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t5 => t5,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T5, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t5 => t5,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T6, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t6 => t6,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T3, T6, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t3 => t3,
            t6 => t6,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T3, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t3 => t3,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T3, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t3 => t3,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T5, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t5 => t5,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T5, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t5 => t5,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T6, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t6 => t6,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T4, T6, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t4 => t4,
            t6 => t6,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T3, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t3 => t3,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T3, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t3 => t3,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T4, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t4 => t4,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T4, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t4 => t4,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T6, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t6 => t6,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T5, T6, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t5 => t5,
            t6 => t6,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T3, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t3 => t3,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T3, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t3 => t3,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T4, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t4 => t4,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T4, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t4 => t4,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T5, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t5 => t5,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T1, T6, T5, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t1 => t1,
            t6 => t6,
            t5 => t5,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T4, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t4 => t4,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T4, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t4 => t4,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T5, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t5 => t5,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T5, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t5 => t5,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T6, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t6 => t6,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T1, T6, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t1 => t1,
            t6 => t6,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T1, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t1 => t1,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T1, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t1 => t1,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T5, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t5 => t5,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T5, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t5 => t5,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T6, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t6 => t6,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T6, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t6 => t6,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T1, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t1 => t1,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T1, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t1 => t1,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T4, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t4 => t4,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T4, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t4 => t4,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T6, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t6 => t6,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T6, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t6 => t6,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T1, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t1 => t1,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T1, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t1 => t1,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T4, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t4 => t4,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T4, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t4 => t4,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T5, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t5 => t5,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T5, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t5 => t5,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T3, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t3 => t3,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T3, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t3 => t3,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T5, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t5 => t5,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T5, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t5 => t5,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T6, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t6 => t6,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T1, T6, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t1 => t1,
            t6 => t6,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T1, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t1 => t1,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T1, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t1 => t1,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T5, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t5 => t5,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T5, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t5 => t5,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T6, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t6 => t6,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T6, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t6 => t6,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T1, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t1 => t1,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T1, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t1 => t1,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T3, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t3 => t3,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T3, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t3 => t3,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T6, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t6 => t6,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T6, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t6 => t6,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T1, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t1 => t1,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T1, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t1 => t1,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T3, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t3 => t3,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T3, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t3 => t3,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T5, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t5 => t5,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T5, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t5 => t5,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T3, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t3 => t3,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T3, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t3 => t3,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T4, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t4 => t4,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T4, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t4 => t4,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T6, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t6 => t6,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T1, T6, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t1 => t1,
            t6 => t6,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T1, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t1 => t1,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T1, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t1 => t1,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T4, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t4 => t4,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T4, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t4 => t4,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T6, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t6 => t6,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T6, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t6 => t6,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T1, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t1 => t1,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T1, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t1 => t1,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T3, T1, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t3 => t3,
            t1 => t1,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T3, T6, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t3 => t3,
            t6 => t6,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T6, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t6 => t6,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T6, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t6 => t6,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T1, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t1 => t1,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T1, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t1 => t1,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T3, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t3 => t3,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T3, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t3 => t3,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T4, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t4 => t4,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T4, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t4 => t4,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T3, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t3 => t3,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T3, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t3 => t3,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T4, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t4 => t4,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T4, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t4 => t4,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T5, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t5 => t5,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T1, T5, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t1 => t1,
            t5 => t5,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T1, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t1 => t1,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T1, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t1 => t1,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T4, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t4 => t4,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T4, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t4 => t4,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T5, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t5 => t5,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T5, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t5 => t5,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T1, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t1 => t1,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T1, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t1 => t1,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T3, T1, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t3 => t3,
            t1 => t1,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T3, T5, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t3 => t3,
            t5 => t5,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T5, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t5 => t5,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T5, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t5 => t5,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T1, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t1 => t1,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T1, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t1 => t1,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T3, T1, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t3 => t3,
            t1 => t1,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T3, T4, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t3 => t3,
            t4 => t4,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T4, T1, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t4 => t4,
            t1 => t1,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T4, T3, T1> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t4 => t4,
            t3 => t3,
            t1 => t1);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T4, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t4 => t4,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T4, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t4 => t4,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T5, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t5 => t5,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T5, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t5 => t5,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T6, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t6 => t6,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T3, T6, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t3 => t3,
            t6 => t6,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T3, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t3 => t3,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T3, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t3 => t3,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T5, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t5 => t5,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T5, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t5 => t5,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T6, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t6 => t6,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T4, T6, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t4 => t4,
            t6 => t6,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T3, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t3 => t3,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T3, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t3 => t3,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T4, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t4 => t4,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T4, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t4 => t4,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T6, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t6 => t6,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T5, T6, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t5 => t5,
            t6 => t6,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T3, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t3 => t3,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T3, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t3 => t3,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T4, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t4 => t4,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T4, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t4 => t4,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T5, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t5 => t5,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T2, T6, T5, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t2 => t2,
            t6 => t6,
            t5 => t5,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T4, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t4 => t4,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T4, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t4 => t4,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T5, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t5 => t5,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T5, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t5 => t5,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T6, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t6 => t6,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T2, T6, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t2 => t2,
            t6 => t6,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T2, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t2 => t2,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T2, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t2 => t2,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T5, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t5 => t5,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T5, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t5 => t5,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T6, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t6 => t6,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T4, T6, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t4 => t4,
            t6 => t6,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T2, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t2 => t2,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T2, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t2 => t2,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T4, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t4 => t4,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T4, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t4 => t4,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T6, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t6 => t6,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T5, T6, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t5 => t5,
            t6 => t6,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T2, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t2 => t2,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T2, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t2 => t2,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T4, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t4 => t4,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T4, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t4 => t4,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T5, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t5 => t5,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T3, T6, T5, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t3 => t3,
            t6 => t6,
            t5 => t5,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T3, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t3 => t3,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T3, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t3 => t3,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T5, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t5 => t5,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T5, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t5 => t5,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T6, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t6 => t6,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T2, T6, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t2 => t2,
            t6 => t6,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T2, T5, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t2 => t2,
            t5 => t5,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T2, T6, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t2 => t2,
            t6 => t6,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T5, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t5 => t5,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T5, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t5 => t5,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T6, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t6 => t6,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T3, T6, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t3 => t3,
            t6 => t6,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T2, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t2 => t2,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T2, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t2 => t2,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T3, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t3 => t3,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T3, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t3 => t3,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T6, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t6 => t6,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T5, T6, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t5 => t5,
            t6 => t6,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T2, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t2 => t2,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T2, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t2 => t2,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T3, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t3 => t3,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T3, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t3 => t3,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T5, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t5 => t5,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T4, T6, T5, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t4 => t4,
            t6 => t6,
            t5 => t5,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T3, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t3 => t3,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T3, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t3 => t3,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T4, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t4 => t4,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T4, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t4 => t4,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T6, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t6 => t6,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T2, T6, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t2 => t2,
            t6 => t6,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T2, T4, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t2 => t2,
            t4 => t4,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T2, T6, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t2 => t2,
            t6 => t6,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T4, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t4 => t4,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T4, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t4 => t4,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T6, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t6 => t6,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T3, T6, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t3 => t3,
            t6 => t6,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T2, T3, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t2 => t2,
            t3 => t3,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T2, T6, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t2 => t2,
            t6 => t6,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T3, T2, T6> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t3 => t3,
            t2 => t2,
            t6 => t6);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T3, T6, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t3 => t3,
            t6 => t6,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T6, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t6 => t6,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T4, T6, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t4 => t4,
            t6 => t6,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T2, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t2 => t2,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T2, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t2 => t2,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T3, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t3 => t3,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T3, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t3 => t3,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T4, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t4 => t4,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T5, T6, T4, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t5 => t5,
            t6 => t6,
            t4 => t4,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T3, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t3 => t3,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T3, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t3 => t3,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T4, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t4 => t4,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T4, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t4 => t4,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T5, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t5 => t5,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T2, T5, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t2 => t2,
            t5 => t5,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T2, T4, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t2 => t2,
            t4 => t4,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T2, T5, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t2 => t2,
            t5 => t5,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T4, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t4 => t4,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T4, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t4 => t4,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T5, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t5 => t5,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T3, T5, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t3 => t3,
            t5 => t5,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T2, T3, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t2 => t2,
            t3 => t3,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T2, T5, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t2 => t2,
            t5 => t5,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T3, T2, T5> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t3 => t3,
            t2 => t2,
            t5 => t5);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T3, T5, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t3 => t3,
            t5 => t5,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T5, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t5 => t5,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T4, T5, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t4 => t4,
            t5 => t5,
            t3 => t3,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T2, T3, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t2 => t2,
            t3 => t3,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T2, T4, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t2 => t2,
            t4 => t4,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T3, T2, T4> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t3 => t3,
            t2 => t2,
            t4 => t4);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T3, T4, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t3 => t3,
            t4 => t4,
            t2 => t2);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T4, T2, T3> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t4 => t4,
            t2 => t2,
            t3 => t3);
    }

    public static OneOf<T1, T2, T3, T4, T5, T6> Map<T1, T2, T3, T4, T5, T6>(this OneOf<T6, T5, T4, T3, T2> input)
    {
        return input.Match<OneOf<T1, T2, T3, T4, T5, T6>>(
            t6 => t6,
            t5 => t5,
            t4 => t4,
            t3 => t3,
            t2 => t2);
    }

    #endregion 6 types, 5 type inputs

    #endregion 6 types
}