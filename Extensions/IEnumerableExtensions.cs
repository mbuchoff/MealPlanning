namespace SystemOfEquations.Extensions;

public static class IEnumerableExtensions
{
    public static IEnumerable<T> Append<T>(this IEnumerable<T> items1, IEnumerable<T> items2)
    {
        foreach (var item in items1)
        {
            yield return item;
        }

        foreach (var item in items2)
        {
            yield return item;
        }
    }
}
