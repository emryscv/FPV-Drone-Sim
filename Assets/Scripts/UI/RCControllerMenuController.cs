using UnityEngine;
using UnityEngine.UIElements;

public class RCControllerMenuController : MonoBehaviour
{
    private RCControllerManager _physicalRCController;
    private Controls controls;

    private UIDocument _uiManager;
    private VisualElement _MainMenu;
    private VisualElement _RCControllerMenu; 

    private Label _controllerStatusLabel;
    private VisualElement _leftStick;
    private VisualElement _rightStick;

    private Button _backBtn;
    private Button _saveBtn;
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
        _controllerStatusLabel.text = _physicalRCController.ControllerName != null ?  _physicalRCController.ControllerName : _controllerStatusLabel.text = "NO CONTROLLER DETECTED";        
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
