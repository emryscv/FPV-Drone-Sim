
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class Speedometer : VisualElement
{
    public Speedometer()
    {
        generateVisualContent += GenerateVisualContent;
    }

    private void GenerateVisualContent(MeshGenerationContext context)
    {
        float width = contentRect.width;
        float height = contentRect.height;

        var painter = context.painter2D;
        painter.BeginPath();
        painter.lineWidth = 10f;
        painter.Arc(new Vector2(width * 0.5f, height), width * 0.5f, 180f, 0f);
        painter.ClosePath();
        painter.fillColor = Color.white;
        painter.Fill(FillRule.NonZero);
        painter.Stroke(); 
    }
}