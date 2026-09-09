using UnityEngine;
using UnityEngine.InputSystem;

public class RCControllerManager : MonoBehaviour
{
    public static RCControllerManager Instance { get; private set; }

    public string ControllerName { get; private set; }
    public int ControllerId { get; private set; } = -1;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
        DontDestroyOnLoad(this);
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
                RegisterController(device);
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
        if (change == InputDeviceChange.Added && IsRCController(device)) RegisterController(device);

        else if (change == InputDeviceChange.Removed && IsRCController(device))
        {
            Debug.Log("Controller disconnected: " + ControllerName + " with ID: " + ControllerId);
            ControllerName = null;
            ControllerId = -1;
        }
    }
    
    private bool IsRCController(InputDevice device)
    {
        return device is Joystick || device is Gamepad;
    }

    private void RegisterController(InputDevice device)
    {
        ControllerName = device.displayName;
        ControllerId = device.deviceId;
        Debug.Log("Controller connected: " + ControllerName + " with ID: " + ControllerId);
    }
}
