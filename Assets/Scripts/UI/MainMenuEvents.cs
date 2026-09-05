using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuEvents : MonoBehaviour
{
    private UIDocument _uiManager;
    private Button _rcControllerNav;
    
    private void Awake()
    {
        _uiManager = GetComponent<UIDocument>();
        _rcControllerNav = _uiManager.rootVisualElement.Q("RCControllerButton") as Button;

        _rcControllerNav.RegisterCallback<ClickEvent>(OnRCControllerBtnClick);
    }

    private void OnDisable()
    {
        _rcControllerNav.UnregisterCallback<ClickEvent>(OnRCControllerBtnClick);  
    } 

    private void OnRCControllerBtnClick(ClickEvent evt)
    {
        Debug.Log("Test - Button Pressed!!");
    }

}
