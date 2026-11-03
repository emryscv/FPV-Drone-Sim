using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HUDController : MonoBehaviour
{

    [SerializeField] private Rigidbody _rb;

    private RCControllerManager controls;

    private UIDocument _uiManager;
    private VisualElement _HUD;

    private VisualElement _OSD;

    private Label _speed;
    private Label _altitude;

    private VisualElement _leftStick;
    private VisualElement _rightStick;

    private VisualElement _crashIndicator;
    
    private Label _timerLabel;

    private const float COUNTDOWN_LARGE_FONT_SIZE = 150f;
    private const float COUNTDOWN_RESTING_FONT_SIZE = 100f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        controls = RCControllerManager.Instance;
        GameEvents.Instance.OnDisplayCrashIndicator += ShowCrashIndicator;
        GameEvents.Instance.OnHideCrashIndicator += HideCrashIndicator;
        GameEvents.Instance.OnTimerStart += StartCountdown;

        //TODO unsubscribe from events in OnDestroy
    }

    private void OnEnable()
    {
        _uiManager = GetComponent<UIDocument>();
        _HUD = _uiManager.rootVisualElement.Q<VisualElement>("HUD");
        
        _OSD = _HUD.Q<VisualElement>("OSD");
        
        _leftStick = _HUD.Q<VisualElement>("LeftStick");
        _rightStick = _HUD.Q<VisualElement>("RightStick");

        _speed = _HUD.Q<Label>("Speed");
        _altitude = _HUD.Q<Label>("Altitude");
       
        _crashIndicator = _HUD.Q<VisualElement>("CrashBadge");

        _timerLabel = _HUD.Q<Label>("TimerLabel");
    }

    // Update is called once per frame
    void Update()
    {
        if (controls.registeredDevice == null) return;
        
        float lStickY = controls.LStickY?.ReadValue() ?? 0f;
        float lStickX = controls.LStickX?.ReadValue() ?? 0f;
        float rStickY = controls.RStickY?.ReadValue() ?? 0f;
        float rStickX = controls.RStickX?.ReadValue() ?? 0f;

        //This equation is fixed to the size of the parent componenet. If the size change this has to change
        _leftStick.style.left = lStickX * 65f + 65f;
        _leftStick.style.top = 65f - lStickY * 65f;

        _rightStick.style.left = rStickX * 65f + 65f;
        _rightStick.style.top = 65f - rStickY * 65f;

        float altitude = _rb.transform.position.y * 3.281f; //ft
        float speed = _rb.linearVelocity.magnitude * 2.237f; //mph

        _speed.text = speed.ToString("F2");
        _altitude.text = altitude.ToString("F2");
    }

    private void ShowCrashIndicator()
    {
        _crashIndicator.style.display = DisplayStyle.Flex;
    }

    private void HideCrashIndicator()
    {
        _crashIndicator.style.display = DisplayStyle.None;
    }

    private void StartCountdown()
    {
        StartCoroutine(CountDownRoutine());
    }
    private IEnumerator CountDownRoutine()
    {
        _OSD.style.display = DisplayStyle.None;
        _timerLabel.style.display = DisplayStyle.Flex;

        int countdown = 3;
        while (countdown > 0)
        {
            _timerLabel.text = countdown.ToString();
            SetTimerFontSize(COUNTDOWN_LARGE_FONT_SIZE, 0f);

            // Let UI Toolkit apply the instant size before starting the shrink transition.
            yield return null;

            SetTimerFontSize(COUNTDOWN_RESTING_FONT_SIZE, 1f);
            yield return new WaitForSecondsRealtime(1f);
            countdown--;
        }    
        _timerLabel.text = "GO!";

        yield return new WaitForSeconds(1f);

        _OSD.style.display = DisplayStyle.Flex;
        _timerLabel.style.display = DisplayStyle.None;
    }

    private void SetTimerFontSize(float fontSize, float transitionDuration)
    {
        _timerLabel.style.transitionDuration = new List<TimeValue>
        {
            new TimeValue(transitionDuration, TimeUnit.Second)
        };
        _timerLabel.style.fontSize = fontSize;
    }
}
