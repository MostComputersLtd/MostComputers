using System.Text;

namespace MOSTComputers.Services.DAL.DAL.Repositories;

internal static class RepositoryCommonElements
{
    internal static string GetDelimeteredListFromIds(List<int> ids)
    {
        StringBuilder sb = new();

        for (int i = 0; i < ids.Count - 1; i++)
        {
            int id = ids[i];
            sb.Append(id);
            sb.Append(", ");
        }

        sb.Append(ids[^1]);

        return sb.ToString();
    }
}