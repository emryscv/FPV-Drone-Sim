using System;
using UnityEngine;

[Serializable]
public class RCControllerCalibration
{
    public AxisInfo leftStickY;
    public AxisInfo leftStickX;
    public AxisInfo rightStickY;
    public AxisInfo rightStickX;
    public AxisInfo throttle;
    public AxisInfo yaw;
    public AxisInfo pitch;
    public AxisInfo  roll;
}