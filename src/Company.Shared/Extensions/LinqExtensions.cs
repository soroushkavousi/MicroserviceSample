using System.Linq.Expressions;

namespace Company.Shared.Extensions;

/// <summary>
///     Extension methods for LINQ operations
/// </summary>
public static class LinqExtensions
{
    /// <summary>
    ///     Conditionally applies a Where clause to an IQueryable
    /// </summary>
    /// <typeparam name="T">The type of elements in the IQueryable</typeparam>
    /// <param name="source">The source IQueryable</param>
    /// <param name="condition">The condition that determines whether to apply the predicate</param>
    /// <param name="predicate">The filter expression to apply if the condition is true</param>
    /// <returns>Filtered IQueryable if condition is true; otherwise the original IQueryable</returns>
    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> source,
        bool condition,
        Expression<Func<T, bool>> predicate)
        => condition ? source.Where(predicate) : source;

    /// <summary>
    ///     Conditionally applies a Where clause to an IEnumerable
    /// </summary>
    /// <typeparam name="T">The type of elements in the IEnumerable</typeparam>
    /// <param name="source">The source IEnumerable</param>
    /// <param name="condition">The condition that determines whether to apply the predicate</param>
    /// <param name="predicate">The filter expression to apply if the condition is true</param>
    /// <returns>Filtered IEnumerable if condition is true; otherwise the original IEnumerable</returns>
    public static IEnumerable<T> WhereIf<T>(
        this IEnumerable<T> source,
        bool condition,
        Func<T, bool> predicate)
        => condition ? source.Where(predicate) : source;
}