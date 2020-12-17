using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EditorExtensions
{
    public static string Cut(this string text, int length)
    {
        return text.Length > length ?
            text.Substring(0, length - 2) + "..." :
            text;
    }
}
