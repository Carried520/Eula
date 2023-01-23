namespace Eula.ListExtensions;

public static class Order
{


    public static void Shuffle<T>(this IList<T> list)
    {
        var rng = new Random();
        int count = list.Count;
        while (count > 1)
        {
            count--;
            int k = rng.Next(count + 1);
            (list[k], list[count]) = (list[count], list[k]);
        }
    }
}