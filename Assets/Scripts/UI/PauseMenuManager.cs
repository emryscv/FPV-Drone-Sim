using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenuManager : MonoBehaviour
{
    private UIDocument _uiManager;
    private VisualElement _PauseMenu;
    private VisualElement _MainMenu;
    private VisualElement _RCControllerMenu; 

    private Button _resumeFlightBtn;
    private Button _rcControllerNav;
    private Button _droneConfigurationNav;
    private Button _fcConfigurationNav;
    private Button _settingsNav;
    private Button _exitNav;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        _uiManager = GetComponent<UIDocument>();
        _PauseMenu = _uiManager.rootVisualElement.Q<VisualElement>("PauseMenu");
        _MainMenu = _uiManager.rootVisualElement.Q<VisualElement>("MainMenu");
        _RCControllerMenu = _uiManager.rootVisualElement.Q<VisualElement>("RCControllerMenu");

        _resumeFlightBtn = _MainMenu.Q<Button>("ResumeFlightButton");
        _rcControllerNav = _MainMenu.Q<Button>("RCControllerButton");
        _droneConfigurationNav = _MainMenu.Q<Button>("DroneButton");
        _fcConfigurationNav = _MainMenu.Q<Button>("FCButton");
        _settingsNav = _MainMenu.Q<Button>("SettingsButton");
        _exitNav = _MainMenu.Q<Button>("ExitButton");

        _resumeFlightBtn.RegisterCallback<ClickEvent>(OnResumeFlightBtnClick);
        _rcControllerNav.RegisterCallback<ClickEvent>(OnRCControllerNavClick);
        _droneConfigurationNav.RegisterCallback<ClickEvent>(OnDroneConfigurationNavClick);
        _fcConfigurationNav.RegisterCallback<ClickEvent>(OnFCConfigurationNavClick);
        _settingsNav.RegisterCallback<ClickEvent>(OnSettingsNavClick);
        _exitNav.RegisterCallback<ClickEvent>(OnExitNavClick);
    }

    private void OnDisable()
    {
        _resumeFlightBtn.UnregisterCallback<ClickEvent>(OnResumeFlightBtnClick);
        _rcControllerNav.UnregisterCallback<ClickEvent>(OnRCControllerNavClick);
        _droneConfigurationNav.UnregisterCallback<ClickEvent>(OnDroneConfigurationNavClick);
        _fcConfigurationNav.UnregisterCallback<ClickEvent>(OnFCConfigurationNavClick);
        _settingsNav.UnregisterCallback<ClickEvent>(OnSettingsNavClick);
        _exitNav.UnregisterCallback<ClickEvent>(OnExitNavClick);
    }

    private void OnResumeFlightBtnClick(ClickEvent evt)
    {
        //TODO resume the flight session
        _PauseMenu.style.display = DisplayStyle.None;
    }

    private void OnRCControllerNavClick(ClickEvent evt)
    {
        //TODO find out if i just want to stop and resume after this or maybe restart after
        _PauseMenu.style.display = DisplayStyle.None;
        _RCControllerMenu.style.display = DisplayStyle.Flex;
    }

    private void OnDroneConfigurationNavClick(ClickEvent evt)
    {
        // Handle drone configuration navigation button click
    }

    private void OnFCConfigurationNavClick(ClickEvent evt)
    {
        // Handle flight controller configuration navigation button click
    }

    private void OnSettingsNavClick(ClickEvent evt)
    {
        // Handle settings navigation button click
    }

    private void OnExitNavClick(ClickEvent evt)
    {
        //TODO figure out what quiting the session should do.
        _PauseMenu.style.display = DisplayStyle.None;
        _MainMenu.style.display = DisplayStyle.Flex;
    }
}
