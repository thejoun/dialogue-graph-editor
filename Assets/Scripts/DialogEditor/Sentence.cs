using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

/// <summary>
/// Class representing a node in a weightless, directed dialog graph
/// </summary>
[Serializable]
public class Sentence
{
    public string Actor { get; set; }

    private string _text = default;
    public string Text
    {
        get => _text;
        set => _text = value;
    }


    private List<Response> _responses = new List<Response>();
    public List<Response> Responses
    {
        get => _responses;
        set => _responses = value;
    }


    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"DIALOG NODE");
        sb.AppendLine($"Text: {Text}");
        sb.AppendLine($"Responses: ");
        Responses.ForEach(r => sb.AppendLine($"\t {r}"));
        return sb.ToString();
    }
}
