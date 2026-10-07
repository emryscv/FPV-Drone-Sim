using UnityEngine;
using UnityEngine.UIElements;

public class FCMenuController : MonoBehaviour
{
    [SerializeField] private FlightController _flightController;

    private UIDocument _uiManager;
    private VisualElement _FCMenuTab;

    private EnumField _RateType;

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

    //Throttle
    private FloatField _ThrottleMid;
    private FloatField _ThrottleExpo;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        _uiManager = GetComponent<UIDocument>();
        _FCMenuTab = _uiManager.rootVisualElement.Q<VisualElement>("FCMenuTab");

        _RateType = _FCMenuTab.Q<EnumField>("RateTypeSelector");
        
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
        _RollRCExpo = _FCMenuTab.Q<FloatField>("RollRCExpo");
        _PitchRCExpo = _FCMenuTab.Q<FloatField>("PitchRCExpo");
        _YawRCExpo = _FCMenuTab.Q<FloatField>("YawRCExpo");

        //Max Velocity
        _RollMaxVel = _FCMenuTab.Q<FloatField>("RollMaxVel");
        _PitchMaxVel = _FCMenuTab.Q<FloatField>("PitchMaxVel");
        _YawMaxVel = _FCMenuTab.Q<FloatField>("YawMaxVel");

        //Throttle
        _ThrottleMid = _FCMenuTab.Q<FloatField>("ThrottleMid");
        _ThrottleExpo = _FCMenuTab.Q<FloatField>("ThrottleExpo");

        _RateType.Init(RateType.Betaflight);
        _RateType.RegisterCallback<ChangeEvent<EnumField>>(OnRateTypeChanged);

        _RollP.RegisterCallback<ChangeEvent<int>>(OnRollPChanged);
        _PitchP.RegisterCallback<ChangeEvent<int>>(OnPitchPChanged);
        _YawP.RegisterCallback<ChangeEvent<int>>(OnYawPChanged);

        _RollI.RegisterCallback<ChangeEvent<int>>(OnRollIChanged);
        _PitchI.RegisterCallback<ChangeEvent<int>>(OnPitchIChanged);
        _YawI.RegisterCallback<ChangeEvent<int>>(OnYawIChanged);    
        
        _RollD.RegisterCallback<ChangeEvent<int>>(OnRollDChanged);
        _PitchD.RegisterCallback<ChangeEvent<int>>(OnPitchDChanged);
        _YawD.RegisterCallback<ChangeEvent<int>>(OnYawDChanged);
        
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

        _ThrottleMid.RegisterCallback<ChangeEvent<float>>(OnThrottleMidChanged);
        _ThrottleExpo.RegisterCallback<ChangeEvent<float>>(OnThrottleExpoChanged);

    }

    private void OnDisable()
    {
        _RateType.UnregisterCallback<ChangeEvent<EnumField>>(OnRateTypeChanged);
        _RollP.UnregisterCallback<ChangeEvent<int>>(OnRollPChanged);
        _PitchP.UnregisterCallback<ChangeEvent<int>>(OnPitchPChanged);
        _YawP.UnregisterCallback<ChangeEvent<int>>(OnYawPChanged);

        _RollI.UnregisterCallback<ChangeEvent<int>>(OnRollIChanged);
        _PitchI.UnregisterCallback<ChangeEvent<int>>(OnPitchIChanged);
        _YawI.UnregisterCallback<ChangeEvent<int>>(OnYawIChanged);    
        
        _RollD.UnregisterCallback<ChangeEvent<int>>(OnRollDChanged);
        _PitchD.UnregisterCallback<ChangeEvent<int>>(OnPitchDChanged);
        _YawD.UnregisterCallback<ChangeEvent<int>>(OnYawDChanged);
        
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

        _ThrottleMid.UnregisterCallback<ChangeEvent<float>>(OnThrottleMidChanged);
        _ThrottleExpo.UnregisterCallback<ChangeEvent<float>>(OnThrottleExpoChanged);
    }

    private void OnRateTypeChanged(ChangeEvent<EnumField> evt)
    {
        _flightController.SetRateType((RateType)_RateType.value);
    }

    private void OnRollPChanged(ChangeEvent<int> evt){}
    
    private void OnPitchPChanged(ChangeEvent<int> evt){}
    
    private void OnYawPChanged(ChangeEvent<int> evt){}

    private void OnRollIChanged(ChangeEvent<int> evt){}

    private void OnPitchIChanged(ChangeEvent<int> evt){}
    
    private void OnYawIChanged(ChangeEvent<int> evt){}

    private void OnRollDChanged(ChangeEvent<int> evt){}
    
    private void OnPitchDChanged(ChangeEvent<int> evt){}
    
    private void OnYawDChanged(ChangeEvent<int> evt){}

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

    private void OnThrottleMidChanged(ChangeEvent<float> evt){}
    
    private void OnThrottleExpoChanged(ChangeEvent<float> evt){}

}
    