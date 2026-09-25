using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;
using Cursor = UnityEngine.Cursor;
using System.Collections;



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
        timeScaleOrig = Time.timeScale;
        Time.timeScale = 0;

        drone = GameObject.FindWithTag("Player");
        dronePhysics = drone.GetComponent<DronePhysics>();
        droneFC = drone.GetComponent<FlightController>();

        _PauseMenu = uiManager.rootVisualElement.Q<VisualElement>("PauseMenu");
        GameEvents.Instance.OnRestart += RestartSimulation;
        GameEvents.Instance.OnCrash += DroneCrash;
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
                    UnpauseSimulation();
                    GameEvents.Instance.Unpause();
                }
                else
                {
                    PauseSimulation();
                    GameEvents.Instance.Pause();
                }
            }
            if (Input.GetKeyDown(KeyCode.R) && !isPaused)
            {
                RestartSimulation();
            }
        }
    }

    // ---- PAUSING ---- //
    public void StartSimulation()
    {
        droneFC.enabled = true;
        dronePhysics.enabled = true;
        isGameStarted = true;

        dronePhysics.ResetDroneState();

        UnpauseSimulation();
    }

    public void PauseSimulation()
    {
        isPaused = true;
        Time.timeScale = 0;

        //Cursor is handled here. Otherwise every UI view
        //would have to handle it independently
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void UnpauseSimulation()
    {
        isPaused = false;
        Time.timeScale = timeScaleOrig;

        //Cursor is handled here. Otherwise every UI view
        //would have to handle it independently
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void RestartSimulation()
    {
        dronePhysics.ResetDroneState();
    }

    public void StopSimulation()
    {
        //TODO fiund out how to reaload everyhing such as position, scene. etc.
        droneFC.enabled = false;
        dronePhysics.enabled = false;
        isGameStarted = false;

        PauseSimulation();
    }
   
    private void DroneCrash()
    {
        StartCoroutine(DroneCrashCoroutine());
    }

    private IEnumerator DroneCrashCoroutine()
    {
        droneFC.enabled = false;
        dronePhysics.enabled = false;

        GameEvents.Instance.DisplayCrashIndicator();
        dronePhysics.Crash();
        yield return new WaitForSeconds(3);

        dronePhysics.ResetDroneState();
        GameEvents.Instance.HideCrashIndicator();

        droneFC.enabled = true;
        dronePhysics.enabled = true;
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