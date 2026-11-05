using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;

public class RCMenuController : MonoBehaviour, ISettingsMenuController
{
    [SerializeField] private VectorImage controller;
    [SerializeField] private VectorImage noController;

    private readonly Color GREEN = new Color(0.2980392f, 0.6705883f, 0.2117647f);
    private readonly Color RED = new Color(0.4039216f, 0.1019608f, 0.08627451f);

    private RCControllerManager controls;
    private UIDocument _uiManager;
    private VisualElement _RCMenuTab;

    private Image _controllerStatusIcon;
    private Label _controllerStatusLabel;

    private VisualElement _leftStick;
    private VisualElement _rightStick;

    private Button _undoBtn;

    private Label _calibrationInstructionsHeading;
    private Label _calibrationInstructionsDescription;

    private Button _yesBtn;
    private Button _noBtn;

    private Button _startCalibrationBtn;
    private Button _saveBtn;
    private Button _savedBtn;


    private CalibrationController _calibrationController;

    private void OnEnable()
    {
        controls = RCControllerManager.Instance;

        _uiManager = GetComponent<UIDocument>();
        _RCMenuTab = _uiManager.rootVisualElement.Q<VisualElement>("RCMenuTab");
        
        _controllerStatusIcon = _RCMenuTab.Q<Image>("StatusIcon");
        _controllerStatusLabel = _RCMenuTab.Q<Label>("StatusLabel");
        
        _leftStick = _RCMenuTab.Q<VisualElement>("LeftStick");
        _rightStick = _RCMenuTab.Q<VisualElement>("RightStick");

        _undoBtn = _RCMenuTab.Q<Button>("UndoButton");
        
        _calibrationInstructionsHeading = _RCMenuTab.Q<VisualElement>("CalibrationPanel").Q<Label>("Heading");
        _calibrationInstructionsDescription = _RCMenuTab.Q<VisualElement>("CalibrationPanel").Q<Label>("Description");
        
        _yesBtn = _RCMenuTab.Q<Button>("YesButton");
        _noBtn = _RCMenuTab.Q<Button>("NoButton");
        
        _startCalibrationBtn = _RCMenuTab.Q<Button>("StartCalibrationButton");
        _saveBtn = _RCMenuTab.Q<Button>("SaveButton");
        _savedBtn = _RCMenuTab.Q<Button>("SavedButton");

        _calibrationController = new CalibrationController(
            this.StartCoroutine,    
            this.StopCoroutine,
            controls,
            _leftStick,
            _rightStick,
            _undoBtn,
            _calibrationInstructionsHeading,
            _calibrationInstructionsDescription,
            _yesBtn,
            _noBtn,
            _startCalibrationBtn,
            _saveBtn);

        _undoBtn.RegisterCallback<ClickEvent>(OnUndoBtnClick);
        _yesBtn.RegisterCallback<ClickEvent>(OnYesBtnClick);
        _noBtn.RegisterCallback<ClickEvent>(OnNoBtnClick);
        _startCalibrationBtn.RegisterCallback<ClickEvent>(OnStartCalibrationBtnClick);
        _saveBtn.RegisterCallback<ClickEvent>(OnSaveBtnClick);

        if (controls.registeredDevice != null) OnControllerConnected();

        GameEvents.Instance.OnControllerConnectedUI += OnControllerConnected;
        GameEvents.Instance.OnControllerDisconnectedUI += OnControllerDisconnected;
    }

    private void Update()
    {
        if (controls.registeredDevice == null) return;

        if (_calibrationController.MoveWithInput)
        {
            float lStickX = controls.LStickX?.ReadValue() ?? 0f;
            float lStickY = controls.LStickY?.ReadValue() ?? 0f;
            float rStickX = controls.RStickX?.ReadValue() ?? 0f;
            float rStickY = controls.RStickY?.ReadValue() ?? 0f;

            _leftStick.style.left = lStickX * 65f + 65f;
            _leftStick.style.top = 65f - lStickY * 65f;
            _rightStick.style.left = rStickX * 65f + 65f;
            _rightStick.style.top = 65f - rStickY * 65f;
        }

        if (_calibrationController.IsInputDiscovery)
        {
            controls.ReadAllAxes();
        }
    }

    private void OnDisable()
    {
        _calibrationController.Stop();

        _undoBtn.UnregisterCallback<ClickEvent>(OnUndoBtnClick);
        _yesBtn.UnregisterCallback<ClickEvent>(OnYesBtnClick);
        _noBtn.UnregisterCallback<ClickEvent>(OnNoBtnClick);
        _startCalibrationBtn.UnregisterCallback<ClickEvent>(OnStartCalibrationBtnClick);
        _saveBtn.UnregisterCallback<ClickEvent>(OnSaveBtnClick);

        GameEvents.Instance.OnControllerConnectedUI -= OnControllerConnected;
        GameEvents.Instance.OnControllerDisconnectedUI -= OnControllerDisconnected;
    }

    private void OnControllerConnected()
    {
        _controllerStatusLabel.text = controls.registeredDevice.displayName;
        _controllerStatusLabel.style.color = GREEN;
        
        _controllerStatusIcon.vectorImage = controller;
        _controllerStatusIcon.tintColor = GREEN;

        Helpers.SetButtonState(_startCalibrationBtn, true);
    }

    private void OnControllerDisconnected()
    {
        _controllerStatusLabel.text = "NO CONTROLLER CONNECTED";
        _controllerStatusLabel.style.color = RED;
        
        _controllerStatusIcon.vectorImage = noController;
        _controllerStatusIcon.tintColor = RED;
        
        Helpers.SetButtonState(_startCalibrationBtn, false);
        _calibrationController.Stop();
    }

    private void OnStartCalibrationBtnClick(ClickEvent evt)
    {
        _calibrationController.Start();
    }

    private void OnUndoBtnClick(ClickEvent evt)
    {
        _calibrationController.Undo();
    }

    private void OnSaveBtnClick(ClickEvent evt)
    {
        controls.SaveCalibration();
        StartCoroutine(NotifySaved());
    }

    private void OnYesBtnClick(ClickEvent evt)
    {
        _calibrationController.SetPositiveButtonPressed(true);
    }

    private void OnNoBtnClick(ClickEvent evt)
    {
        _calibrationController.SetPositiveButtonPressed(false);
    }

    private IEnumerator NotifySaved()
    {
        _saveBtn.style.display = DisplayStyle.None;
        _savedBtn.style.display = DisplayStyle.Flex;

        yield return new WaitForSecondsRealtime(2f);

        _saveBtn.style.display = DisplayStyle.Flex;
        _savedBtn.style.display = DisplayStyle.None;
    }
}
