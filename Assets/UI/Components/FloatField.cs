using UnityEngine.UIElements;

[UxmlElement]
partial class FloatField : VisualElement
{
    [UxmlAttribute]
    public float Value { get => float.Parse(_textField.value); set => Set(value); }

    [UxmlAttribute]
    public float Max { get; set; }

    [UxmlAttribute]
    public float Min { get; set; }

    private TextField _textField;
    private VisualElement _container;
    private Button _incrementButton;
    private Button _decrementButton;
    //TODO Fix the way value is being held i don't think we need the Extra value. TryParse will solve some issues with input validation
    public FloatField()
    {
        AddToClassList("numerical-field");
        _textField = new TextField{name = "Value"};

        _incrementButton = new Button(() => { 
            if (Value < Max) { 
                Value += 0.01f; 
                _textField.value = Value.ToString(); 
            } 
        }) { text = "+" };
        _decrementButton = new Button(() => { 
            if (Value > Min) { 
                Value -= 0.01f;
                _textField.value = Value.ToString(); 
            } 
        }) { text = "-" };

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

    public void Set(float value) //TODO fix this
    {
        if (value > Max) value = Max;
        if (value < Min) value = Min;
        _textField.value = value.ToString();
    }

    //TODO add Update so the button can be held till the right value
}