using System.Collections;
using UnityEngine;

public enum RateType
{
    Betaflight,
    Actual
}

public delegate float RateEquation(int axis, float input);

public delegate Coroutine StartCoroutine(IEnumerator routine);

public delegate void StopCoroutine(Coroutine coroutine);

public delegate void OnUndo();

public struct Map
{
    public string name;
    public Texture2D thumbnail;
}