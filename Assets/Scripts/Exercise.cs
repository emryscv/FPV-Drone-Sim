using UnityEngine;

public class Exercise : MonoBehaviour
{
    [SerializeField] private GameObject startingWaypoint;
    private GameObject current;

    public GameObject Current => current;

    private void Awake()
    {
        current = startingWaypoint;
    }

    public void Restart()
    {
        Debug.Log("Restarting exercise: " + current.name);
        current.SetActive(false);
        
        current = startingWaypoint;
        Debug.Log("Current waypoint reset to starting waypoint: " + current.name);
        current.SetActive(true);
        Debug.Log("Exercise restarted. Current waypoint reset to starting waypoint.");
    }

    public void SetCurrent(GameObject waypoint)
    {
        current = waypoint;
    }
}
