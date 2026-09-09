using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Collections;

public class RCControllerMenuController : MonoBehaviour
{

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
    private int[] _xPositions;
    private int[] _yPositions;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        controls = new Controls();
        controls.RCController.Enable();

        _moveWithInput = true;
        _xPositions = new int[4] { 0, 130, 130, 0 };
        _yPositions = new int[4] { 0, 0, 130, 130 };

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
        if (_physicalRCController.ControllerName != null)
        {
            _controllerStatusLabel.text = _physicalRCController.ControllerName;
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
            float throttle = controls.RCController.Throttle.ReadValue<float>();
            float yaw = controls.RCController.Yaw.ReadValue<float>();
            float pitch = controls.RCController.Pitch.ReadValue<float>();
            float roll = controls.RCController.Roll.ReadValue<float>();

            //This equation is fixed to the size of the parent componenet. If the size change this has to change
            _leftStick.style.left = yaw * 65f + 65f;
            _leftStick.style.top = 65f - throttle * 65f;

            _rightStick.style.left = roll * 65f + 65f;
            _rightStick.style.top = 65f - pitch * 65f;
        }
    }

    private void OnDisable()
    {
        _backBtn.UnregisterCallback<ClickEvent>(OnBackBtnClick);
    }

    private void OnBackBtnClick(ClickEvent evt)
    {
        _MainMenu.visible = true;
        _RCControllerMenu.visible = false;
    }

    private void OnSaveBtnClick(ClickEvent evt)
    {
        _MainMenu.visible = true;
        _RCControllerMenu.visible = false;
    }

    private void OnStartCalibrationBtnClick(ClickEvent evt)
    {
        _moveWithInput = false;
        _calibrationInstructionsHeading.text = "CALIBRATING! ... ";
        _calibrationInstructionsDescription.text = "Move your controller sticks to mimic the movement on screen.";

        SetTransition(_leftStick);
        SetTransition(_rightStick);

        StartCoroutine(GoAroundSequence());

        // _calibrationInstructionsHeading.text = "Left Stick";
        // _calibrationInstructionsDescription.text = "Move the left stick up to mimic the movement on screen.";
        // _leftStick.style.left = 130;

        // _calibrationInstructionsDescription.text = "Center stick";
        // _leftStick.style.left = 65;

        // _calibrationInstructionsDescription.text = "Move the left stick to the right to mimic the movement on screen.";
        // _leftStick.style.top = 0;

        // _calibrationInstructionsDescription.text = "Center stick";
        // _leftStick.style.top = 65;

        // _calibrationInstructionsHeading.text = "Right Stick";
        // _calibrationInstructionsDescription.text = "Move the right stick up to mimic the movement on screen.";
        // _rightStick.style.left = 130;

        // _calibrationInstructionsDescription.text = "Center stick";
        // _rightStick.style.left = 65;

        // _calibrationInstructionsDescription.text = "Move the right stick to the right to mimic the movement on screen.";
        // _rightStick.style.top = 0;

        // _calibrationInstructionsDescription.text = "Center stick";
        // _rightStick.style.top = 65;

        // _calibrationInstructionsHeading.text = "Roll";
        // _calibrationInstructionsDescription.text = "Move the roll stick to the right.";
        // _calibrationInstructionsDescription.text = "Center stick.";
        
        // _calibrationInstructionsHeading.text = "Pitch";
        // _calibrationInstructionsDescription.text = "Move the pitch stick up.";
        // _calibrationInstructionsDescription.text = "Center stick.";

        // _calibrationInstructionsHeading.text = "Yaw";
        // _calibrationInstructionsDescription.text = "Move the yaw stick to the right.";
        // _calibrationInstructionsDescription.text = "Center stick.";
    
        // _calibrationInstructionsHeading.text = "Throttle";
        // _calibrationInstructionsDescription.text = "Move the Throttle stick up.";
        // _calibrationInstructionsDescription.text = "Center stick.";
    }

    private IEnumerator GoAroundSequence()
    {
        for (int j = 0; j < 4; j++)
        {
            for (int i = 0; i < 4; i++)
            {
                _leftStick.style.left = _xPositions[i];
                _leftStick.style.top = _yPositions[i];

                _rightStick.style.left = _xPositions[i];
                _rightStick.style.top = _yPositions[i];

                yield return new WaitForSeconds(0.5f);
            }
        }

        _leftStick.style.left = 65f;
        _leftStick.style.top = 65f;

        _rightStick.style.left = 65f;
        _rightStick.style.top = 65f;

        // yield return new WaitForSeconds(0.5f);

        // _calibrationInstructionsHeading.text = "Left Stick";
        // _calibrationInstructionsDescription.text = "Move the left stick up to mimic the movement on screen.";
        // _leftStick.style.left = 130;

        // yield return new WaitForSeconds(1f);

        // _calibrationInstructionsDescription.text = "Center stick";
        // _leftStick.style.left = 65;

        // yield return new WaitForSeconds(1f);

        // _calibrationInstructionsDescription.text = "Move the left stick to the right to mimic the movement on screen.";
        // _leftStick.style.top = 0;

        // yield return new WaitForSeconds(1f);

        // _calibrationInstructionsDescription.text = "Center stick";
        // _leftStick.style.top = 65;

        // yield return new WaitForSeconds(1f);

        // _calibrationInstructionsHeading.text = "Right Stick";
        // _calibrationInstructionsDescription.text = "Move the right stick up to mimic the movement on screen.";
        // _rightStick.style.left = 130;

        // yield return new WaitForSeconds(1f);

        // _calibrationInstructionsDescription.text = "Center stick";
        // _rightStick.style.left = 65;

        // yield return new WaitForSeconds(1f);

        // _calibrationInstructionsDescription.text = "Move the right stick to the right to mimic the movement on screen.";
        // _rightStick.style.top = 0;

        // yield return new WaitForSeconds(1f);

        // _calibrationInstructionsDescription.text = "Center stick";
        // _rightStick.style.top = 65;

        // yield return new WaitForSeconds(1f);

        // _calibrationInstructionsHeading.text = "Roll";
        // _calibrationInstructionsDescription.text = "Move the roll stick to the right.";
        
        // yield return new WaitForSeconds(1f);

        // _calibrationInstructionsDescription.text = "Center stick.";
        
        // yield return new WaitForSeconds(1f);

        // _calibrationInstructionsHeading.text = "Pitch";
        // _calibrationInstructionsDescription.text = "Move the pitch stick up.";
        
        // yield return new WaitForSeconds(1f);

        // _calibrationInstructionsDescription.text = "Center stick.";

        // yield return new WaitForSeconds(1f);

        // _calibrationInstructionsHeading.text = "Yaw";
        // _calibrationInstructionsDescription.text = "Move the yaw stick to the right.";
        
        // yield return new WaitForSeconds(1f);
        
        // _calibrationInstructionsDescription.text = "Center stick.";

        // yield return new WaitForSeconds(1f);

        // _calibrationInstructionsHeading.text = "Throttle";
        // _calibrationInstructionsDescription.text = "Move the Throttle stick up.";
        
        // yield return new WaitForSeconds(1f);

        // _calibrationInstructionsDescription.text = "Center stick.";    
    }

    private void SetTransition(VisualElement element)
    {
        element.style.transitionProperty = new List<StylePropertyName>
        {
            new StylePropertyName("top"),
            new StylePropertyName("left")
        };

        element.style.transitionDuration = new List<TimeValue>
        {
            new TimeValue(0.5f, TimeUnit.Second),
            new TimeValue(0.5f, TimeUnit.Second)
        };

        element.style.transitionTimingFunction = new List<EasingFunction>
        {
            new EasingFunction(EasingMode.EaseInOut),
            new EasingFunction(EasingMode.EaseInOut)
        };
    }
}


