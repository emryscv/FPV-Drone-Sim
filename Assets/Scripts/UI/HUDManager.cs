using UnityEngine;
using UnityEngine.UIElements;

public class HUDManager : MonoBehaviour
{
    private Controls controls;

    private UIDocument _uiManager;
    private VisualElement _HUD;
    private VisualElement _leftStick;
    private VisualElement _rightStick;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        controls = new Controls();
        controls.RCController.Enable();
    }

    private void OnEnable()
    {
        
        _uiManager = GetComponent<UIDocument>();
        _HUD = _uiManager.rootVisualElement.Q<VisualElement>("HUD");

        _leftStick = _HUD.Q<VisualElement>("LeftStick");
        _rightStick = _HUD.Q<VisualElement>("RightStick");

        Debug.Log("HUD Initialized");
        Debug.Log("Left Stick: " + _leftStick);
    }
    // Update is called once per frame
    void Update()
    {
        float throttle = controls.RCController.Throttle.ReadValue<float>();
        float yaw = controls.RCController.Yaw.ReadValue<float>();
        float pitch = controls.RCController.Pitch.ReadValue<float>();
        float roll = controls.RCController.Roll.ReadValue<float>();

        //This equation is fixed to the size of the parent componenet. If the size change this has to change
        _leftStick.style.left = yaw * 65f + 65f;
        _leftStick.style.top = 65f - throttle * 65f;

        _rightStick.style.left = roll * 65f + 65f;
        _rightStick.style.top = 65f - pitch * 65f;
    }
}
