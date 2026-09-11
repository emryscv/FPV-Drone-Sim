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
    private RCControllerManager _physicalRCController;
    private Controls controls;

    //UI HANDLERS
    private UIDocument _uiManager;
    private VisualElement _MainMenu;
    private VisualElement _RCControllerMenu;

    private Button _backBtn;
    private Button _saveBtn;
    private Button _startCalibrationBtn;

    private VisualElement _controllerStatusIcon;
    private Label _controllerStatusLabel;

    private Label _calibrationInstructionsHeading;
    private Label _calibrationInstructionsDescription;

    private VisualElement _leftStick;
    private VisualElement _rightStick;

    //VARIABLES
    private bool _moveWithInput;
    private bool _onInputDiscovery;
    private int[] _xPositions;
    private int[] _yPositions;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        controls = new Controls();
        controls.RCController.Enable();

        _moveWithInput = true;
        _onInputDiscovery = false;
        _isCalibrated = false;

        _xPositions = new int[] { 0, 130, 130, 0 };
        _yPositions = new int[] { 0, 0, 130, 130 };

    }

    private void OnEnable()
    {
        _physicalRCController = RCControllerManager.Instance; //TODO I might not need this and jsut use the Controls Class

        _uiManager = GetComponent<UIDocument>();
        _MainMenu = _uiManager.rootVisualElement.Q<VisualElement>("MainMenu");
        _RCControllerMenu = _uiManager.rootVisualElement.Q<VisualElement>("RCControllerMenu");

        _controllerStatusIcon = _RCControllerMenu.Q<VisualElement>("StatusIcon");
        _controllerStatusLabel = _RCControllerMenu.Q<Label>("StatusLabel");

        _calibrationInstructionsHeading = _RCControllerMenu.Q<VisualElement>("CalibrationPanel").Q<Label>("Heading");
        _calibrationInstructionsDescription = _RCControllerMenu.Q<VisualElement>("CalibrationPanel").Q<Label>("Description");

        Debug.Log("Heading: " + _calibrationInstructionsHeading + ", Description: " + _calibrationInstructionsDescription);

        _leftStick = _RCControllerMenu.Q<VisualElement>("LeftStick");
        _rightStick = _RCControllerMenu.Q<VisualElement>("RightStick");

        _backBtn = _RCControllerMenu.Q<Button>("BackButton");
        _saveBtn = _RCControllerMenu.Q<Button>("SaveButton");
        _startCalibrationBtn = _RCControllerMenu.Q<Button>("StartCalibrationButton");

        _backBtn.RegisterCallback<ClickEvent>(OnBackBtnClick);
        _saveBtn.RegisterCallback<ClickEvent>(OnSaveBtnClick);
        _startCalibrationBtn.RegisterCallback<ClickEvent>(OnStartCalibrationBtnClick);
    }

    private void Update()
    {
        //TODO I might not need this and just use the controls class
        if (_physicalRCController.registeredDevice != null)
        {
            _controllerStatusLabel.text = _physicalRCController.registeredDevice.displayName;
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
            float throttle = _physicalRCController._throttle.axis.ReadValue();
            float yaw = _physicalRCController._yaw.axis.ReadValue();
            float pitch = _physicalRCController._pitch.axis.ReadValue();
            float roll = _physicalRCController._roll.axis.ReadValue();

            //This equation is fixed to the size of the parent componenet. If the size change this has to change
            _leftStick.style.left = yaw * 65f + 65f;
            _leftStick.style.top = 65f - throttle * 65f;

            _rightStick.style.left = roll * 65f + 65f;
            _rightStick.style.top = 65f - pitch * 65f;
        }

        if (_onInputDiscovery)
        {
            _physicalRCController.ReadAllAxes();
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
        _MainMenu.visible = true;
        _RCControllerMenu.visible = false;
    }

    private void OnSaveBtnClick(ClickEvent evt)
    {
        _physicalRCController.SaveCalibration();
    }

    private void OnStartCalibrationBtnClick(ClickEvent evt)
    {
        StartCoroutine(CalibrationRoutine());
    }

    private IEnumerator CalibrationRoutine()
    {
        _physicalRCController.FindAllAxes();

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

        _physicalRCController.PruneAxes();

        yield return new WaitForSecondsRealtime(1f);

        _calibrationInstructionsHeading.text = "Left Stick";
        _calibrationInstructionsDescription.text = "Move the left stick up to mimic the movement on screen.";
        _leftStick.style.left = 130;

        yield return WaitForAxisDetection();
        _physicalRCController.SetLeftStickY();

        _calibrationInstructionsDescription.text = "Center stick";
        _leftStick.style.left = 65;

        yield return new WaitForSecondsRealtime(1f);

        _calibrationInstructionsDescription.text = "Move the left stick to the right to mimic the movement on screen.";
        _leftStick.style.top = 0;

        yield return WaitForAxisDetection();
        _physicalRCController.SetLeftStickX();

        _calibrationInstructionsDescription.text = "Center stick";
        _leftStick.style.top = 65;

        yield return new WaitForSecondsRealtime(1f);

        _calibrationInstructionsHeading.text = "Right Stick";
        _calibrationInstructionsDescription.text = "Move the right stick up to mimic the movement on screen.";
        _rightStick.style.left = 130;

        yield return WaitForAxisDetection();
        _physicalRCController.SetRightStickY();

        _calibrationInstructionsDescription.text = "Center stick";
        _rightStick.style.left = 65;

        yield return new WaitForSecondsRealtime(1f);

        _calibrationInstructionsDescription.text = "Move the right stick to the right to mimic the movement on screen.";
        _rightStick.style.top = 0;

        yield return WaitForAxisDetection();
        _physicalRCController.SetRightStickX();

        _calibrationInstructionsDescription.text = "Center stick";
        _rightStick.style.top = 65;

        yield return new WaitForSecondsRealtime(1f);

        _calibrationInstructionsHeading.text = "Roll";
        _calibrationInstructionsDescription.text = "Move the roll stick to the right.";

        yield return WaitForAxisDetection();
        _physicalRCController.SetRoll();

        _calibrationInstructionsDescription.text = "Center stick.";

        yield return new WaitForSecondsRealtime(1f);

        _calibrationInstructionsHeading.text = "Pitch";
        _calibrationInstructionsDescription.text = "Move the pitch stick up.";

        yield return WaitForAxisDetection();
        _physicalRCController.SetPitch();

        _calibrationInstructionsDescription.text = "Center stick.";

        yield return new WaitForSecondsRealtime(1f);

        _calibrationInstructionsHeading.text = "Yaw";
        _calibrationInstructionsDescription.text = "Move the yaw stick to the right.";

        yield return WaitForAxisDetection();
        _physicalRCController.SetYaw();

        _calibrationInstructionsDescription.text = "Center stick.";

        yield return new WaitForSecondsRealtime(1f);

        _calibrationInstructionsHeading.text = "Throttle";
        _calibrationInstructionsDescription.text = "Move the Throttle stick up.";

        yield return WaitForAxisDetection();
        _physicalRCController.SetThrottle();

        yield return new WaitForSecondsRealtime(1f);

        _calibrationInstructionsDescription.text = "Center stick.";

        _moveWithInput = true;
        _isCalibrated = true;

        SetTransition(_leftStick, 0f, EasingMode.Linear);
        SetTransition(_rightStick, 0f, EasingMode.Linear);

        _physicalRCController.PrintDebug();
    }

    private IEnumerator WaitForAxisDetection()
    {
        float elapsed = 0f;

        while (elapsed < AXIS_DETECTION_TIMEOUT_SECONDS)
        {
            if (_physicalRCController.TryReadAxis())
            {
                yield break;
            }

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        Debug.LogWarning("Timed out while waiting for axis movement during calibration.");
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


