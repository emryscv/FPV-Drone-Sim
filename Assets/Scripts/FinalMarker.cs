using UnityEngine;

public class FinalMarker : Waypoint
{
    private TrainingManager trainingManager;
    
    void Awake()
    {
        trainingManager = GetComponentInParent<TrainingManager>();
        playerInRange = false;
        Debug.Log("Final Marker Awake: " + trainingManager);   
    }

    protected override void FinishObjective()
    {
        gameObject.SetActive(false);
        GameEvents.Instance.NextExercise();
    }
}
