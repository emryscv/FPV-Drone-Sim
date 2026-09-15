using UnityEngine;
using UnityEngine.UIElements;

public class HUDManager : MonoBehaviour
{

    [SerializeField] private Rigidbody _rb;

    private RCControllerManager controls;

    private UIDocument _uiManager;
    private VisualElement _HUD;

    private Label _speed;
    private Label _altitude;

    private VisualElement _leftStick;
    private VisualElement _rightStick;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        controls = RCControllerManager.Instance;
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

        Debug.Log("Speed: " + _speed);
        Debug.Log("Altitude: " + _altitude);
    }
    // Update is called once per frame
    void Update()
    {
        float lStickY = controls.LStickY.inverted ? -controls.LStickY.axis.ReadValue() : controls.LStickY.axis.ReadValue();
        float lStickX = controls.LStickX.inverted ? -controls.LStickX.axis.ReadValue() : controls.LStickX.axis.ReadValue();
        float rStickY = controls.RStickY.inverted ? -controls.RStickY.axis.ReadValue() : controls.RStickY.axis.ReadValue();
        float rStickX = controls.RStickX.inverted ? -controls.RStickX.axis.ReadValue() : controls.RStickX.axis.ReadValue();

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
}
