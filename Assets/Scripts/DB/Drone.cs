using SQLite;

public class Drone
{
    
    // --- Drone MetaData --- //
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; }

    // --- Drone Physical Properties --- //
    public float Mass { get; set; }
    public float FrameSize { get; set; }
    public float PropellerDiameter { get; set; }
    public float BatteryCapacity { get; set; }
    public float MaxThrust { get; set; }
    public int CameraAngle { get; set; }

    //TODO figure this out for yaw
    public float MotorTorqueConstant { get; set; }

    // --- PID --- //
    //Roll
    public float RollP{ get; set; } 
    public float RollI { get; set; } 
    public float RollD { get; set; } 

    //Pitch
    public float PitchP { get; set; } 
    public float PitchI { get; set; } 
    public float PitchD { get; set; } 

    //Yaw
    public float YawP { get; set; } 
    public float YawI { get; set; } 
    public float YawD { get; set; } 
}