using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SentenceNode : Node
{

    private Sentence _sentence = default;
    public Sentence Sentence
    {
        get => _sentence;
        set => _sentence = value;
    }


    public override string ToString()
    {
        return $"Sentence {Id}";
    }
}
