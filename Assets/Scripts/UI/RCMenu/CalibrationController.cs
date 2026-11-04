using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using System;

public sealed class CalibrationController
{
    private readonly StartCoroutine HostStartCoroutine;
    private readonly StopCoroutine HostStopCoroutine;

    private readonly RCControllerManager _controls;

    private readonly VisualElement _leftStick;
    private readonly VisualElement _rightStick;

    private readonly Button _undoBtn;

    private readonly Label _instructionsHeading;
    private readonly Label _instructionsDescription;

    private readonly Button _yesBtn;
    private readonly Button _noBtn;

    private readonly Button _startCalibrationBtn;
    private readonly Button _saveBtn;

    private bool _moveWithInput = true;
    private bool _onInputDiscovery;
    private bool _buttonPressed;
    private bool _isPositive;

    private Coroutine _calibrationRoutine;
    private CalibrationStateMachine _calibrationSteps;

    private readonly int[] _xPositions = { 0, 130, 130, 0 };
    private readonly int[] _yPositions = { 0, 0, 130, 130 };

    public bool MoveWithInput => _moveWithInput;
    public bool IsInputDiscovery => _onInputDiscovery;

    public CalibrationController(
        StartCoroutine startCoroutine,
        StopCoroutine stopCoroutine,
        RCControllerManager controls,
        VisualElement leftStick,
        VisualElement rightStick,
        Button undoBtn,
        Label instructionsHeading,
        Label instructionsDescription,
        Button yesBtn,
        Button noBtn,
        Button startCalibrationBtn,
        Button saveBtn)
    {
        HostStartCoroutine = startCoroutine;
        HostStopCoroutine = stopCoroutine;
        _controls = controls;
        _startCalibrationBtn = startCalibrationBtn;
        _saveBtn = saveBtn;
        _undoBtn = undoBtn;
        _instructionsHeading = instructionsHeading;
        _instructionsDescription = instructionsDescription;
        _yesBtn = yesBtn;
        _noBtn = noBtn;
        _leftStick = leftStick;
        _rightStick = rightStick;
        _calibrationSteps = CreateCalibrationSteps();
    }

    public void Start()
    {
        if (_calibrationRoutine != null)
        {
            HostStopCoroutine(_calibrationRoutine);
            Reset(true);
        }

        _calibrationSteps.Reset();
        _calibrationRoutine = HostStartCoroutine(_calibrationSteps.Start.RoutineFactory());
        _startCalibrationBtn.text = "RESTART CALIBRATION";

    }

    public void Stop()
    {
        if (_calibrationRoutine != null)
        {
            HostStopCoroutine(_calibrationRoutine);
            Reset(true);
        }
    }

    public void Reset(bool force)
    {
        //TODO probably need to change it for the state machine 
        _calibrationRoutine = null;
        _startCalibrationBtn.text = "START CALIBRATION";
        _instructionsHeading.text = "READY TO CALIBRATE";
        _instructionsDescription.text = "Calibration maps your physical controller's axis range so the simulator receives accurate inputs. Press Start Calibration to begin the 2-step process.";

        _yesBtn.style.display = DisplayStyle.None;
        _noBtn.style.display = DisplayStyle.None;
        _undoBtn.style.display = DisplayStyle.None;

        if (force)
        {
            SetTransition(_leftStick, 0f, EasingMode.Linear);
            SetTransition(_rightStick, 0f, EasingMode.Linear);

            _leftStick.style.left = 65f;
            _leftStick.style.top = 65f;

            _rightStick.style.left = 65f;
            _rightStick.style.top = 65f;

            _moveWithInput = true;
            _onInputDiscovery = false;
            _buttonPressed = false;
            _isPositive = false;
        }

        Helpers.SetButtonState(_saveBtn, !force);
    }

    public void Undo()
    {
        HostStopCoroutine(_calibrationRoutine);
        _calibrationRoutine = null;
        _calibrationSteps.Current.OnUndo?.Invoke();

        _calibrationSteps.MovePrevious();
        _calibrationRoutine = HostStartCoroutine(_calibrationSteps.Current.RoutineFactory());

        if (_calibrationSteps.Start == _calibrationSteps.Current)
        {
            _undoBtn.style.display = DisplayStyle.None;
        }

    }

    public void SetPositiveButtonPressed(bool isPositive)
    {
        _buttonPressed = true;
        _isPositive = isPositive;
    }

    private IEnumerator CalibrationDiscoveryStep()
    {
        Debug.Log("Starting calibration discovery step.");
        _controls.FindAllAxes();

        _moveWithInput = false;
        _instructionsHeading.text = "CALIBRATING! ... ";
        _instructionsDescription.text = "Move your controller sticks to mimic the movement on screen.";

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

        _controls.PruneAxes();

        _instructionsDescription.text = "Center stick";
        yield return new WaitForSecondsRealtime(2f);
        _undoBtn.style.display = DisplayStyle.Flex;

        StartNextCalibrationStep();
    }

    private IEnumerator LeftStickXStep() => AxisStep("Left Stick", "Move the left stick to the right to mimic the movement on screen.", _leftStick, true, "LStickX");
    private IEnumerator LeftStickYStep() => AxisStep("Left Stick", "Move the left stick up to mimic the movement on screen.", _leftStick, false, "LStickY");
    private IEnumerator RightStickXStep() => AxisStep("Right Stick", "Move the right stick to the right to mimic the movement on screen.", _rightStick, true, "RStickX");
    private IEnumerator RightStickYStep() => AxisStep("Right Stick", "Move the right stick up to mimic the movement on screen.", _rightStick, false, "RStickY");
    private IEnumerator RollStep() => AxisStep("Roll", "Move the roll stick to the right.", null, true, "Roll");
    private IEnumerator PitchStep() => AxisStep("Pitch", "Move the pitch stick up.", null, false, "Pitch");
    private IEnumerator YawStep() => AxisStep("Yaw", "Move the yaw stick to the right.", null, true, "Yaw");
    private IEnumerator ThrottleStep() => AxisStep("Throttle", "Move the Throttle stick up.", null, false, "Throttle");
    private IEnumerator InvertInputStep()
    {
        _moveWithInput = true;

        SetTransition(_leftStick, 0f, EasingMode.Linear);
        SetTransition(_rightStick, 0f, EasingMode.Linear);

        _yesBtn.style.display = DisplayStyle.Flex;
        _noBtn.style.display = DisplayStyle.Flex;

        _instructionsHeading.text = "Invert Input";
        _instructionsDescription.text = "Do you want to invert any input?";

        yield return WaitForButtonPress();
        if (!_isPositive)
        {
            _controls.PrintDebug();
            Reset(false);
            yield break;
        }

        StartNextCalibrationStep();
    }

    private IEnumerator InvertRollStep() => InvertAxisStep("Invert Roll", "Do you want to invert roll?", "Roll");
    private IEnumerator InvertPitchStep() => InvertAxisStep("Invert Pitch", "Do you want to invert pitch?", "Pitch");
    private IEnumerator InvertYawStep() => InvertAxisStep("Invert Yaw", "Do you want to invert yaw?", "Yaw");
    private IEnumerator InvertThrottleStep() => InvertAxisStep("Invert Throttle", "Do you want to invert throttle?", "Throttle");

    private IEnumerator AxisStep(string heading, string instruction, VisualElement stick, bool horizontal, string axis)
    {
        _instructionsHeading.text = heading;
        _instructionsDescription.text = instruction;
        if (stick != null)
        {
            if (horizontal) stick.style.left = 130;
            else stick.style.top = 0;
        }

        yield return WaitForAxisDetection();
        _controls.SetAxis(axis);
        yield return new WaitForSecondsRealtime(0.5f);

        _instructionsDescription.text = "Center stick.";
        if (stick != null)
        {
            if (horizontal) stick.style.left = 65;
            else stick.style.top = 65;
        }

        yield return WaitForCenter();
        yield return new WaitForSecondsRealtime(0.5f);
        StartNextCalibrationStep();
    }

    private IEnumerator InvertAxisStep(string heading, string instruction, string axis)
    {
        _instructionsHeading.text = heading;
        _instructionsDescription.text = instruction;

        yield return WaitForButtonPress();
        _controls.InvertAxis(axis, _isPositive);

        StartNextCalibrationStep();
    }

    private void StartNextCalibrationStep()
    {
        if (_calibrationSteps.MoveNext())
        {
            _calibrationRoutine = HostStartCoroutine(_calibrationSteps.Current.RoutineFactory());
        }
        else
        {
            Reset(false);
        }
    }

    private IEnumerator WaitForAxisDetection()
    {
        while (!_controls.TryReadAxis())
        {
            yield return null;
        }
    }

    private IEnumerator WaitForCenter()
    {
        while (!_controls.IsCentered())
        {
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
    private void UndoLeftStickX()
    {
        _leftStick.style.left = 65f;
    }

    private void UndoLeftStickY()
    {
        _leftStick.style.top = 65f;
    }

    private void UndoRightStickX()
    {
        _rightStick.style.left = 65f;
    }

    private void UndoRightStickY()
    {
        _rightStick.style.top = 65f;
    }

    private void UndoInvertInput()
    {

        _buttonPressed = false;
        _isPositive = false;
        _yesBtn.style.display = DisplayStyle.None;
        _noBtn.style.display = DisplayStyle.None;

        _moveWithInput = false;
        _leftStick.style.left = 65f;
        _leftStick.style.top = 65f;
        _rightStick.style.left = 65f;
        _rightStick.style.top = 65f;
    }

    private void UndoInvertRoll()
    {
        _controls.InvertAxis("Roll", false);
    }
    
    private void UndoInvertPitch()
    {
        _controls.InvertAxis("Pitch", false);
    }

    private void UndoInvertYaw()
    {
        _controls.InvertAxis("Yaw", false);
    }

    private void UndoInvertThrottle()
    {
        _controls.InvertAxis("Throttle", false);
    }


    private CalibrationStateMachine CreateCalibrationSteps()
    {
        CalibrationStep discovery = new(CalibrationDiscoveryStep);
        CalibrationStep leftStickX = new(LeftStickXStep, UndoLeftStickX);
        CalibrationStep leftStickY = new(LeftStickYStep, UndoLeftStickY);
        CalibrationStep rightStickX = new(RightStickXStep, UndoRightStickX);
        CalibrationStep rightStickY = new(RightStickYStep, UndoRightStickY);
        CalibrationStep roll = new(RollStep, null);
        CalibrationStep pitch = new(PitchStep, null);
        CalibrationStep yaw = new(YawStep, null);
        CalibrationStep throttle = new(ThrottleStep, null);
        CalibrationStep invertInput = new(InvertInputStep, UndoInvertInput);
        CalibrationStep invertRoll = new(InvertRollStep, UndoInvertRoll);
        CalibrationStep invertPitch = new(InvertPitchStep, UndoInvertPitch);
        CalibrationStep invertYaw = new(InvertYawStep, UndoInvertYaw);
        CalibrationStep invertThrottle = new(InvertThrottleStep, UndoInvertThrottle);

        CalibrationStateMachine.Link(discovery, leftStickX);
        CalibrationStateMachine.Link(leftStickX, leftStickY);
        CalibrationStateMachine.Link(leftStickY, rightStickX);
        CalibrationStateMachine.Link(rightStickX, rightStickY);
        CalibrationStateMachine.Link(rightStickY, roll);
        CalibrationStateMachine.Link(roll, pitch);
        CalibrationStateMachine.Link(pitch, yaw);
        CalibrationStateMachine.Link(yaw, throttle);
        CalibrationStateMachine.Link(throttle, invertInput);
        CalibrationStateMachine.Link(invertInput, invertRoll);
        CalibrationStateMachine.Link(invertRoll, invertPitch);
        CalibrationStateMachine.Link(invertPitch, invertYaw);
        CalibrationStateMachine.Link(invertYaw, invertThrottle);

        return new CalibrationStateMachine(discovery);
    }
}
