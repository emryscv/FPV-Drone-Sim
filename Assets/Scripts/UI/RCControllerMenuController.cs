using UnityEngine;
using UnityEngine.UIElements;

public class RCControllerMenuController : MonoBehaviour
{
    private UIDocument _uiManager;
    private VisualElement _MainMenu;
    private VisualElement _RCControllerMenu; 

    private Button _backBtn;
    private Button _saveBtn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
 private void OnEnable()
    {
        _uiManager = GetComponent<UIDocument>();
        _MainMenu = _uiManager.rootVisualElement.Q<VisualElement>("MainMenu");
        _RCControllerMenu = _uiManager.rootVisualElement.Q<VisualElement>("RCControllerMenu");

        _backBtn = _RCControllerMenu.Q("BackButton") as Button;
        _saveBtn = _RCControllerMenu.Q("SaveButton") as Button;

        _backBtn.RegisterCallback<ClickEvent>(OnBackBtnClick);
    }

    private void OnDisable()
    {
        _backBtn.UnregisterCallback<ClickEvent>(OnBackBtnClick);  
    } 

    private void OnBackBtnClick(ClickEvent evt)
    {
        Debug.Log("El Animaah");
        _MainMenu.visible = true;
        _RCControllerMenu.visible = false;
    }

}
