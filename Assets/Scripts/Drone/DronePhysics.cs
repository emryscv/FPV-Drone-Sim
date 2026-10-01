using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

//TODO Implement Air Drag, RPM, Thrust Curve, Ground Effect, Prop Wash, Turbulence, Wind.

public class DronePhysics : MonoBehaviour
{
    int groundLayer;

    float mass;
    float k; //Rotor's torque constant
    float l; //Arm Length
    float sqrt2;
    float maxThrust;

    private FlightController flightController; // Reference to the FlightController script
    private Rigidbody rb; // Reference to the Rigidbody component

    private Transform FLMotor; // Reference to the Transform component
    private Transform FRMotor; // Reference to the Transform component
    private Transform RLMotor; // Reference to the Transform component
    private Transform RRMotor; // Reference to the Transform component

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        groundLayer = LayerMask.NameToLayer("Ground");
        flightController = GetComponent<FlightController>(); // Get the FlightController component attached to the same GameObject
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component attached to the same GameObject
    }

    void Start()
    {
        mass = rb.mass; //kg
        l = 0.033f; //m
        k = 0.01f; //Nm TODO figure out

        maxThrust = Mathf.Abs(Physics.gravity.y * mass * 2f) / 4f; //This is thrust per motor

        sqrt2 = Mathf.Sqrt(2);

        FLMotor = GameObject.Find("FLMotor").transform;
        FRMotor = GameObject.Find("FRMotor").transform;
        RLMotor = GameObject.Find("RLMotor").transform;
        RRMotor = GameObject.Find("RRMotor").transform;
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        float[] motorThrust = {
            flightController.motorMix[0] * maxThrust,
            flightController.motorMix[1] * maxThrust,
            flightController.motorMix[2] * maxThrust,
            flightController.motorMix[3] * maxThrust
        };

        float c = motorThrust[0] + motorThrust[1] + motorThrust[2] + motorThrust[3]; // Collective thrust

        //Debug.Log("M1: " + flightController.motorMix[0] + " M2: " + flightController.motorMix[1] + " M3: " + flightController.motorMix[2] + " M4: " + flightController.motorMix[3] + " Max Thrust: " + maxThrust + " mass: " + mass);
        //Debug.Log("M1: " + motorThrust[0] + " C: " + c / 4f + " M2: " + motorThrust[1] + " M3: " + motorThrust[2] + " M4: " + motorThrust[3]);
        Vector3 torque = new Vector3(
            l / sqrt2 * (motorThrust[0] - motorThrust[1] + motorThrust[2] - motorThrust[3]),
            k * (-motorThrust[0] + motorThrust[1] + motorThrust[2] - motorThrust[3]),
            l / sqrt2 * (-motorThrust[0] - motorThrust[1] + motorThrust[2] + motorThrust[3])
        );

        //Debug.Log("Torque: " + torque);
        //rb.AddForceAtPosition(transform.up * motorThrust[0], RRMotor.position, ForceMode.Force);
        //rb.AddForceAtPosition(transform.up * motorThrust[1], FRMotor.position, ForceMode.Force);
        //rb.AddForceAtPosition(transform.up * motorThrust[2], FLMotor.position, ForceMode.Force);
        //rb.AddForceAtPosition(transform.up * motorThrust[3], RLMotor.position, ForceMode.Force);
        rb.AddRelativeTorque(torque);
        rb.AddForce(transform.up * c, ForceMode.Force);

        // Consider it a crash if the drone is upside down and almost stationary
        if (rb.linearVelocity.magnitude < 0.1f && Vector3.Angle(transform.up, Vector3.up) >= 90)
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
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public void ResetDroneState(Vector3? position = null, Quaternion? rotation = null)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.position = position == null ? new Vector3(45, 0.063f, 0) : position.Value;
        rb.rotation = rotation == null ? Quaternion.Euler(0, 270, 0) : rotation.Value;
    }


}