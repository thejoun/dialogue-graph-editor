using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Helper class for reading/writing Lists from/to json files via the JsonUtility class.
/// </summary>
public static class JsonHelper
{
    public static List<T> FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.conversation;
    }

    public static string ToJson<T>(List<T> array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.conversation = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(List<T> array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.conversation = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public List<T> conversation;
    }

} // end JsonHelper class
