using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DroneMenuController : MonoBehaviour, ISettingsMenuController
{
    [SerializeField] private DronePhysics _dronePhysics;
    [SerializeField] private VisualTreeAsset _droneCardUXML;

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

    private VisualElement droneLibrary;

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

        droneLibrary = _DroneMenuTab.Q<VisualElement>("DroneLibrary").Q<VisualElement>("Content");

        droneName.RegisterValueChangedCallback(OnDroneNameChanged);
        totalWeight.RegisterValueChangedCallback(OnTotalWeightChanged);
        frameSize.RegisterValueChangedCallback(OnFrameSizeChanged);
        batteryCapacity.RegisterValueChangedCallback(OnBatteryCapacityChanged);
        motorThrust.RegisterValueChangedCallback(OnMotorThrustChanged);
        cameraAngle.RegisterValueChangedCallback(OnCameraAngleChanged);

        SetDrone(_dronePhysics.DroneProperties);
        PopulateDroneLibrary();
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

    public void PopulateDroneLibrary()
    {
        List<Drone> drones = DatabaseManager.Instance.GetAllDrones();
        foreach (var drone in drones)
        {
            VisualElement droneCard = _droneCardUXML.Instantiate();
            droneCard.Q<Label>("Name").text = drone.Name;
            droneCard.Q<Label>("Size").text = drone.FrameSize + "mm - " + drone.PropellerDiameter + "\"";
            droneCard.Q<Label>("ThrustToWeight").text = (drone.MaxThrust / drone.Mass) + ":1";

            //As a proof of concept this callbacks are not being unregistered when a drone is deleted
            droneCard.Q<Image>("DeleteIcon").RegisterCallback<ClickEvent>(evt =>
            {
                DatabaseManager.Instance.DeleteDrone(drone.Id);;
                droneLibrary.Remove(droneCard);
            });

            droneCard.RegisterCallback<ClickEvent>(evt =>
            {
                SetDrone(drone);
            });

            droneLibrary.Add(droneCard);
        }
    }

    public void SetDrone(Drone drone)
    {
        //This is to account for both initializing from the flightController and from a UI selection
        if(drone != _dronePhysics.DroneProperties)
            _dronePhysics.DroneProperties = drone;
    
        droneName.value = _dronePhysics.DroneProperties.Name;
        totalWeight.Set(_dronePhysics.DroneProperties.Mass);
        frameSize.Set((int)_dronePhysics.DroneProperties.FrameSize);
        batteryCapacity.Set((int)_dronePhysics.DroneProperties.BatteryCapacity);
        motorThrust.Set(_dronePhysics.DroneProperties.MaxThrust);
        cameraAngle.value = _dronePhysics.DroneProperties.CameraAngle;

        cameraAngleLabel.text = _dronePhysics.DroneProperties.CameraAngle.ToString() + "°";
    }
}
