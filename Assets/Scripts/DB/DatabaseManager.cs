using SQLite;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

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

                // for(int i = 2; i < 34; i++)
                // {
                //     DeleteDrone(i); // Delete drones with IDs from 2 to 33 before inserting new ones
                // }

                // InsertDrone(new Drone()
                // {
                //     Name = "Second Drone",
                //     Mass = 1.5f,
                //     FrameSize = 250,
                //     PropellerDiameter = 5,
                //     MaxThrust = 10,
                //     BatteryCapacity = 1500,
                //     CameraAngle = 20
                // });
                // InsertDrone(new Drone()
                // {
                //     Name = "First Drone",
                //     Mass = 1.2f,
                //     FrameSize = 220,
                //     PropellerDiameter = 4,
                //     MaxThrust = 8,
                //     BatteryCapacity = 1200,
                //     CameraAngle = 15
                // });

                // InsertDrone(new Drone()
                // {
                //     Name = "Third Drone",
                //     Mass = 1.8f,
                //     FrameSize = 300,
                //     PropellerDiameter = 6,
                //     MaxThrust = 12,
                //     BatteryCapacity = 2000,
                //     CameraAngle = 25
                // });

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

    public List<Drone> GetAllDrones()
    {
        using SQLiteConnection database = CreateConnection();
        return database.Table<Drone>().ToList();
    }

    public void DeleteDrone(int droneId)
    {
        using SQLiteConnection database = CreateConnection();
        database.RunInTransaction(() =>
        {
            database.Delete<Drone>(droneId);
            database.Delete<PID>(droneId);
        });
    }
}