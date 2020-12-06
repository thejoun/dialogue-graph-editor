using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Struct representing two dimensions, with implicit conversion to Vector2.
// Has integer values and a Vector2 implicit cast.
public struct Dimensions
{
    public int X { get; set; }
    public int Y { get; set; }

    public Dimensions(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Dimensions(float x, float y)
    {
        X = Mathf.RoundToInt(x);
        Y = Mathf.RoundToInt(y);
    }

    public static implicit operator Vector2(Dimensions d) => new Vector2(d.X, d.Y);

    public static Dimensions operator /(Dimensions d, float f) => new Dimensions(d.X / f, d.Y / f);
}
