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
        _RollP.RegisterCallback<ChangeEvent<int>>(OnRollPChanged);
        _PitchP.RegisterCallback<ChangeEvent<int>>(OnPitchPChanged);
        _YawP.RegisterCallback<ChangeEvent<int>>(OnYawPChanged);

        _RollI.RegisterCallback<ChangeEvent<int>>(OnRollIChanged);
        _PitchI.RegisterCallback<ChangeEvent<int>>(OnPitchIChanged);
        _YawI.RegisterCallback<ChangeEvent<int>>(OnYawIChanged);    
        
        _RollD.RegisterCallback<ChangeEvent<int>>(OnRollDChanged);
        _PitchD.RegisterCallback<ChangeEvent<int>>(OnPitchDChanged);
        _YawD.RegisterCallback<ChangeEvent<int>>(OnYawDChanged);
    }

    private void OnDisable()
    {
        _RollP.UnregisterCallback<ChangeEvent<int>>(OnRollPChanged);
        _PitchP.UnregisterCallback<ChangeEvent<int>>(OnPitchPChanged);
        _YawP.UnregisterCallback<ChangeEvent<int>>(OnYawPChanged);

        _RollI.UnregisterCallback<ChangeEvent<int>>(OnRollIChanged);
        _PitchI.UnregisterCallback<ChangeEvent<int>>(OnPitchIChanged);
        _YawI.UnregisterCallback<ChangeEvent<int>>(OnYawIChanged);    
        
        _RollD.UnregisterCallback<ChangeEvent<int>>(OnRollDChanged);
        _PitchD.UnregisterCallback<ChangeEvent<int>>(OnPitchDChanged);
        _YawD.UnregisterCallback<ChangeEvent<int>>(OnYawDChanged);
    }

    private void OnRollPChanged(ChangeEvent<int> evt)
    {
        Debug.Log("Roll P changed to: " + evt.newValue);
    }
    
    private void OnPitchPChanged(ChangeEvent<int> evt)
    {
        Debug.Log("Pitch P changed to: " + evt.newValue);
    }
    
    private void OnYawPChanged(ChangeEvent<int> evt)
    {
        Debug.Log("Yaw P changed to: " + evt.newValue);
    }

    private void OnRollIChanged(ChangeEvent<int> evt)
    {
        Debug.Log("Roll I changed to: " + evt.newValue);
    }

    private void OnPitchIChanged(ChangeEvent<int> evt)
    {
        Debug.Log("Pitch I changed to: " + evt.newValue);
    }
    
    private void OnYawIChanged(ChangeEvent<int> evt)
    {
        Debug.Log("Yaw I changed to: " + evt.newValue);
    }

    private void OnRollDChanged(ChangeEvent<int> evt)
    {
        Debug.Log("Roll D changed to: " + evt.newValue);
    }
    
    private void OnPitchDChanged(ChangeEvent<int> evt)
    {
        Debug.Log("Pitch D changed to: " + evt.newValue);
    }
    
    private void OnYawDChanged(ChangeEvent<int> evt)
    {
        Debug.Log("Yaw D changed to: " + evt.newValue);
    }
}
    