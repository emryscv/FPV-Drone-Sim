using UnityEngine;

public class FlightController : MonoBehaviour
{
    // private DronePhysics dronePhysics;
    private Rigidbody drone; // Reference to the Rigidbody component
    private RCControllerManager controls;

    // I think this is going to be a raw measurement of the input, and then we will apply the rate transformation
    float throttle;
    float yaw;
    float pitch;
    float roll;

    //PID constants
    float[] kP;
    float[] kI;
    float[] kD;

    //PID cumulative terms
    float[] cumulativeI;
    float[] prevError;
    float[] prevTime;

    //BetaflightRates
    //For Actual Rates assume that rcRates is center sensitivity 
    //and rates is stick movement sensitivity
    float[] rcRates;
    float[] rates;
    float[] rcExpo;
    float RC_RATE_INCREMENTAL = 14.54f;

    //OutputRotor Thrusts
    public float[] motorMix;
    float[][] motorMixMatrix;

    RateEquation ComputeRate;

    void Awake()
    {
        controls = RCControllerManager.Instance;

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

        rcRates = new float[3] { 1.0f, 1.0f, 1.0f };
        rates = new float[3] { 0.7f, 0.7f, 0.7f };
        rcExpo = new float[3] { 0.1f, 0.1f, 0.1f };

        motorMix = new float[4] { 0f, 0f, 0f, 0f };
        motorMixMatrix = new float[4][] {
            new float[3] {  1, -1, -1 },
            new float[3] { -1,  1, -1 },
            new float[3] {  1,  1,  1 },
            new float[3] { -1, -1,  1 }
        };

        SetRateType(RateType.Betaflight);
    }

    // Update is called once per frame
    void Update()
    {
        //This is the order of the axis in the controller 
        throttle = controls.Throttle?.ReadValue() ?? 0f;
        yaw = controls.Yaw?.ReadValue() ?? 0f;
        pitch = controls.Pitch?.ReadValue() ?? 0f;
        roll = controls.Roll?.ReadValue() ?? 0f;

        //This is the order of the axis in Unity physics
        float throttleSetpoint = (throttle + 1) / 2.0f;
        float pitchSetpoint = ComputeRate(0, pitch);
        float yawSetpoint = ComputeRate(1, yaw);
        float rollSetpoint = ComputeRate(2, roll) * -1.0f; // Invert roll axis to match Unity and Betaflight conventions

        //Angular Velocity is given in the World Frame and we need to convert it to the Local Frame for the PID controller
        Vector3 localAngularVelocity = transform.InverseTransformDirection(drone.angularVelocity);
        float pitchPID = PIDEquation(pitchSetpoint, localAngularVelocity.x, 0) / 1000.0f;
        float yawPID = PIDEquation(yawSetpoint, localAngularVelocity.y, 1) / 1000.0f;
        float rollPID = PIDEquation(rollSetpoint, localAngularVelocity.z, 2) / 1000.0f;

        float motorMin = float.MaxValue;
        float motorMax = float.MinValue;

        for (int i = 0; i < 4; i++)
        {
            motorMix[i] = motorMixMatrix[i][0] * pitchPID + motorMixMatrix[i][1] * yawPID + motorMixMatrix[i][2] * rollPID;
          
            motorMin = System.Math.Min(motorMin, motorMix[i]);
            motorMax = System.Math.Max(motorMax, motorMix[i]);
        }

        float motorRange = motorMax - motorMin;

        float normalizationFactor = motorRange > 1.0f ? 1.0f / motorRange : 1.0f;
        throttleSetpoint = Mathf.Clamp(throttleSetpoint, -motorMin * normalizationFactor, 1.0f - motorMax * normalizationFactor);

        for (int i = 0; i < 4; i++)
        {
            motorMix[i] = (throttleSetpoint + motorMix[i] * normalizationFactor);
        }
    }

    public void SetRateType(RateType type)
    {
        switch (type)
        {
            case RateType.Betaflight:
                ComputeRate = ComputeBetaflightRate;
                break;
            case RateType.Actual:
                ComputeRate = ComputeActualRate;
                break;
        }
    }

    float ComputeBetaflightRate(int axis, float input)
    {
        float inputAbs = Mathf.Abs(input);

        if(rcExpo[axis] != 0)
            input = input * inputAbs * inputAbs * inputAbs * rcExpo[axis] + input * (1 - rcExpo[axis]);

        float rcRate = rcRates[axis];
        if (rcRate > 2.0f)
            rcRate += RC_RATE_INCREMENTAL * (rcRate - 2.0f);
        
        float angleRate = 200.0f * rcRate * input;

        if(rates[axis] != 0)
            angleRate *=  1.0f / Mathf.Clamp((1.0f - (inputAbs * rates[axis])), 0.01f, 1.00f);
        
        return angleRate;
    }

    float ComputeActualRate(int axis, float input)
    {
        float inputAbs = Mathf.Abs(input);
        
        float input5 = input * input * input * input * input; //This is faster than Mathf.Pow(input, 5)
        float expof = inputAbs * (input5 * rcExpo[axis] + input * (1 - rcExpo[axis]));

        //TODO I believe we don't need to multiply by 10
        float centerSensitivity = rcRates[axis] * 10.0f;
        float stickMovement = Mathf.Max(0, rates[axis] * 10.0f - centerSensitivity);
        
        return input * centerSensitivity + stickMovement * expof;
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
                                                 //Debug.Log("Axis: " + axis + " measurement: " + measurements + " setpoint: " + (setpoint * Mathf.PI / 180f) + " P: " + P + " I: " + I + " D: " + D + " PID: " + PID);

        return PID;
    }
}