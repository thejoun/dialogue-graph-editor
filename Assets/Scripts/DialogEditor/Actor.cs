using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Actor : MonoBehaviour
{
    [SerializeField]
    private string _title;
    public string Title
    {
        get => _title;
        set => _title = value;
    }
}
