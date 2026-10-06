using UnityEngine.UIElements;

[UxmlElement]
partial class IntegerField : VisualElement
{
    [UxmlAttribute]
    public int Value { get => int.Parse(_textField.value); set => _textField.value = value.ToString(); }

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
        _textField = new TextField();
        _textField.name = "Value";

        _incrementButton = new Button(() => { if (Value < Max) { Value++; _textField.value = Value.ToString(); } }) { text = "+" };
        _decrementButton = new Button(() => { if (Value > Min) { Value--; _textField.value = Value.ToString(); } }) { text = "-" };

        //_container = new VisualElement();
        //_container.name = "ControlsContainer";
        //_container.Add(_incrementButton);
        //_container.Add(_decrementButton);

        Add(_decrementButton);
        Add(_textField);
        Add(_incrementButton);
        //Add(_container);
    
    }
}