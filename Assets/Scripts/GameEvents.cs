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

    public event Action OnCrash;
    public void Crash()
    {
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
}
