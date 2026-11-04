
using UnityEngine.UIElements;

public static class Helpers
{
    public static void SetButtonState(Button button, bool enabled)
    {
        button.SetEnabled(enabled);
        if (enabled)
            button.RemoveFromClassList("disabled");
        else
            button.AddToClassList("disabled");
    }
}