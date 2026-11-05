using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [SerializeField] public float objectiveTime;
    [SerializeField] public string objectivePopup;
    [SerializeField] public GameObject nextObjective;
    private Exercise exercise;

    private float currentTime;
    private int objectsInside;

    protected bool playerInRange;

    private void OnEnable()
    {
        currentTime = 0.0f;
        objectsInside = 0;
        playerInRange = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        objectsInside = 0;
        currentTime = 0.0f;
        exercise = GetComponentInParent<Exercise>();
        Debug.Log("Waypoint Start: " + exercise);
        if (objectivePopup != "")
        {
            GameManager.Instance.ShowPopup(objectivePopup);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //TODO delete this logic. Maybe hovering is not that important
        //Debug.Log("Objects inside: " + objectsInside);
        if (objectsInside > 0)
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
            //Debug.Log("Player is out of waypoint range. Current time: " + currentTime);
        }

        currentTime = Mathf.Clamp(currentTime, 0.0f, objectiveTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered by: " + transform.name);
        if (other.transform.parent.name == "PlayerCompoundCollider")
        {
            objectsInside++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger exited by: " + transform.name);
        if (other.transform.parent.name == "PlayerCompoundCollider")
        {
            objectsInside--;
        }
    }

    protected virtual void FinishObjective()
    {    
        exercise.SetCurrent(nextObjective);
     
        nextObjective.SetActive(true);
        gameObject.SetActive(false);
    }
}
