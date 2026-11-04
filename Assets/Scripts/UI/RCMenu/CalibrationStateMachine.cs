public sealed class CalibrationStateMachine
{
    public CalibrationStep Start { get; }
    public CalibrationStep Current { get; private set; }

    public CalibrationStateMachine(CalibrationStep start)
    {
        Start = start;
        Current = start;
    }

    public void Reset()
    {
        Current = Start;
    }

    public bool MoveNext()
    {
        if (Current.Next == null)
        {
            return false;
        }

        Current = Current.Next;
        return true;
    }

    public bool MovePrevious()
    {
        if (Current.Previous == null)
        {
            return false;
        }

        Current = Current.Previous;
        return true;
    }

    public static void Link(CalibrationStep previous, CalibrationStep next)
    {
        previous.Next = next;
        next.Previous = previous;
    }
}