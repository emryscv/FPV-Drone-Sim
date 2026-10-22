using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
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
    private FloatField motorTorqueConstant;
    private SliderInt cameraAngle;
    private Label cameraAngleLabel;

    private Label thrustToWeight;
    private Label hoverThrottle;
    private Label totalThrust;
    private Label maxSpeed;

    private VisualElement droneLibrary;
    private VisualElement addBtn;

    private VisualElement activeDroneCard;
    private int droneCount;

    void Awake()
    {
        _uiManager = GetComponent<UIDocument>();
        _DroneMenuTab = _uiManager.rootVisualElement.Q<VisualElement>("DroneMenuTab");

        droneName = _DroneMenuTab.Q<TextField>("DroneName");
        totalWeight = _DroneMenuTab.Q<FloatField>("TotalWeight");
        frameSize = _DroneMenuTab.Q<IntegerField>("FrameSize");
        batteryCapacity = _DroneMenuTab.Q<IntegerField>("BatteryCapacity");
        motorThrust = _DroneMenuTab.Q<FloatField>("MotorThrust");
        motorTorqueConstant = _DroneMenuTab.Q<FloatField>("MotorTorqueConstant");
        cameraAngle = _DroneMenuTab.Q<SliderInt>("CameraAngle");
        cameraAngleLabel = _DroneMenuTab.Q<Label>("CameraAngleLabel");

        thrustToWeight = _DroneMenuTab.Q<VisualElement>("TWR").Q<Label>("Value");
        hoverThrottle = _DroneMenuTab.Q<VisualElement>("HoverThrottle").Q<Label>("Value");
        totalThrust = _DroneMenuTab.Q<VisualElement>("TotalThrust").Q<Label>("Value");
        maxSpeed = _DroneMenuTab.Q<VisualElement>("MaxSpeed").Q<Label>("Value");

        droneLibrary = _DroneMenuTab.Q<VisualElement>("DroneLibrary").Q<VisualElement>("Content");
        addBtn = _DroneMenuTab.Q<VisualElement>("AddButton");

        droneName.RegisterValueChangedCallback(OnDroneNameChanged);
        totalWeight.RegisterValueChangedCallback(OnTotalWeightChanged);
        frameSize.RegisterValueChangedCallback(OnFrameSizeChanged);
        batteryCapacity.RegisterValueChangedCallback(OnBatteryCapacityChanged);
        motorThrust.RegisterValueChangedCallback(OnMotorThrustChanged);
        motorTorqueConstant.RegisterValueChangedCallback(OnMotorTorqueConstantChanged);
        cameraAngle.RegisterValueChangedCallback(OnCameraAngleChanged);

        Debug.Log("Drone menu initialized with drone: " + droneLibrary);
        addBtn.RegisterCallback<ClickEvent>(OnAddNewDrone);

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
        UpdatePerformanceEstimates();
    }

    public void OnFrameSizeChanged(ChangeEvent<string> evt)
    {
        _dronePhysics.DroneProperties.FrameSize = int.Parse(evt.newValue);
        UpdatePerformanceEstimates();
    }

    public void OnBatteryCapacityChanged(ChangeEvent<string> evt)
    {
        _dronePhysics.DroneProperties.BatteryCapacity = int.Parse(evt.newValue);
        UpdatePerformanceEstimates();
    }

    public void OnMotorThrustChanged(ChangeEvent<string> evt)
    {
        _dronePhysics.DroneProperties.MaxThrust = float.Parse(evt.newValue);
        UpdatePerformanceEstimates();
    }

    public void OnMotorTorqueConstantChanged(ChangeEvent<string> evt)
    {
        _dronePhysics.DroneProperties.MotorTorqueConstant = float.Parse(evt.newValue);
        UpdatePerformanceEstimates();
    }

    public void OnCameraAngleChanged(ChangeEvent<int> evt)
    {
        _dronePhysics.DroneProperties.CameraAngle = evt.newValue;
        cameraAngleLabel.text = evt.newValue.ToString() + "°";
    }

    public void OnSave()
    {
        //TODO this is more complex
        Debug.Log("Saving drone: " + _dronePhysics.DroneProperties.Name);
        activeDroneCard.Q<Label>("Name").text = _dronePhysics.DroneProperties.Name;
        activeDroneCard.Q<Label>("Size").text = _dronePhysics.DroneProperties.FrameSize + "mm - " + _dronePhysics.DroneProperties.PropellerDiameter + "\"";
        activeDroneCard.Q<Label>("ThrustToWeight").text = (_dronePhysics.DroneProperties.MaxThrust / _dronePhysics.DroneProperties.Mass) + ":1";
        
        DatabaseManager.Instance.UpdateDrone(_dronePhysics.DroneProperties);
    }

    private void OnAddNewDrone(ClickEvent evt)
    {
        Debug.Log("Adding new drone: " + _dronePhysics.DroneProperties.Name);
        DatabaseManager.Instance.InsertDrone(_dronePhysics.DroneProperties);
        AddDroneCard(_dronePhysics.DroneProperties);
        droneCount++;
    }

    public void PopulateDroneLibrary()
    {
        List<Drone> drones = DatabaseManager.Instance.GetAllDrones();
        droneCount = drones.Count;
        foreach (Drone drone in drones)
        {
            Debug.Log("Populating drone library with: " + drone.Id + " - " + drone.Name);
            AddDroneCard(drone);            
        }
    }

    public void SetDrone(Drone drone)
    {
        //This is to account for both initializing from the flightController and from a UI selection
        if (drone != _dronePhysics.DroneProperties)
            _dronePhysics.DroneProperties = drone;

        droneName.value = _dronePhysics.DroneProperties.Name;
        totalWeight.Set(_dronePhysics.DroneProperties.Mass);
        frameSize.Set((int)_dronePhysics.DroneProperties.FrameSize);
        batteryCapacity.Set((int)_dronePhysics.DroneProperties.BatteryCapacity);
        motorThrust.Set(_dronePhysics.DroneProperties.MaxThrust);
        cameraAngle.value = _dronePhysics.DroneProperties.CameraAngle;
        UpdatePerformanceEstimates();

        cameraAngleLabel.text = _dronePhysics.DroneProperties.CameraAngle.ToString() + "°";
    }

    public void AddDroneCard(Drone drone)
    {
        Debug.Log("Adding drone card for: " + drone.Id + " - " + drone.Name);
        VisualElement droneCard = _droneCardUXML.Instantiate();
        droneCard.Q<Label>("Name").text = drone.Name;
        droneCard.Q<Label>("Size").text = drone.FrameSize + "mm - " + drone.PropellerDiameter + "\"";
        droneCard.Q<Label>("ThrustToWeight").text = (drone.MaxThrust / drone.Mass) + ":1";

        //As a proof of concept this callbacks are not being unregistered when a drone is deleted
        droneCard.Q<Image>("DeleteIcon").RegisterCallback<ClickEvent>(evt =>
        {
            if (droneCount > 1)
            {
                droneCount--;
                if (activeDroneCard == droneCard)
                    activeDroneCard = null;

                DatabaseManager.Instance.DeleteDrone(drone.Id); ;
                droneLibrary.Remove(droneCard);
            }
        });

        droneCard.RegisterCallback<ClickEvent>(evt =>
        {
            SetDrone(drone);
            SetActiveDroneCard(droneCard);
        });

        droneLibrary.Add(droneCard);
        
        if(drone.Id == _dronePhysics.DroneProperties.Id)
            SetActiveDroneCard(droneCard);
    }

    public void SetActiveDroneCard(VisualElement droneCard)
    {
        if (activeDroneCard != null)
            activeDroneCard.RemoveFromClassList("activeDroneCard");
        droneCard.AddToClassList("activeDroneCard");
        activeDroneCard = droneCard;
    }

    private void UpdatePerformanceEstimates()
    {
        if (_dronePhysics.DroneProperties == null)
            return;

        float maxThrust = _dronePhysics.DroneProperties.MaxThrust * 4;

        thrustToWeight.text = ((int)(maxThrust / _dronePhysics.DroneProperties.Mass)).ToString() + ":1";
        hoverThrottle.text = (_dronePhysics.DroneProperties.Mass * 100 / maxThrust).ToString("F0");
        totalThrust.text = maxThrust.ToString("F2");
        //TODO Compute max speed based on drone properties
        maxSpeed.text = "N/A"; 
    }
}
