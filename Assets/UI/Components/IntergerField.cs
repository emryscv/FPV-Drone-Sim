using UnityEngine.UIElements;

[UxmlElement]
partial class IntegerField : VisualElement
{
    [UxmlAttribute]
    public int Value { get => int.Parse(_textField.value); set => Set(int.Parse(value.ToString())); }

    [UxmlAttribute]
    public int Max { get; set; }

    [UxmlAttribute]
    public int Min { get; set; }

    private TextField _textField;
    private VisualElement _container;
    private Button _incrementButton;
    private Button _decrementButton;

    public IntegerField()
    {
        AddToClassList("numerical-field");
        _textField = new TextField{name = "Value"};

        _incrementButton = new Button(() => { if (Value < Max) { Value++; _textField.value = Value.ToString(); } }) { text = "+" };
        _decrementButton = new Button(() => { if (Value > Min) { Value--; _textField.value = Value.ToString(); } }) { text = "-" };

        Add(_decrementButton);
        Add(_textField);
        Add(_incrementButton);

    }

    public void RegisterValueChangedCallback(
    EventCallback<ChangeEvent<string>> callback)
    {
        _textField.RegisterValueChangedCallback(callback);
    }

    public void UnregisterValueChangedCallback(
        EventCallback<ChangeEvent<string>> callback)
    {
        _textField.UnregisterValueChangedCallback(callback);
    }

    public void Set(int value) //TODO fix this
    {
        if (value > Max) value = Max;
        if (value < Min) value = Min;
        _textField.value = value.ToString();
    }

    //TODO add Update so the button can be held till the right value
}