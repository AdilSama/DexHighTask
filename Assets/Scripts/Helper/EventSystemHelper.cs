using UnityEngine.EventSystems;

public class EventSystemHelper
{
    private static EventSystem _eventSystem;

    private static EventSystem EventSystem => _eventSystem ??= EventSystem.current;

    public static void EnableInput() => EventSystem.enabled = true;
    public static void DisableInput() => EventSystem.enabled = false;
}
