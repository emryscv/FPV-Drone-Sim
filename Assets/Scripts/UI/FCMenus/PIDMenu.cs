using UnityEngine;
using UnityEngine.UIElements;

public class PIDMenu : MonoBehaviour
{
    [SerializeField] private FlightController _flightController;

    private UIDocument _uiManager;
    private VisualElement _FCMenuTab;

    //PID
    private IntegerField _RollP;
    private IntegerField _PitchP;
    private IntegerField _YawP;

    private IntegerField _RollI;
    private IntegerField _PitchI;
    private IntegerField _YawI;

    private IntegerField _RollD;
    private IntegerField _PitchD;
    private IntegerField _YawD;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        _uiManager = GetComponent<UIDocument>();
        _FCMenuTab = _uiManager.rootVisualElement.Q<VisualElement>("FCMenuTab");
 
        //PID
        _RollP = _FCMenuTab.Q<IntegerField>("RollP");
        _PitchP = _FCMenuTab.Q<IntegerField>("PitchP");
        _YawP = _FCMenuTab.Q<IntegerField>("YawP");

        _RollI = _FCMenuTab.Q<IntegerField>("RollI");
        _PitchI = _FCMenuTab.Q<IntegerField>("PitchI");
        _YawI = _FCMenuTab.Q<IntegerField>("YawI");

        _RollD = _FCMenuTab.Q<IntegerField>("RollD");
        _PitchD = _FCMenuTab.Q<IntegerField>("PitchD");
        _YawD = _FCMenuTab.Q<IntegerField>("YawD");

        //Event Handlers
        _RollP.RegisterValueChangedCallback(OnRollPChanged);
        _PitchP.RegisterValueChangedCallback(OnPitchPChanged);
        _YawP.RegisterValueChangedCallback(OnYawPChanged);

        _RollI.RegisterValueChangedCallback(OnRollIChanged);
        _PitchI.RegisterValueChangedCallback(OnPitchIChanged);
        _YawI.RegisterValueChangedCallback(OnYawIChanged);

        _RollD.RegisterValueChangedCallback(OnRollDChanged);
        _PitchD.RegisterValueChangedCallback(OnPitchDChanged);
        _YawD.RegisterValueChangedCallback(OnYawDChanged);
    
        // Initialize the fields with the current values from the flight controller
        _RollP.Set((int)_flightController.KP[2]);
        _PitchP.Set((int)_flightController.KP[0]);
        _YawP.Set((int)_flightController.KP[1]);

        _RollI.Set((int)_flightController.KI[2]);
        _PitchI.Set((int)_flightController.KI[0]);
        _YawI.Set((int)_flightController.KI[1]);

        _RollD.Set((int)_flightController.KD[2]);
        _PitchD.Set((int)_flightController.KD[0]);
        _YawD.Set((int)_flightController.KD[1]);
    }

    private void OnDisable()
    {
        _RollP.UnregisterValueChangedCallback();
        _PitchP.UnregisterValueChangedCallback();
        _YawP.UnregisterValueChangedCallback();

        _RollI.UnregisterValueChangedCallback();
        _PitchI.UnregisterValueChangedCallback();
        _YawI.UnregisterValueChangedCallback();

        _RollD.UnregisterValueChangedCallback();
        _PitchD.UnregisterValueChangedCallback();
        _YawD.UnregisterValueChangedCallback();
    }

    private void OnRollPChanged(ChangeEvent<string> evt)
    {
        Debug.Log("Roll P changed to: " + evt.newValue);
        _flightController.SetP(2, float.Parse(evt.newValue));
    }
    
    private void OnPitchPChanged(ChangeEvent<string> evt)
    {
        Debug.Log("Pitch P changed to: " + evt.newValue);
        _flightController.SetP(0, float.Parse(evt.newValue));
    }
    
    private void OnYawPChanged(ChangeEvent<string> evt)
    {
        Debug.Log("Yaw P changed to: " + evt.newValue);
        _flightController.SetP(1, float.Parse(evt.newValue));
    }

    private void OnRollIChanged(ChangeEvent<string> evt)
    {
        Debug.Log("Roll I changed to: " + evt.newValue);
        _flightController.SetI(2, float.Parse(evt.newValue));
    }

    private void OnPitchIChanged(ChangeEvent<string> evt)
    {
        Debug.Log("Pitch I changed to: " + evt.newValue);
        _flightController.SetI(0, float.Parse(evt.newValue));
    }
    
    private void OnYawIChanged(ChangeEvent<string> evt)
    {
        Debug.Log("Yaw I changed to: " + evt.newValue);
        _flightController.SetI(1, float.Parse(evt.newValue));
    }

    private void OnRollDChanged(ChangeEvent<string> evt)
    {
        Debug.Log("Roll D changed to: " + evt.newValue);
        _flightController.SetD(2, float.Parse(evt.newValue));
    }
    
    private void OnPitchDChanged(ChangeEvent<string> evt)
    {
        Debug.Log("Pitch D changed to: " + evt.newValue);
        _flightController.SetD(0, float.Parse(evt.newValue));
    }
    
    private void OnYawDChanged(ChangeEvent<string> evt)
    {
        Debug.Log("Yaw D changed to: " + evt.newValue);
        _flightController.SetD(1, float.Parse(evt.newValue));
    }
}
    