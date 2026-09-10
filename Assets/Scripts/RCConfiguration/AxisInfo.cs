using UnityEngine.InputSystem.Controls;

public class AxisInfo
{
    public string path { get; set; }
    public float min { get; set; }
    public float center { get; set; }
    public float max { get; set; }
    public bool inverted { get; set; }

    //Axis Discovery Variables
    public int count { get; set; }
    public float lastValue { get; set; }


    public AxisControl axis { get; set; }

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
}