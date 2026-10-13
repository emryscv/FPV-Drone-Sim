using UnityEngine;
using UnityEngine.UIElements;

public class RatesMenu : MonoBehaviour
{
    [SerializeField] private FlightController _flightController;

    private UIDocument _uiManager;
    private VisualElement _FCMenuTab;
    private EnumField _RateType;

    private VisualElement _RCRateCol;
    private VisualElement _RateCol;
    private VisualElement _CenterSensitivityCol;
    private VisualElement _MaxRateCol;

    //Betaflight Rates
    private FloatField _RollRCRate;
    private FloatField _PitchRCRate;
    private FloatField _YawRCRate;

    private FloatField _RollRate;
    private FloatField _PitchRate;
    private FloatField _YawRate;

    //Actual Rates
    private IntegerField _RollCenterSensitivy;
    private IntegerField _PitchCenterSensitivy;
    private IntegerField _YawCenterSensitivy;

    private IntegerField _RollMaxRate;
    private IntegerField _PitchMaxRate;
    private IntegerField _YawMaxRate;

    //RC Expo both systems
    private FloatField _RollRCExpo;
    private FloatField _PitchRCExpo;
    private FloatField _YawRCExpo;

    //Max Velocity in Deg per second
    private Label _RollMaxVel;
    private Label _PitchMaxVel;
    private Label _YawMaxVel;

    private RateCurveGraphElement _rateCurveGraph;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        _uiManager = GetComponent<UIDocument>();
        _FCMenuTab = _uiManager.rootVisualElement.Q<VisualElement>("FCMenuTab");

        _RateType = _FCMenuTab.Q<EnumField>("RateTypeSelector");
        _RateType.Init(RateType.Betaflight);

        _RCRateCol = _FCMenuTab.Q<VisualElement>("RCRate");
        _RateCol = _FCMenuTab.Q<VisualElement>("Rate");
        _CenterSensitivityCol = _FCMenuTab.Q<VisualElement>("CenterSensitivity");
        _MaxRateCol = _FCMenuTab.Q<VisualElement>("MaxRate");

        //Betaflight Rates
        _RollRCRate = _FCMenuTab.Q<FloatField>("RollRCRate");
        _PitchRCRate = _FCMenuTab.Q<FloatField>("PitchRCRate");
        _YawRCRate = _FCMenuTab.Q<FloatField>("YawRCRate");

        _RollRate = _FCMenuTab.Q<FloatField>("RollRate");
        _PitchRate = _FCMenuTab.Q<FloatField>("PitchRate");
        _YawRate = _FCMenuTab.Q<FloatField>("YawRate");

        //Actual Rates
        _RollCenterSensitivy = _FCMenuTab.Q<IntegerField>("RollCenterSensitivity");
        _PitchCenterSensitivy = _FCMenuTab.Q<IntegerField>("PitchCenterSensitivity");
        _YawCenterSensitivy = _FCMenuTab.Q<IntegerField>("YawCenterSensitivity");

        _RollMaxRate = _FCMenuTab.Q<IntegerField>("RollMaxRate");
        _PitchMaxRate = _FCMenuTab.Q<IntegerField>("PitchMaxRate");
        _YawMaxRate = _FCMenuTab.Q<IntegerField>("YawMaxRate");

        //RC Expo both systems
        _RollRCExpo = _FCMenuTab.Q<FloatField>("RollExpo");
        _PitchRCExpo = _FCMenuTab.Q<FloatField>("PitchExpo");
        _YawRCExpo = _FCMenuTab.Q<FloatField>("YawExpo");

        //Max Velocity
        _RollMaxVel = _FCMenuTab.Q<Label>("RollMaxVel");
        _PitchMaxVel = _FCMenuTab.Q<Label>("PitchMaxVel");
        _YawMaxVel = _FCMenuTab.Q<Label>("YawMaxVel");

        VisualElement rateGraphPlaceholder = _FCMenuTab
            .Q<VisualElement>("RatesCurve")
            .Q<VisualElement>("Graph");

        if (rateGraphPlaceholder != null)
        {
            rateGraphPlaceholder.Clear();
            _rateCurveGraph = new RateCurveGraphElement(_flightController);
            rateGraphPlaceholder.Add(_rateCurveGraph);
        }

        //Event Handlers
        _RateType.RegisterValueChangedCallback(OnRateTypeChanged);

        _RollRCRate.RegisterValueChangedCallback(OnRollRCRateChanged);
        _PitchRCRate.RegisterValueChangedCallback(OnPitchRCRateChanged);
        _YawRCRate.RegisterValueChangedCallback(OnYawRCRateChanged);

        _RollRate.RegisterValueChangedCallback(OnRollRateChanged);
        _PitchRate.RegisterValueChangedCallback(OnPitchRateChanged);
        _YawRate.RegisterValueChangedCallback(OnYawRateChanged);

        //RCRate and Center Sensitivity use the same event handlers because they use the same variable in the FC
        _RollCenterSensitivy.RegisterValueChangedCallback(OnRollRCRateChanged);
        _PitchCenterSensitivy.RegisterValueChangedCallback(OnPitchRCRateChanged);
        _YawCenterSensitivy.RegisterValueChangedCallback(OnYawRCRateChanged);

        //Rate and Max Rate use the same event handlers because they use the same variable in the FC
        _RollMaxRate.RegisterValueChangedCallback(OnRollRateChanged);
        _PitchMaxRate.RegisterValueChangedCallback(OnPitchRateChanged);
        _YawMaxRate.RegisterValueChangedCallback(OnYawRateChanged);

        _RollRCExpo.RegisterValueChangedCallback(OnRollRCExpoChanged);
        _PitchRCExpo.RegisterValueChangedCallback(OnPitchRCExpoChanged);
        _YawRCExpo.RegisterValueChangedCallback(OnYawRCExpoChanged);

        if ((RateType)_RateType.value == RateType.Betaflight)
        {
            _RollRCRate.Set(_flightController.RCRates[2]);
            _PitchRCRate.Set(_flightController.RCRates[0]);
            _YawRCRate.Set(_flightController.RCRates[1]);

            _RollRate.Set(_flightController.Rates[2]);
            _PitchRate.Set(_flightController.Rates[0]);
            _YawRate.Set(_flightController.Rates[1]);
        }
        else
        {
            // RCRate and Center Sensitivity use the same variable in the FC
            _RollCenterSensitivy.Set((int)_flightController.RCRates[2]);
            _PitchCenterSensitivy.Set((int)_flightController.RCRates[0]);
            _YawCenterSensitivy.Set((int)_flightController.RCRates[1]);

            // Rate and Max Rate use the same variable in the FC
            _RollMaxRate.Set((int)_flightController.Rates[2]);
            _PitchMaxRate.Set((int)_flightController.Rates[0]);
            _YawMaxRate.Set((int)_flightController.Rates[1]);
        }

        _RollRCExpo.Set(_flightController.RCExpo[2]);
        _PitchRCExpo.Set(_flightController.RCExpo[0]);
        _YawRCExpo.Set(_flightController.RCExpo[1]);

        UpdateMaxVelocityLabels();
        _rateCurveGraph?.MarkDirtyRepaint();
    }

    private void OnDisable()
    {
        _RateType.UnregisterValueChangedCallback(OnRateTypeChanged);

        _RollRCRate.UnregisterValueChangedCallback();
        _PitchRCRate.UnregisterValueChangedCallback();
        _YawRCRate.UnregisterValueChangedCallback();

        _RollRate.UnregisterValueChangedCallback();
        _PitchRate.UnregisterValueChangedCallback();
        _YawRate.UnregisterValueChangedCallback();

        _RollCenterSensitivy.UnregisterValueChangedCallback();
        _PitchCenterSensitivy.UnregisterValueChangedCallback();
        _YawCenterSensitivy.UnregisterValueChangedCallback();

        _RollMaxRate.UnregisterValueChangedCallback();
        _PitchMaxRate.UnregisterValueChangedCallback();
        _YawMaxRate.UnregisterValueChangedCallback();

        _RollRCExpo.UnregisterValueChangedCallback();
        _PitchRCExpo.UnregisterValueChangedCallback();
        _YawRCExpo.UnregisterValueChangedCallback();
    }

    private void OnRateTypeChanged(ChangeEvent<System.Enum> evt)
    {
        Debug.Log("Rate Type changed to: " + evt.newValue);
        _flightController.SetRateType((RateType)_RateType.value);
        //TODO When the rates are persistent there has to be a refresh in the valeus whenever the UI changes
        if((RateType)_RateType.value == RateType.Betaflight)
        {
            _RCRateCol.style.display = DisplayStyle.Flex;
            _RateCol.style.display = DisplayStyle.Flex;
            _CenterSensitivityCol.style.display = DisplayStyle.None;
            _MaxRateCol.style.display = DisplayStyle.None;
        }
        else
        {
            _RCRateCol.style.display = DisplayStyle.None;
            _RateCol.style.display = DisplayStyle.None;
            _CenterSensitivityCol.style.display = DisplayStyle.Flex;
            _MaxRateCol.style.display = DisplayStyle.Flex;
        }
    }

    private void OnRollRCRateChanged(ChangeEvent<string> evt)
    {
        Debug.Log("Roll RC Rate changed to: " + evt.newValue);
        _flightController.SetRCRate(2, float.Parse(evt.newValue));
        RefreshRatesUi();
    }

    private void OnPitchRCRateChanged(ChangeEvent<string> evt)
    {
        Debug.Log("Pitch RC Rate changed to: " + evt.newValue);
        _flightController.SetRCRate(0, float.Parse(evt.newValue));
        RefreshRatesUi();
    }

    private void OnYawRCRateChanged(ChangeEvent<string> evt)
    {
        Debug.Log("Yaw RC Rate changed to: " + evt.newValue);
        _flightController.SetRCRate(1, float.Parse(evt.newValue));
        RefreshRatesUi();
    }

    private void OnRollRateChanged(ChangeEvent<string> evt)
    {
        Debug.Log("Roll Rate changed to: " + evt.newValue);
        _flightController.SetRate(2, float.Parse(evt.newValue));
        RefreshRatesUi();
    }

    private void OnPitchRateChanged(ChangeEvent<string> evt)
    {
        Debug.Log("Pitch Rate changed to: " + evt.newValue);
        _flightController.SetRate(0, float.Parse(evt.newValue));
        RefreshRatesUi();
    }

    private void OnYawRateChanged(ChangeEvent<string> evt)
    {
        Debug.Log("Yaw Rate changed to: " + evt.newValue);
        _flightController.SetRate(1, float.Parse(evt.newValue));
        RefreshRatesUi();
    }

    private void OnRollRCExpoChanged(ChangeEvent<string> evt)
    {
        Debug.Log("Roll RC Expo changed to: " + evt.newValue);
        _flightController.SetExpo(2, float.Parse(evt.newValue));
        RefreshRatesUi();
    }

    private void OnPitchRCExpoChanged(ChangeEvent<string> evt)
    {
        Debug.Log("Pitch RC Expo changed to: " + evt.newValue);
        _flightController.SetExpo(0, float.Parse(evt.newValue));
        RefreshRatesUi();
    }

    private void OnYawRCExpoChanged(ChangeEvent<string> evt)
    {
        Debug.Log("Yaw RC Expo changed to: " + evt.newValue);
        _flightController.SetExpo(1, float.Parse(evt.newValue));
        RefreshRatesUi();
    }

    private void RefreshRatesUi()
    {
        UpdateMaxVelocityLabels();
        _rateCurveGraph?.MarkDirtyRepaint();
    }

    private void UpdateMaxVelocityLabels()
    {
        _RollMaxVel.text = _flightController.ComputeRate(2, 1f).ToString("0.##");
        _PitchMaxVel.text = _flightController.ComputeRate(0, 1f).ToString("0.##");
        _YawMaxVel.text = _flightController.ComputeRate(1, 1f).ToString("0.##");
    }
}

//TODO get this out of here
public class RateCurveGraphElement : VisualElement
{
    private readonly FlightController _flightController;

    private static readonly Color BackgroundColor = new Color(0.03f, 0.05f, 0.07f, 1f);
    private static readonly Color GridColor = new Color(0.95f, 0.95f, 0.95f, 0.08f);
    private static readonly Color AxisColor = new Color(0.95f, 0.95f, 0.95f, 0.2f);
    private static readonly Color[] CurveColors =
    {
        new Color(0.30f, 0.67f, 0.21f, 1f),
        new Color(0.29f, 0.62f, 0.83f, 1f),
        new Color(0.83f, 0.64f, 0.29f, 1f)
    };

    public RateCurveGraphElement(FlightController flightController)
    {
        _flightController = flightController;
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

        if (_flightController == null || _flightController.ComputeRate == null)
        {
            return;
        }

        float maxRate = GetGraphMaxRate();
        DrawAxes(painter, rect);
        DrawCurveForAxis(painter, rect, 2, maxRate, CurveColors[0]);
        DrawCurveForAxis(painter, rect, 0, maxRate, CurveColors[1]);
        DrawCurveForAxis(painter, rect, 1, maxRate, CurveColors[2]);
    }

    private float GetGraphMaxRate()
    {
        float maxRate = 1f;

        for (int axis = 0; axis < 3; axis++)
        {
            for (int i = 0; i <= 100; i++)
            {
                float input = Mathf.Lerp(-1f, 1f, i / 100f);
                float value = Mathf.Abs(_flightController.ComputeRate(axis, input));
                if (value > maxRate)
                {
                    maxRate = value;
                }
            }
        }

        return maxRate;
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
            float tx = i / 4f;
            float x = Mathf.Lerp(rect.xMin, rect.xMax, tx);
            painter.BeginPath();
            painter.MoveTo(new Vector2(x, rect.yMin));
            painter.LineTo(new Vector2(x, rect.yMax));
            painter.Stroke();

            float y = Mathf.Lerp(rect.yMin, rect.yMax, tx);
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

        float centerX = rect.xMin + rect.width * 0.5f;
        float centerY = rect.yMin + rect.height * 0.5f;

        painter.BeginPath();
        painter.MoveTo(new Vector2(rect.xMin, centerY));
        painter.LineTo(new Vector2(rect.xMax, centerY));
        painter.Stroke();

        painter.BeginPath();
        painter.MoveTo(new Vector2(centerX, rect.yMin));
        painter.LineTo(new Vector2(centerX, rect.yMax));
        painter.Stroke();
    }

    private void DrawCurveForAxis(Painter2D painter, Rect rect, int axis, float maxRate, Color color)
    {
        painter.strokeColor = color;
        painter.lineWidth = 2f;
        painter.BeginPath();

        const int sampleCount = 120;
        float halfHeight = rect.height * 0.5f * 0.9f;
        float centerY = rect.yMin + rect.height * 0.5f;

        for (int i = 0; i <= sampleCount; i++)
        {
            float t = i / (float)sampleCount;
            float input = Mathf.Lerp(-1f, 1f, t);
            float output = _flightController.ComputeRate(axis, input);
            float normalized = Mathf.Clamp(output / maxRate, -1f, 1f);

            float x = Mathf.Lerp(rect.xMin, rect.xMax, t);
            float y = centerY - normalized * halfHeight;

            if (i == 0)
            {
                painter.MoveTo(new Vector2(x, y));
            }
            else
            {
                painter.LineTo(new Vector2(x, y));
            }
        }

        painter.Stroke();
    }
}
