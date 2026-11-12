using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;
using Cursor = UnityEngine.Cursor;
using System.Collections;
using System;



public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool isGameStarted;
    public bool isPaused;
    public bool isSettingOpened;
    public float timeScaleOrig;

    [SerializeField] TrainingManager trainingManager;

    GameObject drone;
    DronePhysics dronePhysics;
    FlightController droneFC;
    // Reference to the player's Rigidbody component
    VisualElement _PauseMenu;

    [Header("===Menus===")]
    [SerializeField] UIDocument uiManager;
    // [SerializeField] GameObject menuActive;
    // [SerializeField] GameObject menuPause;
    // [SerializeField] GameObject menuOptions;
    // [SerializeField] GameObject menuWin;
    // [SerializeField] GameObject menuLose;

    [Header("===Displayed Text===")]
    [SerializeField] TMP_Text currentObjectiveTime;
    [SerializeField] Slider objectiveSlider;
    [SerializeField] TextPopup popWindow;

    void Awake()
    {
        Instance = this;
        isGameStarted = false;
        isPaused = true;
        isSettingOpened = false;

        timeScaleOrig = Time.timeScale;
        Time.timeScale = 0;

        drone = GameObject.FindWithTag("Player");
        dronePhysics = drone.GetComponent<DronePhysics>();
        droneFC = drone.GetComponent<FlightController>();

        _PauseMenu = uiManager.rootVisualElement.Q<VisualElement>("PauseMenu");
        GameEvents.Instance.OnRestart += () => RestartSimulation(true, false);
        GameEvents.Instance.OnCrash += DroneCrash;
        GameEvents.Instance.OnSettingsOpened += OnSettingsOpened;
        GameEvents.Instance.OnSettingsClosed += OnSettingsClosed;
        GameEvents.Instance.OnTimerEnd += OnTimerEnd;
        GameEvents.Instance.OnNextExercise += OnNextExercise;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameStarted)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused && !isSettingOpened)
                {
                    UnpauseSimulation();
                    GameEvents.Instance.Unpause();
                }
                else
                {
                    PauseSimulation();
                    GameEvents.Instance.Pause();
                }
            }
            if (!isPaused)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    RestartSimulation(true, true);
                }
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    dronePhysics.ChangeCameraAngle(true);
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    dronePhysics.ChangeCameraAngle(false);
                }
                if(Input.GetKeyDown(KeyCode.M))
                {
                    GameEvents.Instance.FlightModeSwap();
                }
            }
        }
    }

    // ---- PAUSING ---- //
    public void StartSimulation()
    {
        isGameStarted = true;
        dronePhysics.ResetDroneState();

        GameEvents.Instance.TimerStart();
    }

    public void PauseSimulation()
    {
        isPaused = true;
        Time.timeScale = 0;

        droneFC.enabled = false;
        dronePhysics.enabled = false;
        //Cursor is handled here. Otherwise every UI view
        //would have to handle it independently
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void UnpauseSimulation()
    {
        isPaused = false;
        Time.timeScale = timeScaleOrig;

        droneFC.enabled = true;
        dronePhysics.enabled = true;
        //Cursor is handled here. Otherwise every UI view
        //would have to handle it independently
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void RestartSimulation(bool timer, bool pause)
    {   
        if (pause)
            PauseSimulation();

        dronePhysics.ResetDroneState();
        trainingManager.RestartExcercise();
        
        if (timer)
            GameEvents.Instance.TimerStart();
        else if (isPaused)
            UnpauseSimulation();
    }

    public void OnSettingsOpened()
    {
        isSettingOpened = true;
    }
    public void OnSettingsClosed()
    {
        isSettingOpened = false;
    }
    public void OnTimerEnd()
    {
        UnpauseSimulation();
    }
    private void OnNextExercise()
    {
        StartCoroutine(OnNextExerciseRoutine());
    }

    private IEnumerator OnNextExerciseRoutine()
    {  
        //This is a temporary solution but it works
        trainingManager.NextExcercise();
        dronePhysics.ResetDroneState();

        yield return new WaitForSecondsRealtime(0.1f);
        RestartSimulation(true, true);
    }

    public void StopSimulation()
    {
        //TODO fiund out how to reaload everyhing such as position, scene. etc.
        isGameStarted = false;

        PauseSimulation();
        trainingManager.ResetTraining();
    }

    private void DroneCrash()
    {
        StartCoroutine(DroneCrashCoroutine());
    }

    private IEnumerator DroneCrashCoroutine()
    {
        PauseSimulation();

        GameEvents.Instance.DisplayCrashIndicator();
        dronePhysics.Crash();
        Debug.Log("TrainingManager: " + trainingManager);
        yield return new WaitForSecondsRealtime(3);

        GameEvents.Instance.HideCrashIndicator();
        RestartSimulation(false, false);
    }

    // ---- WIN CONDITION FEEDBACK ---- //
    public void YouLose()
    {
        PauseSimulation();
        // menuActive = menuLose;
        // menuActive.SetActive(true);
    }

    public void YouWin()
    {
        PauseSimulation();
        // menuActive = menuWin;
        // menuActive.SetActive(true);
    }

    public void ShowPopup(string textMessage)
    {
        PauseSimulation();
        //menuActive = popWindow.gameObject;
        //menuActive.SetActive(true);
        popWindow.confirmButton.onClick.AddListener(PopupConfirm);
        popWindow.text.SetText(textMessage);
    }

    public void PopupConfirm()
    {
        UnpauseSimulation();
    }
}