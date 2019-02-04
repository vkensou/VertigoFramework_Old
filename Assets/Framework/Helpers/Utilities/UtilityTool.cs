using System.Collections.Generic;

public static class UtilityTool
{
    public static void Resize<T>(this List<T> list, int num)
    {
        if (list.Count > num)
        {
            list.RemoveRange(num, list.Count - num);
        }
        else if (list.Count < num)
        {
            list.AddRange(new T[num - list.Count]);
        }
    }
}
