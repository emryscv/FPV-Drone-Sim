using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [SerializeField] public float objectiveTime;
    [SerializeField] public string objectivePopup;
    [SerializeField] public WaypointSpawner nextObjective;
    float currentTime;

    protected bool playerInRange;

    void Awake()
    {
        playerInRange = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentTime = 0.0f;
        if (objectivePopup != "")
        {
            GameManager.Instance.ShowPopup(objectivePopup);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //TODO delete this logic. Maybe hovering is not that important
        if (playerInRange)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= objectiveTime)
            {
                FinishObjective();
            }
        }
        else
        {
            currentTime -= Time.deltaTime;
        }

        currentTime = Mathf.Clamp(currentTime, 0.0f, objectiveTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    protected virtual void FinishObjective()
    {
        if (nextObjective != null)
        {
            nextObjective.SpawnObjective();
        }
        GameObject.Destroy(gameObject);
    }
}
