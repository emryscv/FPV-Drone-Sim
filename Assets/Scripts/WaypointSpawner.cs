using UnityEngine;

public class WaypointSpawner : MonoBehaviour
{
    [SerializeField] float objectiveTime;
    [SerializeField] string objectivePopup;
    [SerializeField] WaypointSpawner nextObjective;
    [SerializeField] GameObject objectivePrefab;

    GameObject objectToSpawn;
    float originalObjectiveTime;
    string originalObjectivePopup;


    void Start()
    {
        originalObjectiveTime = objectivePrefab.GetComponent<Waypoint>().objectiveTime;
        originalObjectivePopup = objectivePrefab.GetComponent<Waypoint>().objectivePopup;
        objectToSpawn = objectivePrefab;
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

        objectivePrefab.GetComponent<Waypoint>().objectiveTime = originalObjectiveTime;
        objectivePrefab.GetComponent<Waypoint>().objectivePopup = originalObjectivePopup;
        objectivePrefab.GetComponent<Waypoint>().nextObjective = null;
    }
}
