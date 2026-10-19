using UnityEngine;
using UnityEngine.UIElements;

public class SettingsView : MonoBehaviour
{
    private UIDocument _uiManager;
    private VisualElement _MainMenu;
    private VisualElement _PauseMenu;
    private VisualElement _SettingsView;
    
    private VisualElement _RCMenuTab;
    private VisualElement _DroneMenuTab;
    private VisualElement _FCMenuTab;
    private VisualElement _SettingsMenuTab;
        
    private Button _backBtn;
    private Button _saveBtn;

    private Button _rcTabBtn;
    private Button _droneTabBtn;
    private Button _fcTabBtn;
    private Button _settingsTabBtn;


    private VisualElement _currentTab;
    private ISettingsMenuController _currentTabController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        _uiManager = GetComponent<UIDocument>();
        _MainMenu = _uiManager.rootVisualElement.Q<VisualElement>("MainMenu");
        _PauseMenu = _uiManager.rootVisualElement.Q<VisualElement>("PauseMenu");

        _SettingsView = _uiManager.rootVisualElement.Q<VisualElement>("SettingsView");

        _RCMenuTab = _SettingsView.Q<VisualElement>("RCMenuTab");
        _DroneMenuTab = _SettingsView.Q<VisualElement>("DroneMenuTab");
        _FCMenuTab = _SettingsView.Q<VisualElement>("FCMenuTab");
        _SettingsMenuTab = _SettingsView.Q<VisualElement>("SettingsMenuTab");

        _backBtn = _SettingsView.Q<Button>("BackButton");
        _saveBtn = _SettingsView.Q<Button>("SaveButton");

        _rcTabBtn       = _SettingsView.Q<Button>("RCTabButton");
        _droneTabBtn = _SettingsView.Q<Button>("DroneTabButton");
        _fcTabBtn    = _SettingsView.Q<Button>("FCTabButton");
        _settingsTabBtn = _SettingsView.Q<Button>("SettingsTabButton");
   
        _backBtn.RegisterCallback<ClickEvent>(OnBackBtnClick);
        _saveBtn.RegisterCallback<ClickEvent>(OnSaveBtnClick);
        _rcTabBtn.RegisterCallback<ClickEvent>(OnRCTabBtnClicked);
        _droneTabBtn.RegisterCallback<ClickEvent>(OnDroneTabBtnClicked);
        _fcTabBtn.RegisterCallback<ClickEvent>(OnFCTabBtnClicked);
        _settingsTabBtn.RegisterCallback<ClickEvent>(OnSettingsTabBtnClicked);
    }

    private void OnDisable()
    {
        _backBtn.UnregisterCallback<ClickEvent>(OnBackBtnClick);
        _saveBtn.UnregisterCallback<ClickEvent>(OnSaveBtnClick);
        _rcTabBtn.UnregisterCallback<ClickEvent>(OnRCTabBtnClicked);
        _droneTabBtn.UnregisterCallback<ClickEvent>(OnDroneTabBtnClicked);
        _fcTabBtn.UnregisterCallback<ClickEvent>(OnFCTabBtnClicked);
        _settingsTabBtn.UnregisterCallback<ClickEvent>(OnSettingsTabBtnClicked);
    }

    private void OnBackBtnClick(ClickEvent evt)
    {
        //TODO stop calibration if needed
        //StopCoroutine(CalibrationRoutine()); 
        // this but also make sure that calibration is not in a unsafe state
        

        //TODO find better way to do this. Maybe a centralized UI manager to handle view transitions
        _SettingsView.style.display = DisplayStyle.None;

        if (GameManager.Instance.isGameStarted && GameManager.Instance.isPaused)   
            _PauseMenu.style.display = DisplayStyle.Flex;
        else
            _MainMenu.style.display = DisplayStyle.Flex;
    }

    private void OnSaveBtnClick(ClickEvent evt)
    {
        //TODO implement save functionality for each menu. Maybe with Polimorfism
        _currentTabController.OnSave();
    }

    private void OnRCTabBtnClicked(ClickEvent evt)
    {
        _currentTab.style.display = DisplayStyle.None;
        _currentTab = _RCMenuTab;
        _currentTab.style.display = DisplayStyle.Flex;
        _currentTabController = gameObject.GetComponent<RCMenuController>();
    }
    private void OnDroneTabBtnClicked(ClickEvent evt)
    {
        _currentTab.style.display = DisplayStyle.None;
        _currentTab = _DroneMenuTab;
        _currentTab.style.display = DisplayStyle.Flex;
        //_currentTabController = gameObject.GetComponent<DroneMenuController>();
    }
    private void OnFCTabBtnClicked(ClickEvent evt)
    {
        _currentTab.style.display = DisplayStyle.None;
        _currentTab = _FCMenuTab;
        _currentTab.style.display = DisplayStyle.Flex;
        _currentTabController = gameObject.GetComponent<FCMenuController>();
    }

    private void OnSettingsTabBtnClicked(ClickEvent evt)
    {
        _currentTab.style.display = DisplayStyle.None;
        _currentTab = _SettingsMenuTab;
        _currentTab.style.display = DisplayStyle.Flex;
        //_currentTabController = gameObject.GetComponent<SettingsMenuController>();
    }
}
