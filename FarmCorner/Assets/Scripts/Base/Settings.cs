using UnityEngine;

public class Settings : MonoBehaviour
{
    private enum Tutorial { None = 0, HorizontalSwerve = 1, JoystickSwerve = 2, Drag = 3 }
    [SerializeField] private Tutorial tutorial;

    [Range(60, 120)]
    [SerializeField] private int TFrame = 60;

    private void Start()
    {
        Application.targetFrameRate = TFrame;
    }

    public int GetTutorialIndex()
    {
        return (int)tutorial;
    }
}
