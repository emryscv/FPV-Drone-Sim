using UnityEngine.UIElements;

[UxmlElement]
partial class IntegerField : VisualElement
{
    [UxmlAttribute]
    public int Max { get; set; }

    [UxmlAttribute]
    public int Min { get; set; }

    private TextField _textField;
    private Button _incrementButton;
    private Button _decrementButton;

    private EventCallback<ChangeEvent<string>> _onValueChangedCallback;
    
    public IntegerField()
    {
        AddToClassList("numerical-field");
        _textField = new TextField
        {
            name = "Value",
            value = Min.ToString()
        };

        _incrementButton = new Button(() =>
        {
            int value = int.Parse(_textField.value);
            if (value < Max)
            {
                value++;
                _textField.value = value.ToString();
            }
        })
        { text = "+" };
        _decrementButton = new Button(() =>
        {
            int value = int.Parse(_textField.value);
            if (value > Min)
            {
                value--;
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

            if (int.TryParse(evt.newValue, out int value))
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

    public void Set(int value)
    {
        if (value > Max) value = Max;
        if (value < Min) value = Min;

        _textField.value = value.ToString();
    }

    //TODO add Update so the button can be held till the right value
}