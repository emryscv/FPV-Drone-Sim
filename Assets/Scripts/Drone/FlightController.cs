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
    public float[] KP { get; private set; }
    public float[] KI { get; private set; }
    public float[] KD { get; private set; }

    //PID cumulative terms
    float[] cumulativeI;
    float[] prevError;
    float[] prevTime;

    //BetaflightRates
    //For Actual Rates assume that rcRates is center sensitivity 
    //and rates is stick movement sensitivity
    public float[] RCRates { get; private set; }
    public float[] Rates { get; private set; }
    public float[] RCExpo { get; private set; }
    float RC_RATE_INCREMENTAL = 14.54f;

    //OutputRotor Thrusts
    public float[] MotorMix { get; private set; }
    float[][] motorMixMatrix;

    public RateEquation ComputeRate;

    void Awake()
    {
        controls = RCControllerManager.Instance;

        drone = GetComponent<Rigidbody>(); // Get the Rigidbody component attached to the same GameObject

        prevTime = new float[3] { Time.time, Time.time, Time.time };
        prevError = new float[3] { 0.0f, 0.0f, 0.0f };
        cumulativeI = new float[3] { 0.0f, 0.0f, 0.0f };

        //TODO we need to tune these constants
        KP = new float[3] { 200.0f, 500.0f, 200.0f };
        KI = new float[3] { 0.0f, 45.0f, 0.0f };
        KD = new float[3] { 0.0f, 0.0f, 0.0f };

        RCRates = new float[3] { 1.0f, 1.0f, 1.0f };
        Rates = new float[3] { 0.7f, 0.7f, 0.7f };
        RCExpo = new float[3] { 0.1f, 0.1f, 0.1f };

        MotorMix = new float[4] { 0f, 0f, 0f, 0f };
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
            MotorMix[i] = motorMixMatrix[i][0] * pitchPID + motorMixMatrix[i][1] * yawPID + motorMixMatrix[i][2] * rollPID;
          
            motorMin = System.Math.Min(motorMin, MotorMix[i]);
            motorMax = System.Math.Max(motorMax, MotorMix[i]);
        }

        float motorRange = motorMax - motorMin;

        float normalizationFactor = motorRange > 1.0f ? 1.0f / motorRange : 1.0f;
        throttleSetpoint = Mathf.Clamp(throttleSetpoint, -motorMin * normalizationFactor, 1.0f - motorMax * normalizationFactor);

        for (int i = 0; i < 4; i++)
        {
            MotorMix[i] = throttleSetpoint + MotorMix[i] * normalizationFactor;
        }
    }

    // --- Rate computation functions --- //
    float ComputeBetaflightRate(int axis, float input)
    {
        float inputAbs = Mathf.Abs(input);

        if(RCExpo[axis] != 0)
            input = input * inputAbs * inputAbs * inputAbs * RCExpo[axis] + input * (1 - RCExpo[axis]);

        float rcRate = RCRates[axis];
        if (rcRate > 2.0f)
            rcRate += RC_RATE_INCREMENTAL * (rcRate - 2.0f);
        
        float angleRate = 200.0f * rcRate * input;

        if(Rates[axis] != 0)
            angleRate *=  1.0f / Mathf.Clamp((1.0f - inputAbs * Rates[axis]), 0.01f, 1.00f);
        
        return angleRate;
    }

    float ComputeActualRate(int axis, float input)
    {
        float inputAbs = Mathf.Abs(input);
        
        float input5 = input * input * input * input * input; //This is faster than Mathf.Pow(input, 5)
        float expof = inputAbs * (input5 * RCExpo[axis] + input * (1 - RCExpo[axis]));

        //TODO I believe we don't need to multiply by 10
        float centerSensitivity = RCRates[axis] * 10.0f;
        float stickMovement = Mathf.Max(0, Rates[axis] * 10.0f - centerSensitivity);
        
        return input * centerSensitivity + stickMovement * expof;
    }

    float PIDEquation(float setpoint, float measurements, int axis)
    {
        float deltaTime = Time.time - prevTime[axis];
        prevTime[axis] = Time.time;

        float error = setpoint * Mathf.PI / 180f - measurements; //TODO fix this
                                                                 //Debug.Log("Axis: " + axis + " measurement: " + measurements + " setpoint: " + (setpoint * Mathf.PI / 180f) + " error: " + error);
                                                                 //Proportional term
        float P = KP[axis] * error;

        //Integral term
        float I = cumulativeI[axis] + KI[axis] * error * deltaTime;
        I = System.Math.Clamp(I, -400, 400); //Betaflight values
        cumulativeI[axis] = I;

        //Derivative term
        float D = deltaTime == 0 ? 0 : KD[axis] * (error - prevError[axis]) / deltaTime;

        prevError[axis] = error;

        float PID = P + I + D;
        PID = System.Math.Clamp(PID, -500, 500); //Betaflight values
                                                 //Debug.Log("Axis: " + axis + " measurement: " + measurements + " setpoint: " + (setpoint * Mathf.PI / 180f) + " P: " + P + " I: " + I + " D: " + D + " PID: " + PID);

        return PID;
    }

    // --- Setters --- //
    public void SetP(int axis, float value)
    {
        KP[axis] = value;
    }
    
    public void SetI(int axis, float value)
    {
        KI[axis] = value;
    }

    public void SetD(int axis, float value)
    {
        KD[axis] = value;
    }

    public void SetRCRate(int axis, float value)
    {
        RCRates[axis] = value;
    }
    
    public void SetRate(int axis, float value)
    {
        Rates[axis] = value;
    }   

    public void SetExpo(int axis, float value)
    {
        RCExpo[axis] = value;
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
}
