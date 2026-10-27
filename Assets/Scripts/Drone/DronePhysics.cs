using UnityEngine;

//TODO Implement Air Drag, RPM, Thrust Curve, Ground Effect, Prop Wash, Turbulence, Wind.

public class DronePhysics : MonoBehaviour
{
    [SerializeField] private int droneId = 1;

    private float[] propellerSizes = { //This array values are m conversion of the units in the UI
        0.035f,  //35mm
        0.04f,   //40mm
        0.045f,  //45mm
        0.0508f, //2"
        0.0638f, //2.5"
        0.0762f, //3"
        0.1016f, //4"
        0.127f,  //5"
        0.12954f, //5.1"
        0.1524f, //6"
        0.1778f, //7"
        0.2032f, //8"
        0.254f   //10"
    };

    //Temporal Vairables to test gorund effect
    public bool groundEffectEmpricalIsActive;
    public bool groundEffectLikeHelicoptersIsActive;
    public bool groundEffectSanchezCuevasIsActive;


    int groundLayer;
    //TODO find out if {get; set;} do what  i expect
    public Drone DroneProperties { get; set; }

    private FlightController flightController; // Reference to the FlightController script
    private Rigidbody droneRB; // Reference to the Rigidbody component

    private Transform FLMotor; // Reference to the Transform component
    private Transform FRMotor; // Reference to the Transform component
    private Transform RLMotor; // Reference to the Transform component
    private Transform RRMotor; // Reference to the Transform component

    private float l; //Half the distance between two consecutive motors. Assuming the frame is square
    private float propellerSize;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        DroneProperties = DatabaseManager.Instance.GetAllDrones()[0];

        groundLayer = LayerMask.NameToLayer("Ground");
        flightController = GetComponent<FlightController>(); // Get the FlightController component attached to the same GameObject
        droneRB = GetComponent<Rigidbody>(); // Get the Rigidbody component attached to the same GameObject

        FLMotor = GameObject.Find("FLMotor").transform;
        FRMotor = GameObject.Find("FRMotor").transform;
        RLMotor = GameObject.Find("RLMotor").transform;
        RRMotor = GameObject.Find("RRMotor").transform;

        groundEffectEmpricalIsActive = false;
        groundEffectLikeHelicoptersIsActive = true;

    }

    void OnEnable()
    {
        GetComponentInChildren<Camera>().transform.localRotation = Quaternion.Euler(-1 * DroneProperties.CameraAngle, 0, 0);

        //TODO Check if the drone frame is on meters
        droneRB.mass = DroneProperties.Mass; //kg
        l = DroneProperties.FrameSize / 200.0f; //Half the distance between two consecutive motors assuming the frame is square   
        propellerSize = propellerSizes[DroneProperties.PropellerDiameter]; //m

        //TODO see how can i mantain aspect ratio
        Vector3 scale = droneRB.gameObject.transform.localScale;
        scale.x = l * 2f; //m
        scale.y = l * 2f; //m
        scale.z = l * 2f; //m

        droneRB.gameObject.transform.localScale = scale;

        Debug.Log("Drone Properties: Mass: " + DroneProperties.Mass + "kg, Frame Size: " + DroneProperties.FrameSize + "mm, Propeller Size: " + propellerSize + "m, Battery Capacity: " + DroneProperties.BatteryCapacity + "mAh, Max Thrust: " + DroneProperties.MaxThrust + "N, Camera Angle: " + DroneProperties.CameraAngle + "°, Motor Torque Constant: " + DroneProperties.MotorTorqueConstant);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float[] motorThrust = {
            flightController.MotorMix[0] * DroneProperties.MaxThrust,
            flightController.MotorMix[1] * DroneProperties.MaxThrust,
            flightController.MotorMix[2] * DroneProperties.MaxThrust,
            flightController.MotorMix[3] * DroneProperties.MaxThrust
        };

        float c = motorThrust[0] + motorThrust[1] + motorThrust[2] + motorThrust[3]; // Collective thrust

        //Debug.Log("M1: " + flightController.motorMix[0] + " M2: " + flightController.motorMix[1] + " M3: " + flightController.motorMix[2] + " M4: " + flightController.motorMix[3] + " Max Thrust: " + maxThrust + " mass: " + mass);
        //Debug.Log("M1: " + motorThrust[0] + " C: " + c / 4f + " M2: " + motorThrust[1] + " M3: " + motorThrust[2] + " M4: " + motorThrust[3]);
        Vector3 torque = new Vector3(
            0,//l * (motorThrust[0] - motorThrust[1] + motorThrust[2] - motorThrust[3]),
            DroneProperties.MotorTorqueConstant * (-motorThrust[0] + motorThrust[1] + motorThrust[2] - motorThrust[3]),
            0//l * (-motorThrust[0] - motorThrust[1] + motorThrust[2] + motorThrust[3])
        );

        //Debug.Log("Torque: " + torque);
        droneRB.AddForceAtPosition(transform.up * ApplyGroundEffect(RLMotor.position.y, motorThrust[0]), RLMotor.position, ForceMode.Force);
        droneRB.AddForceAtPosition(transform.up * ApplyGroundEffect(FLMotor.position.y, motorThrust[1]), FLMotor.position, ForceMode.Force);
        droneRB.AddForceAtPosition(transform.up * ApplyGroundEffect(RRMotor.position.y, motorThrust[2]), RRMotor.position, ForceMode.Force);
        droneRB.AddForceAtPosition(transform.up * ApplyGroundEffect(FRMotor.position.y, motorThrust[3]), FRMotor.position, ForceMode.Force);
        
        Debug.Log(ApplyGroundEffect(RLMotor.position.y, motorThrust[0]) + " " + motorThrust[0]);

        droneRB.AddRelativeTorque(torque);
        //droneRB.AddForce(transform.up * c, ForceMode.Force);

        // Consider it a crash if the drone is upside down and almost stationary
        if (droneRB.linearVelocity.magnitude < 0.1f && Vector3.Angle(transform.up, Vector3.up) >= 90)
            GameEvents.Instance.Crash();
    }

    void OnCollisionEnter(Collision collision)
    {
        //Crash with the ground won't count as a crash unless is upside down
        bool isCrash = collision.gameObject.layer != groundLayer;

        // Only consider it a crash if the collision is strong enough about 22 mph
        isCrash = isCrash && collision.relativeVelocity.magnitude > 10f;

        ContactPoint contact = collision.GetContact(0);
        Vector3 localContactPoint = transform.InverseTransformPoint(contact.point);

        //Colissions with the bottom of the drone won't count as a crash
        isCrash = isCrash && (localContactPoint.y > 0f);

        if (isCrash)
            GameEvents.Instance.Crash();
    }

    public void Crash()
    {
        droneRB.linearVelocity = Vector3.zero;
        droneRB.angularVelocity = Vector3.zero;
    }

    public void ResetDroneState(Vector3? position = null, Quaternion? rotation = null)
    {
        droneRB.linearVelocity = Vector3.zero;
        droneRB.angularVelocity = Vector3.zero;
        droneRB.position = position == null ? new Vector3(45, 0.063f, 0) : position.Value;
        droneRB.rotation = rotation == null ? Quaternion.Euler(0, 270, 0) : rotation.Value;
    }

    private float ApplyGroundEffect(float groundDistance, float thrust)
    {
        float Kge = 1f; // Ground effect coefficient, this value can be adjusted based on empirical data or specific drone characteristics
        //Debug.Log(groundEffectEmpricalIsActive);
        if (groundEffectEmpricalIsActive)
        {
            float factor = propellerSize / ( propellerSize + 4 * groundDistance); // Empirical model for ground effect 
            thrust *= 1f + Kge * factor * factor;
        } 

        if (groundEffectLikeHelicoptersIsActive)
        {
            float factor = propellerSize / (4 * groundDistance); // Helicopter-like model for ground effect
            thrust *= 1f / (1f - factor * factor);
        }

        return thrust;
    }
}