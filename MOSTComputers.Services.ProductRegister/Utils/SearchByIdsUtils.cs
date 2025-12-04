namespace MOSTComputers.Services.ProductRegister.Utils;
internal static class SearchByIdsUtils
{
    public static List<int> RemoveValuesSmallerThanNumber(IEnumerable<int> intList, int minValueInclusive)
    {
        List<int> output = new();

        foreach (int item in intList)
        {
            if (item < minValueInclusive) continue;

            output.Add(item);
        }

        return output;
    }

    public static List<int> RemoveValuesSmallerThanOne(IEnumerable<int> intList)
    {
        return RemoveValuesSmallerThanNumber(intList, 1);
    }
}