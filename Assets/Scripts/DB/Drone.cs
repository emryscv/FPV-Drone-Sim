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
    public int PropellerDiameter { get; set; } // This value is jsut an identifier for the propeller size, the actual size is determined by the propeller size list in the UI
    public float BatteryCapacity { get; set; }
    public float MaxThrust { get; set; }
    public int CameraAngle { get; set; }

    //TODO figure this out for yaw
    public float MotorTorqueConstant { get; set; }

}