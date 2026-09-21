using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Collections;

public class RCControllerMenuController : MonoBehaviour
{
    private const float AXIS_DETECTION_TIMEOUT_SECONDS = 10f;

    //CONSTANTS
    private Color GREEN = new Color(0.2980392f, 0.6705883f, 0.2117647f);
    private Color RED = new Color(0.4039216f, 0.1019608f, 0.08627451f);

    //INPUT SOURCES
    private RCControllerManager controls;

    //UI HANDLERS
    private UIDocument _uiManager;
    private VisualElement _MainMenu;
    private VisualElement _RCControllerMenu;
    private VisualElement _PauseMenu;

    private Button _backBtn;
    private Button _saveBtn;
    private Button _startCalibrationBtn;

    private VisualElement _controllerStatusIcon;
    private Label _controllerStatusLabel;

    private Label _calibrationInstructionsHeading;
    private Label _calibrationInstructionsDescription;

    private Button _yesBtn;
    private Button _noBtn;


    private VisualElement _leftStick;
    private VisualElement _rightStick;

    //VARIABLES
    private bool _moveWithInput;
    private bool _onInputDiscovery;
    private bool _buttonPressed;
    private bool _isPositive;

    private int[] _xPositions;
    private int[] _yPositions;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _moveWithInput = true;
        _onInputDiscovery = false;

        _buttonPressed = false;
        _isPositive = false;

        _xPositions = new int[] { 0, 130, 130, 0 };
        _yPositions = new int[] { 0, 0, 130, 130 };
    }

    private void OnEnable()
    {
        controls = RCControllerManager.Instance;

        _uiManager = GetComponent<UIDocument>();
        _MainMenu = _uiManager.rootVisualElement.Q<VisualElement>("MainMenu");
        _RCControllerMenu = _uiManager.rootVisualElement.Q<VisualElement>("RCControllerMenu");
        _PauseMenu = _uiManager.rootVisualElement.Q<VisualElement>("PauseMenu");

        _backBtn = _RCControllerMenu.Q<Button>("BackButton");
        _saveBtn = _RCControllerMenu.Q<Button>("SaveButton");
        _startCalibrationBtn = _RCControllerMenu.Q<Button>("StartCalibrationButton");

        _controllerStatusIcon = _RCControllerMenu.Q<VisualElement>("StatusIcon");
        _controllerStatusLabel = _RCControllerMenu.Q<Label>("StatusLabel");

        _calibrationInstructionsHeading = _RCControllerMenu.Q<VisualElement>("CalibrationPanel").Q<Label>("Heading");
        _calibrationInstructionsDescription = _RCControllerMenu.Q<VisualElement>("CalibrationPanel").Q<Label>("Description");

        _yesBtn = _RCControllerMenu.Q<Button>("YesButton");
        _noBtn = _RCControllerMenu.Q<Button>("NoButton");

        _leftStick = _RCControllerMenu.Q<VisualElement>("LeftStick");
        _rightStick = _RCControllerMenu.Q<VisualElement>("RightStick");

        _backBtn.RegisterCallback<ClickEvent>(OnBackBtnClick);
        _saveBtn.RegisterCallback<ClickEvent>(OnSaveBtnClick);
        _startCalibrationBtn.RegisterCallback<ClickEvent>(OnStartCalibrationBtnClick);
        _yesBtn.RegisterCallback<ClickEvent>(OnYesBtnClick);
        _noBtn.RegisterCallback<ClickEvent>(OnNoBtnClick);
    }

    private void Update()
    {
        if (controls.registeredDevice != null)
        {
            _controllerStatusLabel.text = controls.registeredDevice.displayName;
            _controllerStatusLabel.style.color = GREEN;
            _controllerStatusIcon.style.backgroundColor = GREEN;
        }
        else
        {
            _controllerStatusLabel.text = "NO CONTROLLER CONNECTED";
            _controllerStatusLabel.style.color = RED;
            _controllerStatusIcon.style.backgroundColor = RED;
        }

        if (_moveWithInput)
        {
            float lStickX = controls.LStickX?.ReadValue() ?? 0f;
            float lStickY = controls.LStickY?.ReadValue() ?? 0f;

            float rStickX = controls.RStickX?.ReadValue() ?? 0f;
            float rStickY = controls.RStickY?.ReadValue() ?? 0f;

            //This equation is fixed to the size of the parent componenet. If the size change this has to change
            _leftStick.style.left = lStickX * 65f + 65f;
            _leftStick.style.top = 65f - lStickY * 65f;

            _rightStick.style.left = rStickX * 65f + 65f;
            _rightStick.style.top = 65f - rStickY * 65f;
        }

        if (_onInputDiscovery)
        {
            controls.ReadAllAxes();
        }
    }

    private void OnDisable()
    {
        _backBtn.UnregisterCallback<ClickEvent>(OnBackBtnClick);
        _saveBtn.UnregisterCallback<ClickEvent>(OnSaveBtnClick);
        _startCalibrationBtn.UnregisterCallback<ClickEvent>(OnStartCalibrationBtnClick);
    }

    private void OnBackBtnClick(ClickEvent evt)
    {
        _RCControllerMenu.style.display = DisplayStyle.None;

        if (GameManager.Instance.isPaused)   
            _PauseMenu.style.display = DisplayStyle.Flex;
        else
            _MainMenu.style.display = DisplayStyle.Flex;
    }

    private void OnSaveBtnClick(ClickEvent evt)
    {
        controls.SaveCalibration();
    }

    private void OnStartCalibrationBtnClick(ClickEvent evt)
    {
        StartCoroutine(CalibrationRoutine());
    }

    private void OnYesBtnClick(ClickEvent evt)
    {
        _buttonPressed = true;
        _isPositive = true;
    }

    private void OnNoBtnClick(ClickEvent evt)
    {
        // Handle No button click
        _buttonPressed = true;
        _isPositive = false;
    }

    private IEnumerator CalibrationRoutine()
    {
        controls.FindAllAxes(); //TODO what happens if no axis is moved

        _moveWithInput = false;
        _calibrationInstructionsHeading.text = "CALIBRATING! ... ";
        _calibrationInstructionsDescription.text = "Move your controller sticks to mimic the movement on screen.";

        SetTransition(_leftStick, 0.5f, EasingMode.EaseInOut);
        SetTransition(_rightStick, 0.5f, EasingMode.EaseInOut);

        _onInputDiscovery = true;

        for (int j = 0; j < 8; j++)
        {
            for (int i = 0; i < 4; i++)
            {
                _leftStick.style.left = _xPositions[i];
                _leftStick.style.top = _yPositions[i];

                _rightStick.style.left = _xPositions[i];
                _rightStick.style.top = _yPositions[i];

                yield return new WaitForSecondsRealtime(0.25f);
            }
        }

        _leftStick.style.left = 65f;
        _leftStick.style.top = 65f;

        _rightStick.style.left = 65f;
        _rightStick.style.top = 65f;
        _onInputDiscovery = false;

        controls.PruneAxes();

        yield return new WaitForSecondsRealtime(1f);

        _calibrationInstructionsHeading.text = "Left Stick";
        _calibrationInstructionsDescription.text = "Move the left stick to the right to mimic the movement on screen.";
        _leftStick.style.left = 130;

        yield return WaitForAxisDetection();
        controls.SetAxis("LStickX");

        _calibrationInstructionsDescription.text = "Center stick";
        _leftStick.style.left = 65;

        yield return new WaitForSecondsRealtime(1f);

        _calibrationInstructionsDescription.text = "Move the left stick up to mimic the movement on screen.";
        _leftStick.style.top = 0;

        yield return WaitForAxisDetection();
        controls.SetAxis("LStickY");

        _calibrationInstructionsDescription.text = "Center stick";
        _leftStick.style.top = 65;

        yield return new WaitForSecondsRealtime(1f);

        _calibrationInstructionsHeading.text = "Right Stick";
        _calibrationInstructionsDescription.text = "Move the right stick to the right to mimic the movement on screen.";
        _rightStick.style.left = 130;

        yield return WaitForAxisDetection();
        controls.SetAxis("RStickX");

        _calibrationInstructionsDescription.text = "Center stick";
        _rightStick.style.left = 65;

        yield return new WaitForSecondsRealtime(1f);

        _calibrationInstructionsDescription.text = "Move the right stick up to mimic the movement on screen.";
        _rightStick.style.top = 0;

        yield return WaitForAxisDetection();
        controls.SetAxis("RStickY");

        _calibrationInstructionsDescription.text = "Center stick";
        _rightStick.style.top = 65;

        yield return new WaitForSecondsRealtime(1f);

        _calibrationInstructionsHeading.text = "Roll";
        _calibrationInstructionsDescription.text = "Move the roll stick to the right.";

        yield return WaitForAxisDetection();
        controls.SetAxis("Roll");

        _calibrationInstructionsDescription.text = "Center stick.";

        yield return new WaitForSecondsRealtime(1f);

        _calibrationInstructionsHeading.text = "Pitch";
        _calibrationInstructionsDescription.text = "Move the pitch stick up.";

        yield return WaitForAxisDetection();
        controls.SetAxis("Pitch");

        _calibrationInstructionsDescription.text = "Center stick.";

        yield return new WaitForSecondsRealtime(1f);

        _calibrationInstructionsHeading.text = "Yaw";
        _calibrationInstructionsDescription.text = "Move the yaw stick to the right.";

        yield return WaitForAxisDetection();
        controls.SetAxis("Yaw");

        _calibrationInstructionsDescription.text = "Center stick.";

        yield return new WaitForSecondsRealtime(1f);

        _calibrationInstructionsHeading.text = "Throttle";
        _calibrationInstructionsDescription.text = "Move the Throttle stick up.";

        yield return WaitForAxisDetection();
        controls.SetAxis("Throttle");

        yield return new WaitForSecondsRealtime(1f);

        _calibrationInstructionsDescription.text = "Center stick.";

        _moveWithInput = true;

        SetTransition(_leftStick, 0f, EasingMode.Linear);
        SetTransition(_rightStick, 0f, EasingMode.Linear);

        _yesBtn.style.display = DisplayStyle.Flex;
        _noBtn.style.display = DisplayStyle.Flex;

        _calibrationInstructionsHeading.text = "Invert Input";
        _calibrationInstructionsDescription.text = "Do you want to invert any input?";

        yield return WaitForButtonPress();
        if (_isPositive)
        { //don't know the condition 
            _calibrationInstructionsHeading.text = "Invert Roll";
            _calibrationInstructionsDescription.text = "Do you want to invert roll?";

            yield return WaitForButtonPress();
            if (_isPositive) controls.InvertAxis("Roll");

            _calibrationInstructionsHeading.text = "Invert Pitch";
            _calibrationInstructionsDescription.text = "Do you want to invert pitch?";

            yield return WaitForButtonPress();
            if (_isPositive) controls.InvertAxis("Pitch");

            _calibrationInstructionsHeading.text = "Invert Yaw";
            _calibrationInstructionsDescription.text = "Do you want to invert yaw?";

            yield return WaitForButtonPress();
            if (_isPositive) controls.InvertAxis("Yaw");

            _calibrationInstructionsHeading.text = "Invert Throttle";
            _calibrationInstructionsDescription.text = "Do you want to invert throttle?";

            yield return WaitForButtonPress();
            if (_isPositive) controls.InvertAxis("Throttle");
        }

        _yesBtn.style.display = DisplayStyle.None;
        _noBtn.style.display = DisplayStyle.None;

        controls.PrintDebug();
    }

    private IEnumerator WaitForAxisDetection()
    {
        //float elapsed = 0f;

        while (true)
        {
            if (controls.TryReadAxis())
            {
                yield break;
            }

            yield return null;
        }

    }

    private IEnumerator WaitForButtonPress()
    {
        while (!_buttonPressed)
        {
            yield return null;
        }

        _buttonPressed = false;
    }

    private void SetTransition(VisualElement element, float duration, EasingMode easingMode)
    {
        element.style.transitionProperty = new List<StylePropertyName>
            {
                new StylePropertyName("top"),
                new StylePropertyName("left")
            };

        element.style.transitionDuration = new List<TimeValue>
            {
                new TimeValue(duration, TimeUnit.Second),
                new TimeValue(duration, TimeUnit.Second)
            };

        element.style.transitionTimingFunction = new List<EasingFunction>
            {
                new EasingFunction(easingMode),
                new EasingFunction(easingMode)
            };
    }
}

