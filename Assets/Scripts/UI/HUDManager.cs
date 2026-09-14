using UnityEngine;
using UnityEngine.UIElements;

public class HUDManager : MonoBehaviour
{

    [SerializeField] private Rigidbody _rb;

    private Controls controls;

    private UIDocument _uiManager;
    private VisualElement _HUD;

    private Label _speed;
    private Label _altitude;

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
        _speed = _HUD.Q<Label>("Speed");
        _altitude = _HUD.Q<Label>("Altitude");

        Debug.Log("Speed: " + _speed);
        Debug.Log("Altitude: " + _altitude);
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

        float altitude = _rb.transform.position.y * 3.281f; //ft
        float speed = _rb.linearVelocity.magnitude * 2.237f; //mph

        _speed.text = speed.ToString("F2");
        _altitude.text = altitude.ToString("F2");
    }
}
