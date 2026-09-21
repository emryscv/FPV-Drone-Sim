using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using SQLite;
using UnityEditor;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;
using Cursor = UnityEngine.Cursor;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool isGameStarted;
    public bool isPaused;
    public float timeScaleOrig;

    GameObject drone;
    DronePhysics dronePhysics;
    FlightController droneFC;
    // Reference to the player's Rigidbody component
    VisualElement _PauseMenu;

    [Header("===Menus===")]
    [SerializeField] UIDocument uiManager;
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
        Instance = this;
        isGameStarted = false;
        isPaused = true;
        timeScaleOrig = Time.timeScale;
        Time.timeScale = 0;

        drone = GameObject.FindWithTag("Player");
        dronePhysics = drone.GetComponent<DronePhysics>();
        droneFC = drone.GetComponent<FlightController>();

        _PauseMenu = uiManager.rootVisualElement.Q<VisualElement>("PauseMenu");
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameStarted)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused)
                {
                    Unpause();
                    GameEvents.Instance.Unpause();
                }
                else
                {
                    Pause();
                    GameEvents.Instance.Pause();
                }

            }
            if (Input.GetKeyDown(KeyCode.R) && !isPaused)
            {
                //---- Restart ---- //
                dronePhysics.ResetDroneState();
            }
        }
    }

    // ---- PAUSING ---- //
    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0;

        //Cursor is handlke here otherwise every UI view
        //would have to handle it independently
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Unpause()
    {
        isPaused = false;
        Time.timeScale = timeScaleOrig;

        //Cursor is handlke here otherwise every UI view
        //would have to handle it independently
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void StartSimulation()
    {
        droneFC.enabled = true;
        dronePhysics.enabled = true;
        isGameStarted = true;

        Unpause();
    }

    public void StopSimulation()
    {
        //TODO fiund out how to reaload everyhing such as position, scene. etc.
        droneFC.enabled = false;
        dronePhysics.enabled = false;
        isGameStarted = false;
        
        Pause();
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