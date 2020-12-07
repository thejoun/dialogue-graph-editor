using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ResponseNode : Node
{

    private Response _response = default;
    public Response Response
    {
        get => _response;
        set => _response = value;
    }

    public override string ToString()
    {
        return $"Response {Id}";
    }
}
