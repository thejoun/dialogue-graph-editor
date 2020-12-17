using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Node
{
    private static readonly int default_width = 50;
    private static readonly int default_height = 50;

    public Node()
    {
        _sentence = new Sentence();
    }

    /*
    [SerializeField]
    private float _currentScale = 1f;
    public float Scale
    {
        get => _currentScale;
        private set => _currentScale = value;
    }

    public void SetScale(float scale)
    {
        Scale = scale;
        //Rect = new Rect(Rect.position * Scale, Rect.size * Scale);
    }
    */

    [SerializeField]
    private bool _deletedFlag;
    public bool IsDeleted
    {
        get => _deletedFlag;
        set => _deletedFlag = value;
    }

    [SerializeField]
    private Sentence _sentence;
    public Sentence Sentence
    {
        get => _sentence;
        set => _sentence = value;
    }

    [SerializeField]
    private Rect _rect = new Rect(0, 0, default_width, default_height);
    public Rect Rect
    {
        private get => _rect != null ? _rect : new Rect(0, 0, default_width, default_height);
        set => _rect = value;
    }

    // Only for showing in the graph editor. Not persistent.
    public float Scale { get; set; }
    public Vector2 Position { get; set; }
    public Rect WindowRect
    {
        get
        {
            Rect rect = new Rect(Rect);
            rect.position *= Scale;
            rect.position += Position;
            rect.size *= Scale;
            return rect;
        }
        set
        {
            Rect rect = new Rect(value);
            rect.position -= Position;
            rect.position /= Scale;
            rect.size /= Scale;
            _rect = rect;
        }
    }

    /*
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
    */


    [SerializeField]
    private int _id;
    public int Id
    {
        get => _id;
        set => _id = value;
    }

    public void SoftDelete()
    {
        _deletedFlag = true;
    }

    

    public override string ToString()
    {
        return $"Node {Id}";
    }
}
