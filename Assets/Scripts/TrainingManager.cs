using UnityEngine;

public class TrainingManager : MonoBehaviour
{
    private GameObject[] excercises;
    int currentExcercise;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {        
        Exercise[] exerciseComponents = GameObject.Find("Excercises")
            .GetComponentsInChildren<Exercise>(true);

        excercises = new GameObject[exerciseComponents.Length];
        
        for (int i = 0; i < exerciseComponents.Length; i++)
        {
            excercises[i] = exerciseComponents[i].gameObject;
            Debug.Log("Exercise added: " + excercises[i].name);
        }

        foreach (var excercise in excercises)
        {
            excercise.SetActive(false);
        }
        
        currentExcercise = 0;
        excercises[currentExcercise].SetActive(true);
        
    }
    public void NextExcercise()
    {
        excercises[currentExcercise].SetActive(false);
        RestartExcercise();

        currentExcercise = (currentExcercise + 1) % excercises.Length;
        
        excercises[currentExcercise].SetActive(true);
    }

    public void RestartExcercise()
    {
        excercises[currentExcercise].GetComponent<Exercise>().Restart();
    }

    public void ResetTraining()
    {
        RestartExcercise();
        excercises[currentExcercise].SetActive(false);
        
        currentExcercise = 0;
        excercises[currentExcercise].SetActive(true);
    }
}