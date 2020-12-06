using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Class to represent a dialog node in the dialog editor window. 
/// Basically a GUI capsule for the DialodNode class.
/// </summary>
[Serializable]
public class SentenceNode
{
    public enum NodeType
    {
        Default, Start, End
    }

    private static readonly int default_width = 50;
    private static readonly int default_height = 50;


    private Vector2 _position;
    public Vector2 Pos
    {
        get => _position;
        set
        {
            _position = value;
            _rect.position = _position;
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


    private Sentence _content = default;
    public Sentence Content
    {
        get => _content;
        set => _content = value;
    }


    public string Text
    {
        get => _content.Text;
        set
        {
            if (_content != default)
                _content.Text = value;
            else
                _content = new Sentence() { Text = value };
        }
    }


    private Rect _rect = new Rect(0, 0, default_width, default_height);
    public Rect Rect
    {
        get => _rect;
        set => _rect = value;
    }


    public NodeType Type { get; set; } = 0;
}
