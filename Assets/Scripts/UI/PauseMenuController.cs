using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenuController : MonoBehaviour
{
    private UIDocument _uiManager;
    private VisualElement _PauseMenu;
    private VisualElement _MainMenu;
    private VisualElement _SettingsView; 

    private Button _resumeFlightBtn;
    private Button _restartFlightBtn;
    private Button _settingsNav;
    private Button _exitNav;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        // Get reference to UI views
        _uiManager = GetComponent<UIDocument>();
        _PauseMenu = _uiManager.rootVisualElement.Q<VisualElement>("PauseMenu");
        _MainMenu = _uiManager.rootVisualElement.Q<VisualElement>("MainMenu");
        _SettingsView = _uiManager.rootVisualElement.Q<VisualElement>("SettingsView");

        // UI elements References
        _resumeFlightBtn = _PauseMenu.Q<Button>("ResumeFlightButton");
        _restartFlightBtn = _PauseMenu.Q<Button>("RestartFlightButton");
        _settingsNav = _PauseMenu.Q<Button>("SettingsButton");
        _exitNav = _PauseMenu.Q<Button>("ExitButton");

        //In UI events
        _resumeFlightBtn.RegisterCallback<ClickEvent>(OnResumeFlightBtnClick);
        _restartFlightBtn.RegisterCallback<ClickEvent>(OnRestartFlightBtnClick);
        _settingsNav.RegisterCallback<ClickEvent>(OnSettingsNavClick);
        _exitNav.RegisterCallback<ClickEvent>(OnExitNavClick);

        //Events
        GameEvents.Instance.OnPause += OnGamePaused;
        GameEvents.Instance.OnUnpause += OnGameUnpaused;
    }

    private void OnDisable()
    {
        _resumeFlightBtn.UnregisterCallback<ClickEvent>(OnResumeFlightBtnClick);
        _restartFlightBtn.UnregisterCallback<ClickEvent>(OnRestartFlightBtnClick);
        _settingsNav.UnregisterCallback<ClickEvent>(OnSettingsNavClick);
        _exitNav.UnregisterCallback<ClickEvent>(OnExitNavClick);

        GameEvents.Instance.OnPause -= OnGamePaused;
        GameEvents.Instance.OnUnpause -= OnGameUnpaused;
    }

    private void OnResumeFlightBtnClick(ClickEvent evt)
    {
        GameManager.Instance.UnpauseSimulation();
        _PauseMenu.style.display = DisplayStyle.None;
    }

    private void OnRestartFlightBtnClick(ClickEvent evt)
    {
        GameEvents.Instance.Restart();
        _PauseMenu.style.display = DisplayStyle.None;
    }

    private void OnSettingsNavClick(ClickEvent evt)
    {
        // Handle settings navigation button click
        _PauseMenu.style.display = DisplayStyle.None;
        _SettingsView.style.display = DisplayStyle.Flex;
    }

    private void OnExitNavClick(ClickEvent evt)
    {
        //TODO figure out what quiting the session should do.
        _PauseMenu.style.display = DisplayStyle.None;
        _MainMenu.style.display = DisplayStyle.Flex;
        
        //TODO figure out if an event is better
        GameManager.Instance.StopSimulation();
    }

    private void OnGamePaused()
    {
        _PauseMenu.style.display = DisplayStyle.Flex;
    }

    private void OnGameUnpaused()
    {
        _PauseMenu.style.display = DisplayStyle.None;
    }
}
