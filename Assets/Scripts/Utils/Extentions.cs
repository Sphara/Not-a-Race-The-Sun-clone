using UnityEngine;
using System.Collections.Generic;

public static class Extentions {

    public static Vector3 Range(this Random rand, Vector3 min, Vector3 max)
    {
        return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
    }

    public static Vector3 StringToVector3(this string source)
    {
        Vector3 res;

        string[] tmp = source.Substring(1, source.Length - 2).Split(',');

        float x = float.Parse(tmp[0]);
        float y = float.Parse(tmp[1]);
        float z = float.Parse(tmp[2]);

        res = new Vector3(x, y, z);

        return res;
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
