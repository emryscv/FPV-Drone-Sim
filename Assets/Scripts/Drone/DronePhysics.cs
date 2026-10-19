using UnityEngine;

//TODO Implement Air Drag, RPM, Thrust Curve, Ground Effect, Prop Wash, Turbulence, Wind.

public class DronePhysics : MonoBehaviour
{
    [SerializeField] private int droneId = 1;
    int groundLayer;
    //TODO find out if {get; set;} do what  i expect
    public Drone DroneProperties {get; set;}
    private FlightController flightController; // Reference to the FlightController script
    private Rigidbody droneRB; // Reference to the Rigidbody component

    private Transform FLMotor; // Reference to the Transform component
    private Transform FRMotor; // Reference to the Transform component
    private Transform RLMotor; // Reference to the Transform component
    private Transform RRMotor; // Reference to the Transform component

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        groundLayer = LayerMask.NameToLayer("Ground");
        flightController = GetComponent<FlightController>(); // Get the FlightController component attached to the same GameObject
        droneRB = GetComponent<Rigidbody>(); // Get the Rigidbody component attached to the same GameObject
        DroneProperties = DatabaseManager.Instance.GetDrone(droneId);

        droneRB.mass = DroneProperties.Mass; //kg

        //TODO see how can i mantain aspect ratio
        Vector3 scale = droneRB.gameObject.transform.localScale;
        scale.x = DroneProperties.FrameSize  * 2f; //m
        scale.y = DroneProperties.FrameSize  * 2f; //m
        
        droneRB.gameObject.transform.localScale = scale;

        //maxThrust = Mathf.Abs(Physics.gravity.y * mass * 7f) / 4f; //This is thrust per motor

        FLMotor = GameObject.Find("FLMotor").transform;
        FRMotor = GameObject.Find("FRMotor").transform;
        RLMotor = GameObject.Find("RLMotor").transform;
        RRMotor = GameObject.Find("RRMotor").transform;
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
            DroneProperties.FrameSize * (motorThrust[0] - motorThrust[1] + motorThrust[2] - motorThrust[3]),
            DroneProperties.MotorTorqueConstant * (-motorThrust[0] + motorThrust[1] + motorThrust[2] - motorThrust[3]),
            DroneProperties.FrameSize * (-motorThrust[0] - motorThrust[1] + motorThrust[2] + motorThrust[3])
        );

        //Debug.Log("Torque: " + torque);
        //rb.AddForceAtPosition(transform.up * motorThrust[0], RRMotor.position, ForceMode.Force);
        //rb.AddForceAtPosition(transform.up * motorThrust[1], FRMotor.position, ForceMode.Force);
        //rb.AddForceAtPosition(transform.up * motorThrust[2], FLMotor.position, ForceMode.Force);
        //rb.AddForceAtPosition(transform.up * motorThrust[3], RLMotor.position, ForceMode.Force);
        droneRB.AddRelativeTorque(torque);
        droneRB.AddForce(transform.up * c, ForceMode.Force);

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
}