using System;
using UnityEngine.InputSystem.Controls;

[Serializable]
public class AxisInfo
{
    public string path;
    public float min;
    public float center;
    public float max;
    public bool inverted;

    //Axis Discovery Variables
    [NonSerialized] public int count;
    [NonSerialized] public float lastValue;
    [NonSerialized] public AxisControl axis;

    public AxisInfo(string path, float min, float center, float max, bool inverted, int count, float lastValue, AxisControl axis)
    {
        this.path = path;
        this.min = min;
        this.center = center;
        this.max = max;
        this.inverted = inverted;
        this.count = count;
        this.lastValue = lastValue;
        this.axis = axis;
    }

    public float ReadValue()
    {
        if (axis == null) return 0f;

        float value = axis.ReadValue();
        value = 2f * (value - min) / (max - min) - 1f;

        return inverted ? -value : value;
    }
}