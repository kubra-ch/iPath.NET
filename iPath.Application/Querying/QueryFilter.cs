using System.Linq.Expressions;

namespace iPath.Application.Querying;


public class RootFilter
{
    public List<Filter> Filters { get; set; } = new();
    public string Logic { get; set; } = "and";

    public void AddFilter(string Field, object Value)
    {
        Filters.Add(new Filter { Field = Field, Value = Value });
    }
}

public class Filter
{
    public string Field { get; set; }
    public string Operator { get; set; }
    public object Value { get; set; }
    public string Logic { get; set; } = "and"; // This is the nested "logic" property for the inner filters array
    public List<Filter> Filters { get; set; } = new(); // Nested filters array
}



public class QueryFilter<T>
{
    private static Expression BuildFilterExpression(Filter filter, ParameterExpression parameter)
    {
        if (filter.Filters != null && filter.Filters.Any())
        {
            if (filter.Logic?.ToLower() == "and")
            {
                var andFilters = filter.Filters.Select(f => BuildFilterExpression(f, parameter));
                return andFilters.Aggregate(Expression.AndAlso);
            }
            else if (filter.Logic?.ToLower() == "or")
            {
                var orFilters = filter.Filters.Select(f => BuildFilterExpression(f, parameter));
                return orFilters.Aggregate(Expression.OrElse);
            }
        }

        if (filter.Value == null || string.IsNullOrWhiteSpace(filter.Value.ToString()))
            return null;

        var property = Expression.Property(parameter, filter.Field);
        var constant = Expression.Constant(filter.Value);

        switch (filter.Operator.ToLower())
        {
            case "eq":
                return Expression.Equal(property, constant);
            case "neq":
                return Expression.NotEqual(property, constant);
            case "lt":
                return Expression.LessThan(property, constant);
            case "lte":
                return Expression.LessThanOrEqual(property, constant);
            case "gt":
                return Expression.GreaterThan(property, constant);
            case "gte":
                return Expression.GreaterThanOrEqual(property, constant);
            case "contains":
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                return Expression.Call(property, containsMethod, constant);
            case "startswith":
                var startsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
                return Expression.Call(property, startsWithMethod, Expression.Constant(filter.Value, typeof(string)));

            // Add more operators as needed...
            default:
                throw new ArgumentException($"Unsupported operator: {filter.Operator}");
        }
    }

    private static Expression<Func<T, bool>> GetAndFilterExpression(List<Filter> filters)
    {
        if (filters == null || !filters.Any())
            return null;

        var parameter = Expression.Parameter(typeof(T), "x");
        Expression andExpression = null;

        foreach (var filter in filters)
        {
            var filterExpression = BuildFilterExpression(filter, parameter);
            if (filterExpression != null)
            {
                if (andExpression == null)
                {
                    andExpression = filterExpression;
                }
                else
                {
                    andExpression = Expression.AndAlso(andExpression, filterExpression);
                }
            }
        }

        if (andExpression == null)
        {
            // Return default expression that always evaluates to false
            andExpression = Expression.Constant(false);
        }

        return Expression.Lambda<Func<T, bool>>(andExpression, parameter);
    }

    private static Expression<Func<T, bool>> GetOrFilterExpression(List<Filter> filters)
    {
        if (filters == null || !filters.Any())
            return null;

        var parameter = Expression.Parameter(typeof(T), "x");
        Expression orExpression = null;

        foreach (var filter in filters)
        {
            var filterExpression = BuildFilterExpression(filter, parameter);
            if (filterExpression != null)
            {
                if (orExpression == null)
                {
                    orExpression = filterExpression;
                }
                else
                {
                    orExpression = Expression.OrElse(orExpression, filterExpression);
                }
            }
        }

        if (orExpression == null)
        {
            // Return default expression that always evaluates to false
            orExpression = Expression.Constant(false);
        }

        return Expression.Lambda<Func<T, bool>>(orExpression, parameter);
    }

    public static IQueryable<T> ApplyFilter(IQueryable<T> query, RootFilter filter)
    {
        if (filter == null || filter.Filters == null || !filter.Filters.Any())
            return query;

        Expression<Func<T, bool>> compositeFilterExpression = null;

        if (filter.Logic?.ToLower() == "and")
        {
            compositeFilterExpression = GetAndFilterExpression(filter.Filters);
        }
        else if (filter.Logic?.ToLower() == "or")
        {
            compositeFilterExpression = GetOrFilterExpression(filter.Filters);
        }

        return compositeFilterExpression != null
            ? query.Where(compositeFilterExpression)
            : query;
    }
}
