using SQLite;
using UnityEngine;
using System.IO;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }
    private SQLiteConnection database;
    
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
            string path = Path.Combine(Application.persistentDataPath, "SimulatorData.db");
            Debug.Log("Initializing database at: " + path);

            database = new SQLiteConnection(path);
            database.CreateTable<Drone>();

            Debug.Log("Database initialized successfully");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Database initialization failed: " + ex.Message);
        }
    }
}