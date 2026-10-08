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
    private FloatField _RollMaxVel;
    private FloatField _PitchMaxVel;
    private FloatField _YawMaxVel;

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
        _PitchRCExpo = _FCMenuTab.Q<FloatField>("PitcExpo");
        _YawRCExpo = _FCMenuTab.Q<FloatField>("YawExpo");

        //Max Velocity
        _RollMaxVel = _FCMenuTab.Q<FloatField>("RollMaxVel");
        _PitchMaxVel = _FCMenuTab.Q<FloatField>("PitchMaxVel");
        _YawMaxVel = _FCMenuTab.Q<FloatField>("YawMaxVel");

        //Event Handlers
        _RollRCRate.RegisterCallback<ChangeEvent<float>>(OnRollRCRateChanged);
        _PitchRCRate.RegisterCallback<ChangeEvent<float>>(OnPitchRCRateChanged);
        _YawRCRate.RegisterCallback<ChangeEvent<float>>(OnYawRCRateChanged);
        
        _RollRate.RegisterCallback<ChangeEvent<float>>(OnRollRateChanged);
        _PitchRate.RegisterCallback<ChangeEvent<float>>(OnPitchRateChanged);
        _YawRate.RegisterCallback<ChangeEvent<float>>(OnYawRateChanged);

        _RollRCExpo.RegisterCallback<ChangeEvent<float>>(OnRollRCExpoChanged);
        _PitchRCExpo.RegisterCallback<ChangeEvent<float>>(OnPitchRCExpoChanged);
        _YawRCExpo.RegisterCallback<ChangeEvent<float>>(OnYawRCExpoChanged);

        _RollMaxVel.RegisterCallback<ChangeEvent<float>>(OnRollMaxVelChanged);
        _PitchMaxVel.RegisterCallback<ChangeEvent<float>>(OnPitchMaxVelChanged);
        _YawMaxVel.RegisterCallback<ChangeEvent<float>>(OnYawMaxVelChanged);
    }

    private void OnDisable()
    {
        _RollRCRate.UnregisterCallback<ChangeEvent<float>>(OnRollRCRateChanged);
        _PitchRCRate.UnregisterCallback<ChangeEvent<float>>(OnPitchRCRateChanged);
        _YawRCRate.UnregisterCallback<ChangeEvent<float>>(OnYawRCRateChanged);
        
        _RollRate.UnregisterCallback<ChangeEvent<float>>(OnRollRateChanged);
        _PitchRate.UnregisterCallback<ChangeEvent<float>>(OnPitchRateChanged);
        _YawRate.UnregisterCallback<ChangeEvent<float>>(OnYawRateChanged);

        _RollRCExpo.UnregisterCallback<ChangeEvent<float>>(OnRollRCExpoChanged);
        _PitchRCExpo.UnregisterCallback<ChangeEvent<float>>(OnPitchRCExpoChanged);
        _YawRCExpo.UnregisterCallback<ChangeEvent<float>>(OnYawRCExpoChanged);

        _RollMaxVel.UnregisterCallback<ChangeEvent<float>>(OnRollMaxVelChanged);
        _PitchMaxVel.UnregisterCallback<ChangeEvent<float>>(OnPitchMaxVelChanged);
        _YawMaxVel.UnregisterCallback<ChangeEvent<float>>(OnYawMaxVelChanged);
    }

    private void OnRollRCRateChanged(ChangeEvent<float> evt){}
    
    private void OnPitchRCRateChanged(ChangeEvent<float> evt){}
    
    private void OnYawRCRateChanged(ChangeEvent<float> evt){}

    private void OnRollRateChanged(ChangeEvent<float> evt){}
    
    private void OnPitchRateChanged(ChangeEvent<float> evt){}
    
    private void OnYawRateChanged(ChangeEvent<float> evt){}

    private void OnRollRCExpoChanged(ChangeEvent<float> evt){}
    
    private void OnPitchRCExpoChanged(ChangeEvent<float> evt){}
    
    private void OnYawRCExpoChanged(ChangeEvent<float> evt){}

    private void OnRollMaxVelChanged(ChangeEvent<float> evt){}
    
    private void OnPitchMaxVelChanged(ChangeEvent<float> evt){}
    
    private void OnYawMaxVelChanged(ChangeEvent<float> evt){}
}
    