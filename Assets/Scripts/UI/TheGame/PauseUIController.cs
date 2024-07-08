using UnityEngine;
using System;

public class PauseUIController : MonoBehaviour
{
    ///////////////////////////////////////////
    // Fields
    private TMPro.TextMeshProUGUI restartTextObject;
    private bool isLevelRestart;
    
    // events
    public static event Action OnResumePressed;
    public static event Action<bool> OnRestartPressed;
    public static event Action OnOptionsPressed;
    public static event Action OnQuitPressed;
    public static event Action OnExitToMainMenuPressed;

    // const
    private const string LEVEL_NAME = "LevelName";

    ///////////////////////////////////////////
    // Methods

    private void Awake()
    {
        restartTextObject = transform.Find(Definitions.RESTART_TEXT).GetComponent<TMPro.TextMeshProUGUI>();
    }

    public void SetLevelName(string name)
    {
        TMPro.TextMeshProUGUI levelName = transform.Find(LEVEL_NAME).GetComponent<TMPro.TextMeshProUGUI>();
        if (!Utilities.IsFinalLevel())
        {
            int length = name.Length;
            levelName.text = name.Remove(length - 1).PadRight(length + 1) + name.Remove(0, length - 1);
        }
        else levelName.text = "The " + Definitions.FINAL + " " + Definitions.LEVEL;
    }

    public void OnResumeButtonPressed() => OnResumePressed?.Invoke();
    public void OnRestartButtonPressed() => OnRestartPressed?.Invoke(isLevelRestart);
    public void OnOptionsButtonPressed() => OnOptionsPressed?.Invoke();
    public void OnExitToMainMenuButtonPressed() => OnExitToMainMenuPressed?.Invoke();
    public void OnQuitButtonPressed() => OnQuitPressed?.Invoke();

    public void ChangeRestartText(string restartText)
    {
        if (restartTextObject.text != restartText)
        {
            isLevelRestart = true;
            RectTransform restartButton = transform.Find(Definitions.RESTART_BUTTON).GetComponent<RectTransform>();
            restartButton.sizeDelta = new Vector2(Definitions.WIDTH_FOR_RESTART_FROM_SAVE_TEXT, restartButton.sizeDelta.y);
            restartTextObject.text = restartText;
        }
    }
}
