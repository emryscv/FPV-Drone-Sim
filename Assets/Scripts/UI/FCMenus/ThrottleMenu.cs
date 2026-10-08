using UnityEngine;
using UnityEngine.UIElements;

public class ThrottleMenu : MonoBehaviour
{
    [SerializeField] private FlightController _flightController;

    private UIDocument _uiManager;
    private VisualElement _FCMenuTab;

    //Throttle
    private FloatField _ThrottleMid;
    private FloatField _ThrottleExpo;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        _uiManager = GetComponent<UIDocument>();
        _FCMenuTab = _uiManager.rootVisualElement.Q<VisualElement>("FCMenuTab");

        //Throttle
        _ThrottleMid = _FCMenuTab.Q<FloatField>("ThrottleMid");
        _ThrottleExpo = _FCMenuTab.Q<FloatField>("ThrottleExpo");
        
        _ThrottleMid.RegisterCallback<ChangeEvent<string>>(OnThrottleMidChanged);
        _ThrottleExpo.RegisterCallback<ChangeEvent<string>>(OnThrottleExpoChanged);
    }

    private void OnDisable()
    {
        _ThrottleMid.UnregisterCallback<ChangeEvent<string>>(OnThrottleMidChanged);
        _ThrottleExpo.UnregisterCallback<ChangeEvent<string>>(OnThrottleExpoChanged);
    }

    private void OnThrottleMidChanged(ChangeEvent<string> evt)
    {
        Debug.Log("Throttle Mid changed to: " + evt.newValue);
    }
    
    private void OnThrottleExpoChanged(ChangeEvent<string> evt)
    {
        Debug.Log("Throttle Expo changed to: " + evt.newValue);
    }

}
    