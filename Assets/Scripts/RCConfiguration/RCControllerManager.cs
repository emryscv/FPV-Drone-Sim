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

        LoadCalibration();
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
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Added && IsRCController(device)) registeredDevice = device;

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
        var calibration = new RCControllerCalibration
        {
            leftStickYPath = _leftStickY?.axis.path ?? "",
            leftStickXPath = _leftStickX?.axis.path ?? "",
            rightStickYPath = _rightStickY?.axis.path ?? "",
            rightStickXPath = _rightStickX?.axis.path ?? "",
            throttlePath = _throttle?.axis.path ?? "",
            yawPath = _yaw?.axis.path ?? "",
            pitchPath = _pitch?.axis.path ?? "",
            rollPath = _roll?.axis.path ?? ""
        };

        string path = Application.persistentDataPath + "/rc_calibration.json";
        string json = JsonUtility.ToJson(calibration);
        File.WriteAllText(path, json);
    }

    private void LoadCalibration()
    {
        string path = Application.persistentDataPath + "/rc_calibration.json";
        if (!File.Exists(path)) return;

        string json = File.ReadAllText(path);
        var calibration = JsonUtility.FromJson<RCControllerCalibration>(json);
    }
}


