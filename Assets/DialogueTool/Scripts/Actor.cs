using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "Actor", menuName = "Actor", order = 1)]
public class Actor : ScriptableObject
{
    [SerializeField]
    private string _title;
    public string Title
    {
        get => _title;
        set => _title = value;
    }
}
