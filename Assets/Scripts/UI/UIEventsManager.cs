using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
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
    
    private void Awake()
    {
        _uiManager = GetComponent<UIDocument>();
        _MainMenu = _uiManager.rootVisualElement.Q<VisualElement>("MainMenu");
        _RCControllerMenu = _uiManager.rootVisualElement.Q<VisualElement>("RCControllerMenu");
        

        _startFlightNav        = _MainMenu.Q("StartFlightButton") as Button;
        _tutorialNav           = _MainMenu.Q("TutorialButton") as Button;
        _rcControllerNav       = _MainMenu.Q("RCControllerButton") as Button;
        _droneConfigurationNav = _MainMenu.Q("DroneButton") as Button;
        _fcConfigurationNav    = _MainMenu.Q("FCButton") as Button;
        _settingsNav           = _MainMenu.Q("SettingsButton") as Button;
        _exit                  = _MainMenu.Q("ExitButton") as Button;

        _rcControllerNav.RegisterCallback<ClickEvent>(OnRCControllerBtnClick);

        Debug.Log("uiManager: " + _uiManager + " MainMenu: " + _MainMenu + " RCMenu: " + _RCControllerMenu + " RCNavBtn: " + _rcControllerNav);
    }

    private void OnDisable()
    {
        _rcControllerNav.UnregisterCallback<ClickEvent>(OnRCControllerBtnClick);  
    } 

    private void OnRCControllerBtnClick(ClickEvent evt)
    {
        _MainMenu.visible = false;
        _RCControllerMenu.visible = true;
    }

}
