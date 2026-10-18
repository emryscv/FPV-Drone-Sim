using SQLite;

public class PID
{
    // --- Drone MetaData --- //   
    // SQLite doesn't have ForeignKey attributes at least not with this library
    // We will be doing it artificially by forcing this table Ids to match the Drone table's Ids
    [PrimaryKey]
    public int DroneId { get; set; }
    
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