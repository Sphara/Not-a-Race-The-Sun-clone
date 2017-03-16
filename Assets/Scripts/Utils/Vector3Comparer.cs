using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Vector3Comparer : IEqualityComparer<Vector3>
{
    public bool Equals(Vector3 x, Vector3 y)
    {
        return x.x == y.x && x.y == y.y && x.z == y.z;
    }

    public int GetHashCode(Vector3 obj)
    {
        return obj.x.GetHashCode() ^ obj.y.GetHashCode() << 2 ^ obj.z.GetHashCode() >> 2;
    }
}