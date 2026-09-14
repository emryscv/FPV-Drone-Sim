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

    public AxisInfo _leftStickY { get; private set; }
    public AxisInfo _leftStickX { get; private set; }
    public AxisInfo _rightStickY { get; private set; }
    public AxisInfo _rightStickX { get; private set; }

    public AxisInfo _throttle { get; private set; }
    public AxisInfo _yaw { get; private set; }
    public AxisInfo _roll { get; private set; }
    public AxisInfo _pitch { get; private set; }

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
            _leftStickY = _tempDetectedAxis;
        }

        Debug.Log("El animaah");
    }

    public void SetLeftStickX()
    {
        if (_tempDetectedAxis != null)
        {
            _leftStickX = _tempDetectedAxis;
        }
    }

    public void SetRightStickY()
    {
        if (_tempDetectedAxis != null)
        {
            _rightStickY = _tempDetectedAxis;
        }
    }

    public void SetRightStickX()
    {
        if (_tempDetectedAxis != null)
        {
            _rightStickX = _tempDetectedAxis;
        }
    }

    public void SetThrottle()
    {
        if (_tempDetectedAxis != null)
        {
            _throttle = _tempDetectedAxis;
        }
    }

    public void SetYaw()
    {
        if (_tempDetectedAxis != null)
        {
            _yaw = _tempDetectedAxis;
        }
    }

    public void SetPitch()
    {
        if (_tempDetectedAxis != null)
        {
            _pitch = _tempDetectedAxis;
        }
    }

    public void SetRoll()
    {
        if (_tempDetectedAxis != null)
        {
            _roll = _tempDetectedAxis;
        }
    }

    public void PrintDebug()
    {
        Debug.Log("!!!!!!!!!!!DEBUG START!!!!!!!!!!!");
        Debug.Log("Left Stick Y: " + (_leftStickY != null ? _leftStickY.axis.path : "Not assigned"));
        Debug.Log("Left Stick X: " + (_leftStickX != null ? _leftStickX.axis.path : "Not assigned"));
        Debug.Log("Right Stick Y: " + (_rightStickY != null ? _rightStickY.axis.path : "Not assigned"));
        Debug.Log("Right Stick X: " + (_rightStickX != null ? _rightStickX.axis.path : "Not assigned"));
        Debug.Log("Throttle: " + (_throttle != null ? _throttle.axis.path : "Not assigned"));
        Debug.Log("Yaw: " + (_yaw != null ? _yaw.axis.path : "Not assigned"));
        Debug.Log("Pitch: " + (_pitch != null ? _pitch.axis.path : "Not assigned"));
        Debug.Log("Roll: " + (_roll != null ? _roll.axis.path : "Not assigned"));
    }

    public void SaveCalibration()
    {
        RCControllerCalibration calibration = new()
        {
            leftStickY = _leftStickY,
            leftStickX = _leftStickX,
            rightStickY = _rightStickY,
            rightStickX = _rightStickX,
            throttle = _throttle,
            yaw = _yaw,
            pitch = _pitch,
            roll = _roll
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
            _leftStickY = calibration.leftStickY;
            _leftStickX = calibration.leftStickX;
            _rightStickY = calibration.rightStickY;
            _rightStickX = calibration.rightStickX;

            _throttle = calibration.throttle;
            _yaw = calibration.yaw;
            _pitch = calibration.pitch;
            _roll = calibration.roll;

            _leftStickY.axis = registeredDevice.TryGetChildControl<AxisControl>(calibration.leftStickY.path.Split('/', 3)[2]);
            _leftStickX.axis = registeredDevice.TryGetChildControl<AxisControl>(calibration.leftStickX.path.Split('/', 3)[2]);
            _rightStickY.axis = registeredDevice.TryGetChildControl<AxisControl>(calibration.rightStickY.path.Split('/', 3)[2]);
            _rightStickX.axis = registeredDevice.TryGetChildControl<AxisControl>(calibration.rightStickX.path.Split('/', 3)[2]);
           
            _throttle.axis = registeredDevice.TryGetChildControl<AxisControl>(calibration.throttle.path.Split('/', 3)[2]);
            _yaw.axis = registeredDevice.TryGetChildControl<AxisControl>(calibration.yaw.path.Split('/', 3)[2]);
            _pitch.axis = registeredDevice.TryGetChildControl<AxisControl>(calibration.pitch.path.Split('/', 3)[2]);
            _roll.axis = registeredDevice.TryGetChildControl<AxisControl>(calibration.roll.path.Split('/', 3)[2]);
        }
    }
}


