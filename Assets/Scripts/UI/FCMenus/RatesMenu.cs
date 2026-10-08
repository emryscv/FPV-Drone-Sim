using UnityEngine;
using UnityEngine.UIElements;

public class RatesMenu : MonoBehaviour
{
    [SerializeField] private FlightController _flightController;

    private UIDocument _uiManager;
    private VisualElement _FCMenuTab;

    //Betaflight Rates
    private FloatField _RollRCRate;
    private FloatField _PitchRCRate;
    private FloatField _YawRCRate;

    private FloatField _RollRate;
    private FloatField _PitchRate;
    private FloatField _YawRate;

    //Actual Rates
    private IntegerField _RollCenter;
    private IntegerField _PitchCenter;
    private IntegerField _YawCenter;

    private IntegerField _RollMaxRate;
    private IntegerField _PitchMaxRate;
    private IntegerField _YawMaxRate;

    //RC Expo both systems
    private FloatField _RollRCExpo;
    private FloatField _PitchRCExpo;
    private FloatField _YawRCExpo;

    //Max Velocity in Deg per second
    private Label _RollMaxVel;
    private Label _PitchMaxVel;
    private Label _YawMaxVel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        _uiManager = GetComponent<UIDocument>();
        _FCMenuTab = _uiManager.rootVisualElement.Q<VisualElement>("FCMenuTab");

        //Betaflight Rates
        _RollRCRate = _FCMenuTab.Q<FloatField>("RollRCRate");
        _PitchRCRate = _FCMenuTab.Q<FloatField>("PitchRCRate");
        _YawRCRate = _FCMenuTab.Q<FloatField>("YawRCRate");

        _RollRate = _FCMenuTab.Q<FloatField>("RollRate");
        _PitchRate = _FCMenuTab.Q<FloatField>("PitchRate");
        _YawRate = _FCMenuTab.Q<FloatField>("YawRate");

        //Actual Rates
        // _RollCenter = _FCMenuTab.Q<IntegerField>("RollCenter");
        // _PitchCenter = _FCMenuTab.Q<IntegerField>("PitchCenter");
        // _YawCenter = _FCMenuTab.Q<IntegerField>("YawCenter");

        // _RollMaxRate = _FCMenuTab.Q<IntegerField>("RollMaxRate");
        // _PitchMaxRate = _FCMenuTab.Q<IntegerField>("PitchMaxRate");
        // _YawMaxRate = _FCMenuTab.Q<IntegerField>("YawMaxRate");

        //RC Expo both systems
        _RollRCExpo = _FCMenuTab.Q<FloatField>("RollExpo");
        _PitchRCExpo = _FCMenuTab.Q<FloatField>("PitchExpo");
        _YawRCExpo = _FCMenuTab.Q<FloatField>("YawExpo");

        //Max Velocity
        _RollMaxVel = _FCMenuTab.Q<Label>("RollMaxVel");
        _PitchMaxVel = _FCMenuTab.Q<Label>("PitchMaxVel");
        _YawMaxVel = _FCMenuTab.Q<Label>("YawMaxVel");

        //Event Handlers
        _RollRCRate.RegisterCallback<ChangeEvent<string>>(OnRollRCRateChanged);
        _PitchRCRate.RegisterCallback<ChangeEvent<string>>(OnPitchRCRateChanged);
        _YawRCRate.RegisterCallback<ChangeEvent<string>>(OnYawRCRateChanged);
        
        _RollRate.RegisterCallback<ChangeEvent<string>>(OnRollRateChanged);
        _PitchRate.RegisterCallback<ChangeEvent<string>>(OnPitchRateChanged);
        _YawRate.RegisterCallback<ChangeEvent<string>>(OnYawRateChanged);

        _RollRCExpo.RegisterCallback<ChangeEvent<string>>(OnRollRCExpoChanged);
        _PitchRCExpo.RegisterCallback<ChangeEvent<string>>(OnPitchRCExpoChanged);
        _YawRCExpo.RegisterCallback<ChangeEvent<string>>(OnYawRCExpoChanged);

        _RollRCRate.Set(_flightController.RCRates[2]);
        _PitchRCRate.Set(_flightController.RCRates[0]);
        _YawRCRate.Set(_flightController.RCRates[1]);

        _RollRate.Set(_flightController.Rates[2]);
        _PitchRate.Set(_flightController.Rates[0]);
        _YawRate.Set(_flightController.Rates[1]);

        _RollRCExpo.Set(_flightController.RCExpo[2]);
        _PitchRCExpo.Set(_flightController.RCExpo[0]);
        _YawRCExpo.Set(_flightController.RCExpo[1]);

        _RollMaxVel.text = _flightController.ComputeRate(2, 1).ToString();
        _PitchMaxVel.text = _flightController.ComputeRate(0, 1).ToString();
        _YawMaxVel.text = _flightController.ComputeRate(1, 1).ToString();
    }

    private void OnDisable()
    {
        _RollRCRate.UnregisterCallback<ChangeEvent<string>>(OnRollRCRateChanged);
        _PitchRCRate.UnregisterCallback<ChangeEvent<string>>(OnPitchRCRateChanged);
        _YawRCRate.UnregisterCallback<ChangeEvent<string>>(OnYawRCRateChanged);
        
        _RollRate.UnregisterCallback<ChangeEvent<string>>(OnRollRateChanged);
        _PitchRate.UnregisterCallback<ChangeEvent<string>>(OnPitchRateChanged);
        _YawRate.UnregisterCallback<ChangeEvent<string>>(OnYawRateChanged);

        _RollRCExpo.UnregisterCallback<ChangeEvent<string>>(OnRollRCExpoChanged);
        _PitchRCExpo.UnregisterCallback<ChangeEvent<string>>(OnPitchRCExpoChanged);
        _YawRCExpo.UnregisterCallback<ChangeEvent<string>>(OnYawRCExpoChanged);
    }

    private void OnRollRCRateChanged(ChangeEvent<string> evt)
    {
        Debug.Log("Roll RC Rate changed to: " + evt.newValue);
        _flightController.SetRCRate(2, float.Parse(evt.newValue));
    }
    
    private void OnPitchRCRateChanged(ChangeEvent<string> evt)
    {
        Debug.Log("Pitch RC Rate changed to: " + evt.newValue);
        _flightController.SetRCRate(0, float.Parse(evt.newValue));
    }
    
    private void OnYawRCRateChanged(ChangeEvent<string> evt)
    {
        Debug.Log("Yaw RC Rate changed to: " + evt.newValue);
        _flightController.SetRCRate(1, float.Parse(evt.newValue));
    }

    private void OnRollRateChanged(ChangeEvent<string> evt)
    {
        Debug.Log("Roll Rate changed to: " + evt.newValue);
        _flightController.SetRate(2, float.Parse(evt.newValue));       
    }
    
    private void OnPitchRateChanged(ChangeEvent<string> evt)
    {
        Debug.Log("Pitch Rate changed to: " + evt.newValue);
        _flightController.SetRate(0, float.Parse(evt.newValue));
    }
    
    private void OnYawRateChanged(ChangeEvent<string> evt)
    {
        Debug.Log("Yaw Rate changed to: " + evt.newValue);
        _flightController.SetRate(1, float.Parse(evt.newValue));
    }

    private void OnRollRCExpoChanged(ChangeEvent<string> evt)
    {
        Debug.Log("Roll RC Expo changed to: " + evt.newValue);
        _flightController.SetExpo(2, float.Parse(evt.newValue));
    }
    
    private void OnPitchRCExpoChanged(ChangeEvent<string> evt)
    {
        Debug.Log("Pitch RC Expo changed to: " + evt.newValue);
        _flightController.SetExpo(0, float.Parse(evt.newValue));
    }
    
    private void OnYawRCExpoChanged(ChangeEvent<string> evt)
    {
        Debug.Log("Yaw RC Expo changed to: " + evt.newValue);
        _flightController.SetExpo(1, float.Parse(evt.newValue));
    }
}
    