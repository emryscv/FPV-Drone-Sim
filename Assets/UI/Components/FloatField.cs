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

    public FloatField()
    {
        AddToClassList("numerical-field");
        _textField = new TextField();
        _textField.name = "Value";

        _incrementButton = new Button(() => { if (Value < Max) { Value += 0.1f; _textField.value = Value.ToString(); } }) { text = "+" };
        _decrementButton = new Button(() => { if (Value > Min) { Value -= 0.1f; _textField.value = Value.ToString(); } }) { text = "-" };

        //_container = new VisualElement();
        //_container.name = "ControlsContainer";
        //_container.Add(_incrementButton);
        //_container.Add(_decrementButton);

        Add(_decrementButton);
        Add(_textField);
        Add(_incrementButton);
        //Add(_container);
    
    }

    private void Set(float value) //TODO fix this
    {
        if (value > Max) value = Max;
        if (value < Min) value = Min;
        _textField.value = value.ToString();
    }   

    //TODO add Update so the button can be held till the right value
}