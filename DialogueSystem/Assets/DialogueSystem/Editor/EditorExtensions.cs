namespace DialogueSystem.Editor
{
    /// <summary>
    /// Extension methods for Editor classes
    /// </summary>
    public static class EditorExtensions
    {
        // Cut a string to a specified length
        public static string Cut(this string text, int maxLength)
        {
            if (text == null) return "";
            
            return text.Length > maxLength ?
                text.Substring(0, maxLength - 2) + "..." :
                text;
                
        }
    }
}