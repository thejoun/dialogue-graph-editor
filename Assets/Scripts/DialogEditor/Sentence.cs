
using System.Collections.Generic;
using System;
using System.Text;


[Serializable]
public class Sentence
{
    [Serializable]
    public enum SentenceType
    {
        Default, Start, End
    }

    public Actor Actor { get; set; } = default;

    public string Text { get; set; } = default;

    public SentenceType Type { get; set; } = default;

    public List<Response> Responses { get; set; } = new List<Response>();
}
