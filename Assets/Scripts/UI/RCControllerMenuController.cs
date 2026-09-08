using UnityEngine;
using UnityEngine.UIElements;

public class RCControllerMenuController : MonoBehaviour
{
    private RCControllerManager _physicalRCController;
    private Controls controls;

    private UIDocument _uiManager;
    private VisualElement _MainMenu;
    private VisualElement _RCControllerMenu; 

    private Button _backBtn;
    private Button _saveBtn;


    private VisualElement _controllerStatusIcon;
    private Label _controllerStatusLabel;
    private VisualElement _leftStick;
    private VisualElement _rightStick;

    private Color GREEN = new Color(0.2980392f, 0.6705883f, 0.2117647f);
    private Color RED = new Color(0.4039216f, 0.1019608f, 0.08627451f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        controls = new Controls();
        controls.RCController.Enable();
    }

    private void OnEnable()
    {
        _physicalRCController = RCControllerManager.Instance; //TODO I might not need this and jsut use the Controls Class

        _uiManager = GetComponent<UIDocument>();
        _MainMenu = _uiManager.rootVisualElement.Q<VisualElement>("MainMenu");
        _RCControllerMenu = _uiManager.rootVisualElement.Q<VisualElement>("RCControllerMenu");
        
        _controllerStatusIcon = _RCControllerMenu.Q<VisualElement>("StatusIcon");
        _controllerStatusLabel = _RCControllerMenu.Q<Label>("StatusLabel");
        _leftStick = _RCControllerMenu.Q<VisualElement>("LeftStick");
        _rightStick = _RCControllerMenu.Q<VisualElement>("RightStick");

        _backBtn = _RCControllerMenu.Q<Button>("BackButton");
        _saveBtn = _RCControllerMenu.Q<Button>("SaveButton");

        _backBtn.RegisterCallback<ClickEvent>(OnBackBtnClick);
    }

    private void Update()
    {
        //TODO I might not need this and just use the controls class
        if(_physicalRCController.ControllerName != null){
            _controllerStatusLabel.text = _physicalRCController.ControllerName;
            _controllerStatusLabel.style.color = GREEN;
            _controllerStatusIcon.style.backgroundColor = GREEN;
        }else{
            _controllerStatusLabel.text = "NO CONTROLLER CONNECTED";        
            _controllerStatusLabel.style.color = RED;
            _controllerStatusIcon.style.backgroundColor = RED;
        }

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

    private void OnDisable()
    {
        _backBtn.UnregisterCallback<ClickEvent>(OnBackBtnClick);  
    } 

    private void OnBackBtnClick(ClickEvent evt)
    {
        Debug.Log("El Animaah");
        _MainMenu.visible = true;
        _RCControllerMenu.visible = false;
    }

}
