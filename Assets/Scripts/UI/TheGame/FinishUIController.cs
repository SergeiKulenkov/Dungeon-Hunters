using UnityEngine;
using System;

public class FinishUIController : MonoBehaviour
{
    ///////////////////////////////////////////
    // Fields
    [SerializeField] private string diedText;
    private bool isLevelRestart;

    // event
    public static event Action<bool> OnRestartPressed;
    public static event Action OnExitToMainMenuPressed;
    public static event Action OnQuitPressed;

    // const
    private const string FINISH_TEXT = "FinishText";

    ///////////////////////////////////////////
    // Methods

    public void SetTextToDied(string restartText, bool isLevelRestart)
    {
        TMPro.TextMeshProUGUI finishText = transform.Find(FINISH_TEXT).GetComponent<TMPro.TextMeshProUGUI>();
        finishText.text = diedText;
        finishText.color = Color.red;

        this.isLevelRestart = isLevelRestart;
        if (isLevelRestart)
        {
            RectTransform restartButton = transform.Find(Definitions.RESTART_BUTTON).GetComponent<RectTransform>();
            restartButton.sizeDelta = new Vector2(Definitions.WIDTH_FOR_RESTART_FROM_SAVE_TEXT, restartButton.sizeDelta.y);
            TMPro.TextMeshProUGUI restartTextObject = transform.Find(Definitions.RESTART_TEXT).GetComponent<TMPro.TextMeshProUGUI>();
            restartTextObject.text = restartText;
        }
    }

    public void OnRestartButtonPressed() => OnRestartPressed?.Invoke(isLevelRestart);
    public void OnExitToMainMenuButtonPressed() => OnExitToMainMenuPressed?.Invoke();
    public void OnQuitButtonPressed() => OnQuitPressed?.Invoke();
}
