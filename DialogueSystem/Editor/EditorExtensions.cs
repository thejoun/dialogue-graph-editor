using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    /// <summary>
    /// Extension methods for Editor classes
    /// </summary>
    public static class EditorExtensions
    {
        // Cut a string to a specified length
        public static string Cut(this string text, int length)
        {
            if (text == null)
                return "";
            else
                return text.Length > length ?
                    text.Substring(0, length - 2) + "..." :
                    text;
        }
    }
}