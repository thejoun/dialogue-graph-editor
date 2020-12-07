using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DialogHelper
{
    public static string Cut(string text, int length)
    {
        string cutText = text;
        if (text.Length > length)
            cutText = text.Substring(0, length - 2) + "...";
        return cutText;
    }
}
