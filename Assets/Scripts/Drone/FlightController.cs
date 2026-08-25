using UnityEngine;
using UnityEngine.InputSystem;

public class FlightController : MonoBehaviour
{
   // private DronePhysics dronePhysics;
    private Rigidbody drone; // Reference to the Rigidbody component
    private Controls controls;

    // I think this is going to be a raw measurement of the input, and then we will apply the rate transformation
    float throttle;
    float yaw;
    float pitch;
    float roll;

    [SerializeField] float rotateSpeed;
    [SerializeField] float flySpeed;

    //PID constants
    float[] kP;
    float[] kI;
    float[] kD;


    //PID cumulative terms
    float[] cumulativeI;
    float[] prevError;
    float[] prevTime;


    //Rates
    float[] rcExpo;
    float[] rcRates;
    float[] rates;

    //OutputRotor Thrusts
    public float[] motorMix;
    float[][] motorMixMatrix;

    void Awake()
    {
        controls = new Controls();
        controls.RCController.Enable();
        //dronePhysics = GetComponent<DronePhysics>();
        drone = GetComponent<Rigidbody>(); // Get the Rigidbody component attached to the same GameObject
    }

    void Start()
    {
        prevTime = new float[3] { Time.time, Time.time, Time.time };
        prevError = new float[3] { 0.0f, 0.0f, 0.0f };
        cumulativeI = new float[3] { 0.0f, 0.0f, 0.0f };

        //TODO we need to tune these constants
        kP = new float[3] { 200.0f, 500.0f, 200.0f };
        kI = new float[3] { 0.0f, 45.0f, 0.0f };
        kD = new float[3] { 0.0f, 0.0f, 0.0f };

        // kP = new float[3] { 0.6f, 0.1f, 0.1f };
        // kI = new float[3] { 0f, 0f, 0f };
        // kD = new float[3] { 0f, 0f, 0f };

        rcExpo = new float[3] { 0.1f, 0.1f, 0.1f };
        rcRates = new float[3] { 1.0f, 1.0f, 1.0f };
        rates = new float[3] { 0.7f, 0.7f, 0.7f };

        motorMix = new float[4] { 0f, 0f, 0f, 0f };
        motorMixMatrix = new float[4][] {
            new float[3] {  1, -1, -1 },
            new float[3] { -1,  1, -1 },
            new float[3] {  1,  1,  1 },
            new float[3] { -1, -1,  1 }
        };
    }

    // Update is called once per frame
    void Update()
    {
        throttle = controls.RCController.Throttle.ReadValue<float>();
        yaw = controls.RCController.Yaw.ReadValue<float>();
        pitch = controls.RCController.Pitch.ReadValue<float>();
        roll = controls.RCController.Roll.ReadValue<float>();
     
        float throttleSetpoint = (throttle + 1) / 2.0f; // Fix the axis numbers with the axis order
        float pitchSetpoint = ComputeBetaflightRates(0, pitch);
        float yawSetpoint = ComputeBetaflightRates(1, yaw);
        float rollSetpoint = ComputeBetaflightRates(2, roll);


        float pitchPID = PIDEquation(pitchSetpoint, drone.angularVelocity.x, 0) / 1000.0f;
        float yawPID = PIDEquation(yawSetpoint, drone.angularVelocity.y, 1) / 1000.0f;
        float rollPID = PIDEquation(rollSetpoint, drone.angularVelocity.z, 2) / 1000.0f;


        float motorMin = float.MaxValue;
        float motorMax = float.MinValue;

        for (int i = 0; i < 4; i++)
        {
            motorMix[i] = motorMixMatrix[i][0] * pitchPID + motorMixMatrix[i][1] * yawPID + motorMixMatrix[i][2] * rollPID;
            //motorMix[i] = motorMixMatrix[i][1] * yawPID;

            motorMin = System.Math.Min(motorMin, motorMix[i]);
            motorMax = System.Math.Max(motorMax, motorMix[i]);
        }

        //Debug.Log("Thrusts: F1 " + motorMix[0] + " F2 " + motorMix[1] + " F3 " + motorMix[2] + " F4 " + motorMix[3] + " Min: " + motorMin + " Max: " + motorMax);

        float motorRange = motorMax - motorMin;

        //Debug.Log("Thrusts: F1 " + motorMix[0] + " F2 " + motorMix[1] + " F3 " + motorMix[2] + " F4 " + motorMix[3] + " Min: " + motorMin + " Max: " + motorMax);
        //Debug.Log("Min: " + motorMin + " Max: " + motorMax);
        
        float normalizationFactor = motorRange > 1.0f ? 1.0f / motorRange : 1.0f;
        throttleSetpoint = Mathf.Clamp(throttleSetpoint, -motorMin * normalizationFactor, 1.0f - motorMax * normalizationFactor); 
        
        for (int i = 0; i < 4; i++)
        {
            motorMix[i] = (throttleSetpoint + motorMix[i] * normalizationFactor) * 0.08338501f; //this value is the 
        }

        //Debug.Log("Normalized Thrusts: F1 " + (motorMix[0]) + " F2 " + (motorMix[1]) + " F3 " + (motorMix[2]) + " F4 " + (motorMix[3]) + " Throttle Setpoint: " + throttleSetpoint + " Normalization Factor: " + normalizationFactor);
    }

    float ComputeBetaflightRates(int axis, float input)
    {
        float inputAbs = Mathf.Abs(input);

        input = input * inputAbs * inputAbs * inputAbs * rcExpo[axis] + input * (1 - rcExpo[axis]);

        float angleRate = 200.0f * rcRates[axis] * input;

        float rcSuperfactor = 1.0f - (inputAbs * rates[axis]);
        rcSuperfactor = Mathf.Clamp(rcSuperfactor, 0.01f, 1.00f);
        rcSuperfactor = 1.0f / rcSuperfactor;

        angleRate *= rcSuperfactor;

        return angleRate;
    }

    float PIDEquation(float setpoint, float measurements, int axis)
    {
        float deltaTime = Time.time - prevTime[axis];
        prevTime[axis] = Time.time;

        float error = setpoint * Mathf.PI / 180f - measurements; //TODO fix this
        //Debug.Log("Axis: " + axis + " measurement: " + measurements + " setpoint: " + (setpoint * Mathf.PI / 180f) + " error: " + error);
        //Proportional term
        float P = kP[axis] * error;

        //Integral term
        float I = cumulativeI[axis] + kI[axis] * error * deltaTime;
        I = System.Math.Clamp(I, -400, 400); //Betaflight values
        cumulativeI[axis] = I;

        //Derivative term
        float D = deltaTime == 0 ? 0 : kD[axis] * (error - prevError[axis]) / deltaTime;

        prevError[axis] = error;
        
        float PID = P + I + D;
        PID = System.Math.Clamp(PID, -500, 500); //Betaflight values
        Debug.Log("Axis: " + axis + " measurement: " + measurements + " setpoint: " + (setpoint * Mathf.PI / 180f) + " P: " + P + " I: " + I + " D: " + D + " PID: " + PID);

        return PID;
    }
}