using System;
using UnityEngine;

[Serializable]
public class RCControllerCalibration
{
    public AxisInfo lStickY;
    public AxisInfo lStickX;
    public AxisInfo rStickY;
    public AxisInfo rStickX;
    public AxisInfo throttle;
    public AxisInfo yaw;
    public AxisInfo pitch;
    public AxisInfo  roll;
}