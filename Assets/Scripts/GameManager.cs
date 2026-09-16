using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using SQLite;
using UnityEditor;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool isPaused;
    public float timeScaleOrig;

    GameObject player;
    FlightController playerScript; //TODO check this value

    [Header("===Menus===")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuOptions;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    [Header("===Displayed Text===")]
    [SerializeField] TMP_Text currentObjectiveTime;
    [SerializeField] Slider objectiveSlider;
    [SerializeField] TextPopup popWindow;


    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<FlightController>();
        //objectiveSlider.value = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Unpause();
            else
                Pause();
            
        }
        // if (Input.GetButtonDown("Cancel"))
        // {
        //     if (menuActive == null)
        //     {
        //         Pause();
        //         menuActive = menuPause;
        //         menuActive  .SetActive(true);
        //     }
        //     else if (menuActive == menuOptions)
        //     {
        //         menuActive.SetActive(false);
        //         menuActive = menuPause;
        //         menuActive.SetActive(true);
        //     }
        //     else if (menuActive == popWindow.gameObject)
        //     {
        //         PopupConfirm();
        //     }
        //     else
        //     {
        //         Unpause();
        //     }
        //}
    }

    // ---- PAUSING ---- //
    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Debug.Log("Game Paused");
    }
    public void Unpause()
    {
        isPaused = false;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Debug.Log("Game Unpaused");
    }

    // ---- WIN CONDITION FEEDBACK ---- //
    public void YouLose()
    {
        Pause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void YouWin()
    {
        Pause();
        menuActive = menuWin;
        menuActive.SetActive(true);
    }

    public void UpdateObjective(float currTime, float objectiveTime)
    {
        int intObjTime = (int)currTime + 1;
        if (intObjTime > (int)objectiveTime) intObjTime = (int)objectiveTime;
        if (currTime == 0.0f) intObjTime = 0;
        currentObjectiveTime.text = intObjTime.ToString();
        objectiveSlider.value = currTime / objectiveTime;
    }

    public void ShowPopup(string textMessage)
    {
        Pause();
        menuActive = popWindow.gameObject;
        menuActive.SetActive(true);
        popWindow.confirmButton.onClick.AddListener(PopupConfirm);
        popWindow.text.SetText(textMessage);
    }

    public void PopupConfirm()
    {
        Unpause();
    }
}