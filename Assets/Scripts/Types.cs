public enum RateType
{
    Betaflight,
    Actual
}

public delegate float RateEquation(int axis, float input);