using UnityEngine;
using System;

public class DronePhysics : MonoBehaviour
{
    float mass;
    float k; //Rotor's torque constant
    float l; //Arm Length
    float sqrt2;
    private FlightController flightController; // Reference to the FlightController script
    private Rigidbody rb; // Reference to the Rigidbody component

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        flightController = GetComponent<FlightController>(); // Get the FlightController component attached to the same GameObject
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component attached to the same GameObject
    }

    void Start()
    {
        mass = 0.017f; //kg
        l = 0.033f; //m
        k = 0.01f; //Nm TODO figure out
        g = 9.81f; //m/s2
        sqrt2 = Mathf.Sqrt(2);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //
        float c = (flightController.motorMix[0] + flightController.motorMix[1] + flightController.motorMix[2] + flightController.motorMix[3]) / mass; // Collective thrust
     
        Vector3 torque = new Vector3(
            l / sqrt2 * (flightController.motorMix[0] + flightController.motorMix[1] - flightController.motorMix[2] - flightController.motorMix[3]),
            k * (- flightController.motorMix[0] + flightController.motorMix[1] - flightController.motorMix[2] + flightController.motorMix[3]),
            l / sqrt2 * (flightController.motorMix[0] - flightController.motorMix[1] - flightController.motorMix[2] + flightController.motorMix[3]) 
        );

        rb.AddRelativeTorque(torque);
        rb.AddForce(transform.up * c);
    }
}