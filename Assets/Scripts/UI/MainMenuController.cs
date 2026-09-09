using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    private RCControllerManager _physicalRCController;

    private UIDocument _uiManager;
    private VisualElement _MainMenu;
    private VisualElement _RCControllerMenu; 

    private Button _startFlightNav;
    private Button _tutorialNav;
    private Button _rcControllerNav;
    private Button _droneConfigurationNav;
    private Button _fcConfigurationNav;
    private Button _settingsNav;
    private Button _exit;

    private Image _controllerStatusIcon;
    private Label _controllerStatusLabel;


    [SerializeField] private VectorImage controller; 
    [SerializeField] private VectorImage noController; 

    private void OnEnable()
    {
        _physicalRCController = RCControllerManager.Instance;

        _uiManager = GetComponent<UIDocument>();
        _MainMenu = _uiManager.rootVisualElement.Q<VisualElement>("MainMenu");
        _RCControllerMenu = _uiManager.rootVisualElement.Q<VisualElement>("RCControllerMenu");

        _startFlightNav        = _MainMenu.Q<Button>("StartFlightButton");
        _tutorialNav           = _MainMenu.Q<Button>("TutorialButton");
        _rcControllerNav       = _MainMenu.Q<Button>("RCControllerButton");
        _droneConfigurationNav = _MainMenu.Q<Button>("DroneButton");
        _fcConfigurationNav    = _MainMenu.Q<Button>("FCButton");
        _settingsNav           = _MainMenu.Q<Button>("SettingsButton");
        _exit                  = _MainMenu.Q<Button>("ExitButton");

        _controllerStatusIcon  = _MainMenu.Q<Image>("StatusIcon");
        _controllerStatusLabel = _MainMenu.Q<Label>("StatusLabel");

        _startFlightNav.RegisterCallback<ClickEvent>(OnStartFlightBtnClick);
        _rcControllerNav.RegisterCallback<ClickEvent>(OnRCControllerBtnClick);
    }

    private void Update()
    {
        if(_physicalRCController.registeredDevice != null){
            _controllerStatusLabel.text = _physicalRCController.registeredDevice.displayName;
            _controllerStatusIcon.vectorImage = controller;
        }else{
            _controllerStatusLabel.text = "No Controller Connected";
            _controllerStatusIcon.vectorImage = noController;
        }
    }

    private void OnDisable()
    {
        _rcControllerNav.UnregisterCallback<ClickEvent>(OnRCControllerBtnClick);  
    } 

    private void OnStartFlightBtnClick(ClickEvent evt)
    {
        _MainMenu.visible = false;
        _RCControllerMenu.visible = false;
    }

    private void OnRCControllerBtnClick(ClickEvent evt)
    {
        _MainMenu.visible = false;
        _RCControllerMenu.visible = true;
    }
}
