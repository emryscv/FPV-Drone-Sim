using UnityEngine;

public class WaypointSpawner : MonoBehaviour
{
    [SerializeField] float objectiveTime;
    [SerializeField] string objectivePopup;
    [SerializeField] WaypointSpawner nextObjective;
    [SerializeField] GameObject waypointPrefab;

    GameObject objectToSpawn;
    float originalObjectiveTime;
    string originalObjectivePopup;


    void Start()
    {
        originalObjectiveTime = waypointPrefab.GetComponent<Waypoint>().objectiveTime;
        originalObjectivePopup = waypointPrefab.GetComponent<Waypoint>().objectivePopup;
        objectToSpawn = waypointPrefab;
    }

    public void SpawnObjective()
    {
        objectToSpawn.GetComponent<Waypoint>().objectiveTime = objectiveTime;

        if (objectivePopup != "")
        {
            objectToSpawn.GetComponent<Waypoint>().objectivePopup = objectivePopup;
        }

        if (nextObjective != null)
        {
            objectToSpawn.GetComponent<Waypoint>().nextObjective = nextObjective;
        }

        GameObject.Instantiate(objectToSpawn, gameObject.transform);

        waypointPrefab.GetComponent<Waypoint>().objectiveTime = originalObjectiveTime;
        waypointPrefab.GetComponent<Waypoint>().objectivePopup = originalObjectivePopup;
        waypointPrefab.GetComponent<Waypoint>().nextObjective = null;
    }
}
