using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
partial class IntegerField : VisualElement
{
    [UxmlAttribute]
    public int Max { get; set; }

    [UxmlAttribute]
    public int Min { get; set; }

    private TextField _textField;
    private RepeatButton _incrementButton;
    private RepeatButton _decrementButton;

    private EventCallback<ChangeEvent<string>> _onValueChangedCallback;

    public IntegerField()
    {
        AddToClassList("numerical-field");
        _textField = new TextField
        {
            name = "Value",
            value = Min.ToString()
        };

        _incrementButton = new RepeatButton(() =>
        {
            int value = int.Parse(_textField.value);
            if (value < Max)
            {
                value++;
                _textField.value = value.ToString();
            }
        }, 250, 100)
        { text = "+" };
        _decrementButton = new RepeatButton(() =>
        {
            int value = int.Parse(_textField.value);
            if (value > Min)
            {
                value--;
                _textField.value = value.ToString();
            }
        }, 250, 100)
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
        bool isItValid = true;
        if (value > Max) { value = Max; isItValid = false; }
        if (value < Min) { value = Min; isItValid = false; }

        if (isItValid)
            _textField.SetValueWithoutNotify(value.ToString());
        else
            _textField.value = value.ToString();
    }

    //TODO add Update so the button can be held till the right value
}