using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
   public static GameEvents Instance;

    void Awake()
    {
        Instance = this;
    }

    public event Action OnPause;
    public void Pause()
    {
        OnPause?.Invoke();
    }

    public event Action OnUnpause;
    public void Unpause()
    {
        OnUnpause?.Invoke();
    }

    public event Action OnRestart;
    public void Restart()
    {
        OnRestart?.Invoke();
    }

    public event Action OnSettingsOpened;
    public void SettingsOpened()
    {
        OnSettingsOpened?.Invoke();
    }
    
    public event Action OnSettingsClosed;
    public void SettingsClosed()
    {
        OnSettingsClosed?.Invoke();
    }

    public event Action OnCrash;
    public void Crash()
    {
        Debug.Log("Crash event triggered");
        OnCrash?.Invoke();
    }

    public event Action OnDisplayCrashIndicator;
    public void DisplayCrashIndicator()
    {
        OnDisplayCrashIndicator?.Invoke();
    }
    
    public event Action OnHideCrashIndicator;
    public void HideCrashIndicator()
    {
        OnHideCrashIndicator?.Invoke();
    }

    public event Action OnControllerConnectedUI;
    public void ControllerConnected()
    {
        Debug.Log("Controller connected event triggered");
        OnControllerConnectedUI?.Invoke();
    }

    public event Action OnControllerDisconnectedUI;
    public void ControllerDisconnected()
    {
        Debug.Log("Controller disconnected event triggered");
        OnControllerDisconnectedUI?.Invoke();
    }

    public event Action OnTimerStart;
    public void TimerStart()
    {
        OnTimerStart?.Invoke();
    }

    public event Action OnTimerEnd;
    public void TimerEnd()
    {
        OnTimerEnd?.Invoke();
    }

    public event Action OnNextExercise;
    public void NextExercise()
    {
        OnNextExercise?.Invoke();
    }
}
