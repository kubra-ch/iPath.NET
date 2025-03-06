using System.Linq.Dynamic.Core;

namespace iPath.Application.Querying;

public record SortDefinition(string SortColumn = "", bool SortAscending = true);


public static class SortExtension
{
    public static IQueryable<T> ApplySort<T>(this IQueryable<T> query, List<SortDefinition> sortDefinitions)
    {
        sortDefinitions?.ForEach(sortDefinition =>
        {
            query = query.OrderBy(sortDefinition.SortColumn + (sortDefinition.SortAscending ? " asc" : " desc"));
        });

        return query;
    }
}