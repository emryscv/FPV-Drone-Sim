using UnityEngine;
using UnityEngine.UIElements;

public class MapSelectorController : MonoBehaviour
{
    [SerializeField] public Texture2D _HangarThumbnail;
    [SerializeField] public Texture2D _FieldThumbnail;
    

    private UIDocument _uiManager;
    private VisualElement _MapSelectorMenu;
    private VisualElement _MainMenu;
    private VisualElement _HUD;

    private Button _backButton;
    private Button _prevButton;
    private Image _prevCard;
    private Label _title;
    private Image _thumbnail;
    private Button _startButton;
    private Image _nextCard;
    private Button _nextButton;

    private Map[] _maps;

    private Map _selectedMap;
    private int _selectedMapIndex;

    private void OnEnable()
    {
        _maps = new Map[2];
        _maps[0] = new Map { name = "THE HANGAR", thumbnail = _HangarThumbnail };
        _maps[1] = new Map { name = "THE FIELD", thumbnail = _FieldThumbnail };
        _selectedMap = _maps[0];
        _selectedMapIndex = 0;

        _uiManager = GetComponent<UIDocument>();
        _MapSelectorMenu = _uiManager.rootVisualElement.Q<VisualElement>("MapSelectorMenu");
        _MainMenu = _uiManager.rootVisualElement.Q<VisualElement>("MainMenu");
        _HUD = _uiManager.rootVisualElement.Q<VisualElement>("HUD");

        _backButton = _MapSelectorMenu.Q<Button>("BackButton");
        _prevButton = _MapSelectorMenu.Q<Button>("PrevButton");
        _prevCard = _MapSelectorMenu.Q<Image>("PrevCard");
        _title = _MapSelectorMenu.Q<Label>("Title");
        _thumbnail = _MapSelectorMenu.Q<Image>("Thumbnail");
        _startButton = _MapSelectorMenu.Q<Button>("FlyNowBtn");
        _nextCard = _MapSelectorMenu.Q<Image>("NextCard");
        _nextButton = _MapSelectorMenu.Q<Button>("ForwardButton");


        _backButton.RegisterCallback<ClickEvent>(OnBackButtonClicked);
        _prevButton.RegisterCallback<ClickEvent>(OnPrevButtonClicked);
        _startButton.RegisterCallback<ClickEvent>(OnStartButtonClicked);
        _nextButton.RegisterCallback<ClickEvent>(OnNextButtonClicked);
        SetCards();
    }

    private void OnDisable()
    {
        _backButton.UnregisterCallback<ClickEvent>(OnBackButtonClicked);
        _prevButton.UnregisterCallback<ClickEvent>(OnPrevButtonClicked);
        _startButton.UnregisterCallback<ClickEvent>(OnStartButtonClicked);
        _nextButton.UnregisterCallback<ClickEvent>(OnNextButtonClicked);
    }

    private void OnBackButtonClicked(ClickEvent evt)
    {
        Debug.Log("BackButtonClicked");
        _MapSelectorMenu.style.display = DisplayStyle.None;
        _MainMenu.style.display = DisplayStyle.Flex;
    }

    private void OnPrevButtonClicked(ClickEvent evt)
    {
        Debug.Log("PrevButtonClicked");
        _selectedMapIndex = (_selectedMapIndex - 1 + _maps.Length) % _maps.Length;
        SetCards();
    }

    private void OnStartButtonClicked(ClickEvent evt)
    {
        Debug.Log("StartButtonClicked");
        _HUD.style.display = DisplayStyle.Flex;
        _MapSelectorMenu.style.display = DisplayStyle.None;
        GameManager.Instance.StartSimulation();
    }

    private void OnNextButtonClicked(ClickEvent evt)
    {
        Debug.Log("NextButtonClicked");
        _selectedMapIndex = (_selectedMapIndex + 1) % _maps.Length;
        SetCards();
    }

    private void SetCards()
    {
        _selectedMap = _maps[_selectedMapIndex];
        _title.text = _selectedMap.name;
        _thumbnail.image = _selectedMap.thumbnail;
        _prevCard.image = _maps[(_selectedMapIndex - 1 + _maps.Length) % _maps.Length].thumbnail;
        _nextCard.image = _maps[(_selectedMapIndex + 1) % _maps.Length].thumbnail;
    }
}
