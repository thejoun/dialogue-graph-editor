using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    private static readonly int default_width = 50;
    private static readonly int default_height = 50;
    


    private Rect _rect = new Rect(0, 0, default_width, default_height);
    public Rect Rect
    {
        get => _rect;
        set => _rect = value;
    }
    public Vector2 Position
    {
        set => _rect.position = value;
    }
    public Vector2 Dimensions
    {
        set
        {
            _rect.width = value.x;
            _rect.height = value.y;
        }
    }
    public int Width
    {
        set => _rect.width = value;
    }
    public int Height
    {
        set => _rect.height = value;
    }



    private int _id;
    public int Id
    {
        get => _id;
        set => _id = value;
    }
}
