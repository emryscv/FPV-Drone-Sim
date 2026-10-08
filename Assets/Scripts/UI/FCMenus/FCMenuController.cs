using UnityEngine;
using UnityEngine.UIElements;

public class FCMenuController : MonoBehaviour
{
    [SerializeField] private FlightController _flightController;

    private UIDocument _uiManager;
    private VisualElement _FCMenuTab;

    private EnumField _RateType;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        _uiManager = GetComponent<UIDocument>();
        _FCMenuTab = _uiManager.rootVisualElement.Q<VisualElement>("FCMenuTab");

        _RateType = _FCMenuTab.Q<EnumField>("RateTypeSelector");
        _RateType.Init(RateType.Betaflight);
        _RateType.RegisterCallback<ChangeEvent<EnumField>>(OnRateTypeChanged);
        //TODO fix not working
    }

    private void OnDisable()
    {
        _RateType.UnregisterCallback<ChangeEvent<EnumField>>(OnRateTypeChanged);
    }

    private void OnRateTypeChanged(ChangeEvent<EnumField> evt)
    {
        Debug.Log("Rate Type changed to: " + evt.newValue);
        _flightController.SetRateType((RateType)_RateType.value);
    }
}
    