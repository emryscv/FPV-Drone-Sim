using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

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

        float c = (motorThrust[0] + motorThrust[1] + motorThrust[2] + motorThrust[3]); // Collective thrust
        
        Debug.Log("Collective thrust: " + c + " M1: " + motorThrust[0] + " M2: " + motorThrust[1] + " M3: " + motorThrust[2] + " M4: " + motorThrust[3] + " Max Thrust: " + maxThrust + " mass: " + mass);

        Vector3 torque = new Vector3(
            l / sqrt2 * (motorThrust[0] - motorThrust[1] + motorThrust[2] - motorThrust[3]),
            k * (-motorThrust[0] + motorThrust[1] + motorThrust[2] - motorThrust[3]),
            l / sqrt2 * (-motorThrust[0] - motorThrust[1] + motorThrust[2] + motorThrust[3])
        );

        //Debug.Log("Torque: " + torque);

        rb.AddRelativeTorque(torque);
        rb.AddForce(transform.up * c, ForceMode.Force);

        //Debug.Log("Angle: " + Vector3.Angle(transform.up, Vector3.up) + " Linear Velocity: " + rb.linearVelocity.magnitude);
        if(rb.linearVelocity.magnitude == 0 && Vector3.Angle(transform.up, Vector3.up) > 90)
           GameEvents.Instance.Crash();
    }

    void OnCollisionEnter(Collision collision)
    {
        //TODO Improve Crash condition to better detect crashes
        bool isCrash = collision.gameObject.layer != groundLayer;

        isCrash = isCrash && collision.relativeVelocity.magnitude > 8f;   

        ContactPoint contact = collision.GetContact(0);
        Vector3 localContactPoint = transform.InverseTransformPoint(contact.point);

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