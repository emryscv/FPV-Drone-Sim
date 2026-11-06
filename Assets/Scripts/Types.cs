using System.Collections;
using UnityEngine;

public enum RateType
{
    Betaflight,
    Actual
}

public enum FlightMode
{
    Acro,
    Angle,
    Horizon
}

public delegate float RateEquation(int axis, float input);

public delegate Coroutine StartCoroutine(IEnumerator routine);

public delegate void StopCoroutine(Coroutine coroutine);

public delegate void OnUndo();
