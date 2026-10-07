using UnityEngine;
using UnityEngine.UIElements;

public class HUDManager : MonoBehaviour
{

    [SerializeField] private Rigidbody _rb;

    private RCControllerManager controls;

    private UIDocument _uiManager;
    private VisualElement _HUD;
    
    private VisualElement _crashIndicator;

    private Label _speed;
    private Label _altitude;

    private VisualElement _leftStick;
    private VisualElement _rightStick;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        controls = RCControllerManager.Instance;
        GameEvents.Instance.OnDisplayCrashIndicator += ShowCrashIndicator;
        GameEvents.Instance.OnHideCrashIndicator += HideCrashIndicator;
    }

    private void OnEnable()
    {

        _uiManager = GetComponent<UIDocument>();
        _HUD = _uiManager.rootVisualElement.Q<VisualElement>("HUD");

        _leftStick = _HUD.Q<VisualElement>("LeftStick");
        _rightStick = _HUD.Q<VisualElement>("RightStick");

        Debug.Log("HUD Initialized");
        _speed = _HUD.Q<Label>("Speed");
        _altitude = _HUD.Q<Label>("Altitude");
        _crashIndicator = _HUD.Q<VisualElement>("CrashBadge");

        Debug.Log("Speed: " + _speed);
        Debug.Log("Altitude: " + _altitude);
        Debug.Log("Crash Indicator: " + _crashIndicator);


    }
    // Update is called once per frame
    void Update()
    {
        float lStickY = controls.LStickY?.ReadValue() ?? 0f;
        float lStickX = controls.LStickX?.ReadValue() ?? 0f;
        float rStickY = controls.RStickY?.ReadValue() ?? 0f;
        float rStickX = controls.RStickX?.ReadValue() ?? 0f;

        //This equation is fixed to the size of the parent componenet. If the size change this has to change
        _leftStick.style.left = lStickX * 65f + 65f;
        _leftStick.style.top = 65f - lStickY * 65f;

        _rightStick.style.left = rStickX * 65f + 65f;
        _rightStick.style.top = 65f - rStickY * 65f;

        float altitude = _rb.transform.position.y * 3.281f; //ft
        float speed = _rb.linearVelocity.magnitude * 2.237f; //mph

        _speed.text = speed.ToString("F2");
        _altitude.text = altitude.ToString("F2");
    }

    private void ShowCrashIndicator()
    {
        _crashIndicator.style.display = DisplayStyle.Flex;
    }

    private void HideCrashIndicator()
    {
        _crashIndicator.style.display = DisplayStyle.None;
    }
}
