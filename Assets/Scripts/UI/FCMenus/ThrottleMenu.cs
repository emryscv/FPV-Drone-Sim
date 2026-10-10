using UnityEngine;
using UnityEngine.UIElements;

public class ThrottleMenu : MonoBehaviour
{
    [SerializeField] private FlightController _flightController;

    private UIDocument _uiManager;
    private VisualElement _FCMenuTab;

    //Throttle
    private FloatField _ThrottleMid;
    private FloatField _ThrottleExpo;
    private ThrottleCurveGraphElement _throttleCurveGraph;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        _uiManager = GetComponent<UIDocument>();
        _FCMenuTab = _uiManager.rootVisualElement.Q<VisualElement>("FCMenuTab");

        //Throttle
        _ThrottleMid = _FCMenuTab.Q<FloatField>("ThrottleMid");
        _ThrottleExpo = _FCMenuTab.Q<FloatField>("ThrottleExpo");

        VisualElement throttleGraphPlaceholder = _FCMenuTab
            .Q<VisualElement>("Throttle")
            .Q<VisualElement>("Graph");

        if (throttleGraphPlaceholder != null)
        {
            throttleGraphPlaceholder.Clear();
            _throttleCurveGraph = new ThrottleCurveGraphElement();
            throttleGraphPlaceholder.Add(_throttleCurveGraph);
        }
        
        _ThrottleMid.RegisterCallback<ChangeEvent<string>>(OnThrottleMidChanged);
        _ThrottleExpo.RegisterCallback<ChangeEvent<string>>(OnThrottleExpoChanged);
    }

    private void OnDisable()
    {
        _ThrottleMid.UnregisterCallback<ChangeEvent<string>>(OnThrottleMidChanged);
        _ThrottleExpo.UnregisterCallback<ChangeEvent<string>>(OnThrottleExpoChanged);
    }

    private void OnThrottleMidChanged(ChangeEvent<string> evt)
    {
        Debug.Log("Throttle Mid changed to: " + evt.newValue);
        _throttleCurveGraph?.MarkDirtyRepaint();
    }
    
    private void OnThrottleExpoChanged(ChangeEvent<string> evt)
    {
        Debug.Log("Throttle Expo changed to: " + evt.newValue);
        _throttleCurveGraph?.MarkDirtyRepaint();
    }

}

//TODO get this out of here
public class ThrottleCurveGraphElement : VisualElement
{
    private static readonly Color BackgroundColor = new Color(0.03f, 0.05f, 0.07f, 1f);
    private static readonly Color GridColor = new Color(0.95f, 0.95f, 0.95f, 0.08f);
    private static readonly Color AxisColor = new Color(0.95f, 0.95f, 0.95f, 0.2f);
    private static readonly Color LineColor = new Color(0.83f, 0.64f, 0.29f, 1f);

    public ThrottleCurveGraphElement()
    {
        pickingMode = PickingMode.Ignore;
        style.flexGrow = 1;
        generateVisualContent += OnGenerateVisualContent;
        schedule.Execute(MarkDirtyRepaint).Every(33);
    }

    private void OnGenerateVisualContent(MeshGenerationContext context)
    {
        Rect rect = contentRect;
        if (rect.width < 5f || rect.height < 5f)
        {
            return;
        }

        Painter2D painter = context.painter2D;
        DrawBackground(painter, rect);
        DrawGrid(painter, rect);
        DrawAxes(painter, rect);
        DrawDiagonalLine(painter, rect);
    }

    private static void DrawBackground(Painter2D painter, Rect rect)
    {
        painter.fillColor = BackgroundColor;
        painter.BeginPath();
        painter.MoveTo(new Vector2(rect.xMin, rect.yMin));
        painter.LineTo(new Vector2(rect.xMax, rect.yMin));
        painter.LineTo(new Vector2(rect.xMax, rect.yMax));
        painter.LineTo(new Vector2(rect.xMin, rect.yMax));
        painter.ClosePath();
        painter.Fill();
    }

    private static void DrawGrid(Painter2D painter, Rect rect)
    {
        painter.strokeColor = GridColor;
        painter.lineWidth = 1f;

        for (int i = 1; i < 4; i++)
        {
            float t = i / 4f;
            float x = Mathf.Lerp(rect.xMin, rect.xMax, t);
            float y = Mathf.Lerp(rect.yMin, rect.yMax, t);

            painter.BeginPath();
            painter.MoveTo(new Vector2(x, rect.yMin));
            painter.LineTo(new Vector2(x, rect.yMax));
            painter.Stroke();

            painter.BeginPath();
            painter.MoveTo(new Vector2(rect.xMin, y));
            painter.LineTo(new Vector2(rect.xMax, y));
            painter.Stroke();
        }
    }

    private static void DrawAxes(Painter2D painter, Rect rect)
    {
        painter.strokeColor = AxisColor;
        painter.lineWidth = 1.5f;

        painter.BeginPath();
        painter.MoveTo(new Vector2(rect.xMin, rect.yMax));
        painter.LineTo(new Vector2(rect.xMax, rect.yMax));
        painter.Stroke();

        painter.BeginPath();
        painter.MoveTo(new Vector2(rect.xMin, rect.yMin));
        painter.LineTo(new Vector2(rect.xMin, rect.yMax));
        painter.Stroke();
    }

    private static void DrawDiagonalLine(Painter2D painter, Rect rect)
    {
        float paddingX = rect.width * 0.05f;
        float paddingY = rect.height * 0.05f;

        painter.strokeColor = LineColor;
        painter.lineWidth = 2f;
        painter.BeginPath();
        painter.MoveTo(new Vector2(rect.xMin + paddingX, rect.yMax - paddingY));
        painter.LineTo(new Vector2(rect.xMax - paddingX, rect.yMin + paddingY));
        painter.Stroke();
    }
}
    