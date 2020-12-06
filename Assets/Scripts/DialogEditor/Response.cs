using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

/// <summary>
/// Class representing a dialog response.
/// </summary>
[Serializable]
public class Response
{
    public string Text { get; set; } = default;

    // Index of the next dialog node to go to next if this response was selected
    public int NextId { get; set; } = -1;

    // The name of the function this response will trigger.
    public string Trigger { get; set; } = default;


    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"DIALOG RESPONSE");
        sb.Append($"Text: {Text}");
        sb.Append($"NextId: {NextId}");
        sb.Append($"Trigger: {Trigger}");
        return sb.ToString();
    }
}
