using SQLite;
using UnityEngine;
using System.IO;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }
    private string databasePath;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        try
        {
            databasePath = Path.Combine(Application.persistentDataPath, "SimulatorData.db");
            Debug.Log("Initializing database at: " + databasePath);

            using (SQLiteConnection database = CreateConnection())
            {
                database.CreateTable<Drone>();
                database.CreateTable<PID>();
            }

            Debug.Log("Database initialized successfully");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Database initialization failed: " + ex.Message);
        }
    }

    private SQLiteConnection CreateConnection()
    {
        return new SQLiteConnection(databasePath);
    }

    public void InsertDrone(Drone drone)
    {
        using SQLiteConnection database = CreateConnection();
        database.RunInTransaction(() =>
        {
            database.Insert(drone);
            database.Insert(new PID
            {
                DroneId = drone.Id,
                RollP = 0,
                RollI = 0,
                RollD = 0,
                PitchP = 0,
                PitchI = 0,
                PitchD = 0,
                YawP = 0,
                YawI = 0,
                YawD = 0
            });
        });
    }

    public void UpdateDrone(Drone drone)
    {
        using SQLiteConnection database = CreateConnection();
        database.Update(drone);
    }

    public void UpdatePID(PID pid)
    {
        using SQLiteConnection database = CreateConnection();
        database.Update(pid);
    }

    public Drone GetDrone(int droneId)
    {
        using SQLiteConnection database = CreateConnection();
        return database.Find<Drone>(droneId);
    }

    public PID GetPID(int droneId)
    {
        using SQLiteConnection database = CreateConnection();
        return database.Find<PID>(droneId);
    }
}