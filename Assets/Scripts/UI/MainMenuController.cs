using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    private RCControllerManager _physicalRCController;

    private UIDocument _uiManager;
    private VisualElement _MainMenu;
    private VisualElement _MapSelector;
    private VisualElement _SettingsView;

    private Button _startFlightNav;
    private Button _tutorialNav;
    private Button _settingsNav;
    private Button _exit;

    private Image _controllerStatusIcon;
    private Label _controllerStatusLabel;

    private MonoBehaviour _settingsViewController;
    private MonoBehaviour _HUDController;


    [SerializeField] private VectorImage controller;
    [SerializeField] private VectorImage noController;

    private void OnEnable()
    {
        _physicalRCController = RCControllerManager.Instance;

        _uiManager = GetComponent<UIDocument>();
        _MainMenu = _uiManager.rootVisualElement.Q<VisualElement>("MainMenu");
        _SettingsView = _uiManager.rootVisualElement.Q<VisualElement>("SettingsView");
        _MapSelector = _uiManager.rootVisualElement.Q<VisualElement>("MapSelectorMenu");

        _startFlightNav = _MainMenu.Q<Button>("StartFlightButton");
        _tutorialNav = _MainMenu.Q<Button>("TutorialButton");
        _settingsNav = _MainMenu.Q<Button>("SettingsButton");
        _exit = _MainMenu.Q<Button>("ExitButton");

        _controllerStatusIcon = _MainMenu.Q<Image>("StatusIcon");
        _controllerStatusLabel = _MainMenu.Q<Label>("StatusLabel");

        _settingsViewController = gameObject.GetComponent<SettingsViewController>();

        if (_physicalRCController.registeredDevice != null) OnControllerConnected();

        _startFlightNav.RegisterCallback<ClickEvent>(OnStartFlightBtnClick);
        _settingsNav.RegisterCallback<ClickEvent>(OnSettingsBtnClick);
        _exit.RegisterCallback<ClickEvent>(OnExitBtnClick);

        GameEvents.Instance.OnControllerConnectedUI += OnControllerConnected;
        GameEvents.Instance.OnControllerDisconnectedUI += OnControllerDisconnected;
    }

    private void OnControllerConnected()
    {
        _controllerStatusLabel.text = _physicalRCController.registeredDevice.displayName;
        _controllerStatusIcon.vectorImage = controller;
    }

    private void OnControllerDisconnected()
    {
        _controllerStatusLabel.text = "No Controller Connected";
        _controllerStatusIcon.vectorImage = noController;
    }

     private void OnDisable()
    {
        _startFlightNav.UnregisterCallback<ClickEvent>(OnStartFlightBtnClick);
        _settingsNav.UnregisterCallback<ClickEvent>(OnSettingsBtnClick);
        _exit.UnregisterCallback<ClickEvent>(OnExitBtnClick);
    }

    private void OnStartFlightBtnClick(ClickEvent evt)
    {
        _MainMenu.style.display = DisplayStyle.None;
        _MapSelector.style.display = DisplayStyle.Flex;
    }

    private void OnSettingsBtnClick(ClickEvent evt)
    {
        _MainMenu.style.display = DisplayStyle.None;
        _SettingsView.style.display = DisplayStyle.Flex;
        _settingsViewController.enabled = true;
    }

    private void OnExitBtnClick(ClickEvent evt)
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
