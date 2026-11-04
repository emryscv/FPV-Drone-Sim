using System;
using System.Collections;

public sealed class CalibrationStep
{
    public Func<IEnumerator> RoutineFactory { get; }
    public CalibrationStep Previous { get; internal set; }
    public CalibrationStep Next { get; internal set; }
    public OnUndo OnUndo { get; }

    public CalibrationStep(Func<IEnumerator> routineFactory, OnUndo onUndo = null)
    {
        RoutineFactory = routineFactory;
        OnUndo = onUndo;
    }
}
