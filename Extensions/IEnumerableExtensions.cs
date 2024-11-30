namespace SystemOfEquations.Extensions;

public static class IEnumerableExtensions
{
    public static bool None<T>(this IEnumerable<T> items, Func<T, bool> f) => !items.Any(f);
}
