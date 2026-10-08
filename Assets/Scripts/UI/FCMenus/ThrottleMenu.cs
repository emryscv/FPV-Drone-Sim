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

        _ThrottleMid.RegisterCallback<ChangeEvent<float>>(OnThrottleMidChanged);
        _ThrottleExpo.RegisterCallback<ChangeEvent<float>>(OnThrottleExpoChanged);
    }

    private void OnDisable()
    {
        _ThrottleMid.UnregisterCallback<ChangeEvent<float>>(OnThrottleMidChanged);
        _ThrottleExpo.UnregisterCallback<ChangeEvent<float>>(OnThrottleExpoChanged);
    }

    private void OnThrottleMidChanged(ChangeEvent<float> evt){}
    
    private void OnThrottleExpoChanged(ChangeEvent<float> evt){}

}
    