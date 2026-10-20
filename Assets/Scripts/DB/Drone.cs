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
    //TODO Add UI for it
    public float MotorTorqueConstant { get; set; }

}