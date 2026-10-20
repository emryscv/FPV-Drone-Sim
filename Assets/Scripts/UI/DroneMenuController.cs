using UnityEngine;
using UnityEngine.UIElements;

public class DroneMenuController : MonoBehaviour, ISettingsMenuController
{
    [SerializeField] private DronePhysics _dronePhysics;

    private UIDocument _uiManager;
    private VisualElement _DroneMenuTab;

    //TODO Figure propeller size selector later
    private TextField droneName;
    private FloatField totalWeight;
    private IntegerField frameSize;
    private IntegerField batteryCapacity;
    private FloatField motorThrust;
    private SliderInt cameraAngle;

    private Label cameraAngleLabel;

    void Awake()
    {
        _uiManager = GetComponent<UIDocument>();
        _DroneMenuTab = _uiManager.rootVisualElement.Q<VisualElement>("DroneMenuTab");

        droneName = _DroneMenuTab.Q<TextField>("DroneName");
        totalWeight = _DroneMenuTab.Q<FloatField>("TotalWeight");
        frameSize = _DroneMenuTab.Q<IntegerField>("FrameSize");
        batteryCapacity = _DroneMenuTab.Q<IntegerField>("BatteryCapacity");
        motorThrust = _DroneMenuTab.Q<FloatField>("MotorThrust");
        cameraAngle = _DroneMenuTab.Q<SliderInt>("CameraAngle");
        cameraAngleLabel = _DroneMenuTab.Q<Label>("CameraAngleLabel");

        droneName.RegisterValueChangedCallback(OnDroneNameChanged);
        totalWeight.RegisterValueChangedCallback(OnTotalWeightChanged);
        frameSize.RegisterValueChangedCallback(OnFrameSizeChanged);
        batteryCapacity.RegisterValueChangedCallback(OnBatteryCapacityChanged);
        motorThrust.RegisterValueChangedCallback(OnMotorThrustChanged);
        cameraAngle.RegisterValueChangedCallback(OnCameraAngleChanged);

        droneName.value = _dronePhysics.DroneProperties.Name;
        totalWeight.Set(_dronePhysics.DroneProperties.Mass);
        frameSize.Set((int)_dronePhysics.DroneProperties.FrameSize);
        batteryCapacity.Set((int)_dronePhysics.DroneProperties.BatteryCapacity);
        motorThrust.Set(_dronePhysics.DroneProperties.MaxThrust);
        cameraAngle.value = _dronePhysics.DroneProperties.CameraAngle;

        cameraAngleLabel.text = _dronePhysics.DroneProperties.CameraAngle.ToString() + "°";
    }

    public void OnDroneNameChanged(ChangeEvent<string> evt)
    {
        _dronePhysics.DroneProperties.Name = evt.newValue;
    }
    
    public void OnTotalWeightChanged(ChangeEvent<string> evt)
    {
        _dronePhysics.DroneProperties.Mass = float.Parse(evt.newValue);
    }

    public void OnFrameSizeChanged(ChangeEvent<string> evt)
    {
        _dronePhysics.DroneProperties.FrameSize = int.Parse(evt.newValue);
    }

    public void OnBatteryCapacityChanged(ChangeEvent<string> evt)
    {
        _dronePhysics.DroneProperties.BatteryCapacity = int.Parse(evt.newValue);
    }

    public void OnMotorThrustChanged(ChangeEvent<string> evt)
    {
        _dronePhysics.DroneProperties.MaxThrust = float.Parse(evt.newValue);
    }

    public void OnCameraAngleChanged(ChangeEvent<int> evt)
    {
        _dronePhysics.DroneProperties.CameraAngle = evt.newValue;
        cameraAngleLabel.text = evt.newValue.ToString() + "°";
    }

    public void OnSave()
    {
        //TODO this is more complex
        DatabaseManager.Instance.InsertDrone(_dronePhysics.DroneProperties);
    }
}
