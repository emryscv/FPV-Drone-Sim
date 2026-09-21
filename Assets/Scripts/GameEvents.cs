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
        if (OnPause != null)
            OnPause.Invoke();
    }

    public event Action OnUnpause;
    public void Unpause()
    {
        if (OnUnpause != null)
            OnUnpause.Invoke();
    }

}
