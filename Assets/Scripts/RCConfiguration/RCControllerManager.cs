using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class RCControllerManager : MonoBehaviour
{
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

        public AxisInfo(string path, float min, float center, float max, bool inverted, int count, float lastValue)
        {
            this.path = path;
            this.min = min;
            this.center = center;
            this.max = max;
            this.inverted = inverted;
            this.count = count;
            this.lastValue = lastValue;

        }
    }

    public static RCControllerManager Instance { get; private set; }

    public InputDevice registeredDevice { get; private set; }
    private Dictionary<string, AxisInfo> _axesInfo;
    public AxisInfo[] _axesInfoArray; //This one will contain the 4 needed axis only;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
        DontDestroyOnLoad(this);

        _axesInfo = new Dictionary<string, AxisInfo>();
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

    public void PruneAxes(){
        _axesInfoArray = _axesInfo.Values.ToArray();
        _axesInfoArray = _axesInfoArray.OrderByDescending(a => a.count).ToArray();
        _axesInfoArray = _axesInfoArray.Take(4).ToArray();
    }

    public void LogAllAxes()
    {
        foreach (InputControl control in registeredDevice.allControls)
        {
            if (control is not AxisControl axis) continue;
            if (axis.synthetic || axis.noisy) continue;

            float value = axis.ReadValue();
        
            if (!_axesInfo.ContainsKey(axis.path))
            {
                _axesInfo[axis.path] = new AxisInfo(axis.path, value, 0f, value, false, 0, value);
            }
            else{
                if(_axesInfo[axis.path].lastValue != value)
                {
                    _axesInfo[axis.path].lastValue = value;
                    _axesInfo[axis.path].count++;
                    _axesInfo[axis.path].min = Mathf.Min(_axesInfo[axis.path].min, value);
                    _axesInfo[axis.path].max = Mathf.Max(_axesInfo[axis.path].max, value);
                }
            }

            Debug.Log("Axis: " + axis.path + " Value: " + value + " Min: " + _axesInfo[axis.path].min + " Max: " + _axesInfo[axis.path].max + " Count: " + _axesInfo[axis.path].count);
        }
    }
}
