using UnityEngine.UIElements;

[UxmlElement]
partial class FloatField : VisualElement
{
    [UxmlAttribute]
    public float Max { get; set; }

    [UxmlAttribute]
    public float Min { get; set; }

    private TextField _textField;
    private Button _incrementButton;
    private Button _decrementButton;

    private EventCallback<ChangeEvent<string>> _onValueChangedCallback;

    public FloatField()
    {
        AddToClassList("numerical-field");
        _textField = new TextField
        {
            name = "Value",
            value = Min.ToString()
        };

        _incrementButton = new Button(() =>
        {
            float value = float.Parse(_textField.value);
            if (value < Max)
            {
                value += 0.01f;
                _textField.value = value.ToString();
            }
        })
        { text = "+" };
        _decrementButton = new Button(() =>
        {
            float value = float.Parse(_textField.value);
            if (value > Min)
            {
                value -= 0.01f;
                _textField.value = value.ToString();
            }
        })
        { text = "-" };

        Add(_decrementButton);
        Add(_textField);
        Add(_incrementButton);
    }

    public void RegisterValueChangedCallback(EventCallback<ChangeEvent<string>> callback)
    {
        _onValueChangedCallback = (evt) =>
        {
            evt.StopPropagation();

            if (float.TryParse(evt.newValue, out float value))
                Set(value);
            else
            {
                _textField.value = Min.ToString();
                return;
            }

            callback(evt);
        };

        _textField.RegisterValueChangedCallback(_onValueChangedCallback);
    
    }

    public void UnregisterValueChangedCallback()
    {
        _textField.UnregisterValueChangedCallback(_onValueChangedCallback);
    }

    public void Set(float value)
    {
        if (value > Max) value = Max;
        if (value < Min) value = Min;
        _textField.value = value.ToString();
    }

    //TODO add Update so the button can be held till the right value
}