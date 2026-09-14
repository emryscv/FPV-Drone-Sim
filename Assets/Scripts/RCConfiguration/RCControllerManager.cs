using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class RCControllerManager : MonoBehaviour
{
    private const float AXIS_DETECTION_THRESHOLD = 0.8f;

    public static RCControllerManager Instance { get; private set; }

    public InputDevice registeredDevice { get; private set; }

    private List<AxisInfo> _allAxesInfo;
    private AxisInfo[] _axesInfo; //This one will contain the 4 needed axis only;
    private AxisInfo _tempDetectedAxis;

    public AxisInfo LeftStickY { get; private set; }
    public AxisInfo LeftStickX { get; private set; }
    public AxisInfo RightStickY { get; private set; }
    public AxisInfo RightStickX { get; private set; }

    public AxisInfo Throttle { get; private set; }
    public AxisInfo Yaw { get; private set; }
    public AxisInfo Roll { get; private set; }
    public AxisInfo Pitch { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
        DontDestroyOnLoad(this);

        _allAxesInfo = new List<AxisInfo>();
;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;

        foreach (InputDevice device in InputSystem.devices)
        {
            if (IsRCController(device))
            {

                Debug.Log("Device found: " + device.name + " with ID: " + device.deviceId + " display name: " + device.displayName);
                registeredDevice = device;
                break;
            }
        }

        LoadCalibration();
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Added && IsRCController(device)) {
            registeredDevice = device;
            LoadCalibration();
        }
        else if (change == InputDeviceChange.Removed && IsRCController(device))
        {
            registeredDevice = null;
        }
    }

    private bool IsRCController(InputDevice device)
    {
        return device is Joystick || device is Gamepad;
    }

    public void PruneAxes()
    {
        if (_allAxesInfo == null || _allAxesInfo.Count == 0)
        {
            _axesInfo = new AxisInfo[0];
            return;
        }

        _axesInfo = _allAxesInfo.OrderByDescending(a => a.count).Take(4).ToArray();
    }

    public void FindAllAxes()
    {
        if (registeredDevice == null)
        {
            Debug.LogWarning("No registered controller found while discovering axes.");
            return;
        }

        _allAxesInfo.Clear();

        foreach (InputControl control in registeredDevice.allControls)
        {
            if (control is not AxisControl axis) continue;
            if (axis.synthetic || axis.noisy) continue;

            float value = axis.ReadValue();
            _allAxesInfo.Add(new AxisInfo(axis.path, value, 0f, value, false, 0, value, axis));
        }
    }

    public void ReadAllAxes()
    {
        foreach (AxisInfo axisInfo in _allAxesInfo)
        {
            float value = axisInfo.axis.ReadValue();

            if (axisInfo.lastValue != value)
            {
                axisInfo.lastValue = value;
                axisInfo.count++;
                axisInfo.min = Mathf.Min(axisInfo.min, value);
                axisInfo.max = Mathf.Max(axisInfo.max, value);
            }
            Debug.Log("Axis: " + axisInfo.axis.path + " Value: " + value + " Min: " + axisInfo.min + " Max: " + axisInfo.max + " Count: " + axisInfo.count);
        }
    }

    public bool TryReadAxis(float threshold = AXIS_DETECTION_THRESHOLD)
    {
        if (_axesInfo == null || _axesInfo.Length == 0)
        {
            return false;
        }

        foreach (AxisInfo axisInfo in _axesInfo)
        {
            float value = axisInfo.axis.ReadValue();
            Debug.Log("Read Axis: " + axisInfo.axis.path + " Value: " + value);

            if (Mathf.Abs(value) >= threshold)
            {
                _tempDetectedAxis = axisInfo;
                Debug.Log("Found Axis: " + axisInfo.axis.path + " Value: " + value);
                return true;
            }
        }

        return false;
    }

    public void SetLeftStickY()
    {
        if (_tempDetectedAxis != null)
        {
            LeftStickY = _tempDetectedAxis;
        }

        Debug.Log("El animaah");
    }

    public void SetLeftStickX()
    {
        if (_tempDetectedAxis != null)
        {
            LeftStickX = _tempDetectedAxis;
        }
    }

    public void SetRightStickY()
    {
        if (_tempDetectedAxis != null)
        {
            RightStickY = _tempDetectedAxis;
        }
    }

    public void SetRightStickX()
    {
        if (_tempDetectedAxis != null)
        {
            RightStickX = _tempDetectedAxis;
        }
    }

    public void SetThrottle()
    {
        if (_tempDetectedAxis != null)
        {
            Throttle = _tempDetectedAxis;
        }
    }

    public void SetYaw()
    {
        if (_tempDetectedAxis != null)
        {
            Yaw = _tempDetectedAxis;
        }
    }

    public void SetPitch()
    {
        if (_tempDetectedAxis != null)
        {
            Pitch = _tempDetectedAxis;
        }
    }

    public void SetRoll()
    {
        if (_tempDetectedAxis != null)
        {
            Roll = _tempDetectedAxis;
        }
    }

    public void PrintDebug()
    {
        Debug.Log("!!!!!!!!!!!DEBUG START!!!!!!!!!!!");
        Debug.Log("Left Stick Y: " + (LeftStickY != null ? LeftStickY.axis.path : "Not assigned"));
        Debug.Log("Left Stick X: " + (LeftStickX != null ? LeftStickX.axis.path : "Not assigned"));
        Debug.Log("Right Stick Y: " + (RightStickY != null ? RightStickY.axis.path : "Not assigned"));
        Debug.Log("Right Stick X: " + (RightStickX != null ? RightStickX.axis.path : "Not assigned"));
        Debug.Log("Throttle: " + (Throttle != null ? Throttle.axis.path : "Not assigned"));
        Debug.Log("Yaw: " + (Yaw != null ? Yaw.axis.path : "Not assigned"));
        Debug.Log("Pitch: " + (Pitch != null ? Pitch.axis.path : "Not assigned"));
        Debug.Log("Roll: " + (Roll != null ? Roll.axis.path : "Not assigned"));
    }

    public void SaveCalibration()
    {
        RCControllerCalibration calibration = new()
        {
            leftStickY = LeftStickY,
            leftStickX = LeftStickX,
            rightStickY = RightStickY,
            rightStickX = RightStickX,
            throttle = Throttle,
            yaw = Yaw,
            pitch = Pitch,
            roll = Roll
        };

        string path = Application.persistentDataPath + "/rc_calibration.json";
        string json = JsonUtility.ToJson(calibration);
        File.WriteAllText(path, json);

        Debug.Log("Calibration saved to: " + path);
    }

    private void LoadCalibration()
    {
        string path = Application.persistentDataPath + "/rc_calibration.json";
        if (!File.Exists(path)) return;

        string json = File.ReadAllText(path);
        RCControllerCalibration calibration = JsonUtility.FromJson<RCControllerCalibration>(json);

        if (registeredDevice != null)
        {
            Debug.Log("Registered device found. Applying calibration...");
            LeftStickY = calibration.leftStickY;
            LeftStickX = calibration.leftStickX;
            RightStickY = calibration.rightStickY;
            RightStickX = calibration.rightStickX;

            Throttle = calibration.throttle;
            Yaw = calibration.yaw;
            Pitch = calibration.pitch;
            Roll = calibration.roll;

            LeftStickY.axis = registeredDevice.TryGetChildControl<AxisControl>(calibration.leftStickY.path.Split('/', 3)[2]);
            LeftStickX.axis = registeredDevice.TryGetChildControl<AxisControl>(calibration.leftStickX.path.Split('/', 3)[2]);
            RightStickY.axis = registeredDevice.TryGetChildControl<AxisControl>(calibration.rightStickY.path.Split('/', 3)[2]);
            RightStickX.axis = registeredDevice.TryGetChildControl<AxisControl>(calibration.rightStickX.path.Split('/', 3)[2]);
           
            Throttle.axis = registeredDevice.TryGetChildControl<AxisControl>(calibration.throttle.path.Split('/', 3)[2]);
            Yaw.axis = registeredDevice.TryGetChildControl<AxisControl>(calibration.yaw.path.Split('/', 3)[2]);
            Pitch.axis = registeredDevice.TryGetChildControl<AxisControl>(calibration.pitch.path.Split('/', 3)[2]);
            Roll.axis = registeredDevice.TryGetChildControl<AxisControl>(calibration.roll.path.Split('/', 3)[2]);
        }
    }
}


