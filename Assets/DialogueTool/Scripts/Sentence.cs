using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;

[Serializable]
public class Sentence
{
    public Sentence()
    {
        _responses = new List<Response>();
        _text = "";
    }


    [Serializable]
    public enum SentenceType
    {
        Default, Start, End
    }


    [SerializeField]
    private Actor _actor;
    public Actor Actor
    {
        get => _actor;
        set => _actor = value;
    }

    [SerializeField]
    private string _text;
    public string Text
    {
        get => _text;
        set => _text = value;
    }

    [SerializeField]
    private SentenceType _type;
    public SentenceType Type
    {
        get => _type;
        set => _type = value;
    }

    [SerializeField]
    private List<Response> _responses;
    public List<Response> Responses
    {
        get => _responses;
    }


    public Response GetResponse(int id) => Responses[id];

    public Response FirstResponse => GetResponse(0);

    public bool HasResponseChoice => Responses.Count != 0 && !FirstResponse.IsEmpty;

    public bool HasNextSentence => Responses.Count == 1 && FirstResponse.IsEmpty;

    public bool IsDialogueEnd => Responses.Count == 0;
}
