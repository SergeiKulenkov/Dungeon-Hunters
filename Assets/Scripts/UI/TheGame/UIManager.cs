using UnityEngine;

public class UIManager : MonoBehaviour
{
    ///////////////////////////////////////////
    // Fields
    [SerializeField] private Vector2 interactTextOffset;
    [SerializeField] private string savedSuccessfullyText;
    [SerializeField] private string savingFailedText;
    [SerializeField] private string restartFromSaveText;
    private GameObject pauseUI;
    private GameObject HUD;
    private GameObject finishUI;
    private GameObject optionsUI;
    private GameObject crossfade;
    private GameObject pauseBackground;

    public static bool IsOptionsOpened { get; private set; } 

    // const
    private const string CANVAS = "Canvas";
    private const string BACKGROUND = CANVAS + "/Background";
    private const string INTERACT_PROMPTS = CANVAS + "/InteractPrompts";
    private const string PAUSE_UI_PATH = CANVAS + "/PauseUI";
    private const string HUD_PATH = CANVAS + "/HUD";
    private const string FINISH_UI_PATH = CANVAS + "/FinishUI";
    private const string OPTIONS_UI_PATH = CANVAS + "/OptionsUI";
    private const string CROSSFADE_PATH = CANVAS + "/Crossfade";
    
    private const string SAVE_TEXT_PATH = INTERACT_PROMPTS + "/SaveText";
    private const string OPEN_TEXT_PATH = INTERACT_PROMPTS + "/OpenText";
    private const string BUY_TEXT_PATH = INTERACT_PROMPTS + "/BuyText";

    private const string START_ANIM = "Start";

    ///////////////////////////////////////////
    // Methods

    private void Awake()
    {
        HUD = transform.Find(HUD_PATH).gameObject;
        pauseUI = transform.Find(PAUSE_UI_PATH).gameObject;
        finishUI = transform.Find(FINISH_UI_PATH).gameObject;
        optionsUI = transform.Find(OPTIONS_UI_PATH).gameObject;
        crossfade = transform.Find(CROSSFADE_PATH).gameObject;
        pauseBackground = transform.Find(BACKGROUND).gameObject;

        HUD.SetActive(true);
        pauseUI.SetActive(false);
        finishUI.SetActive(false);
        optionsUI.SetActive(false);
        crossfade.SetActive(true);
        pauseBackground.SetActive(false);
    }

    private void OnEnable()
    {
        PauseUIController.OnResumePressed += OnResumePressed;
        PauseUIController.OnOptionsPressed += OnOptionsPressed;

        InputManager.OnPausePressed += OnPausePressed;
        InputManager.OnResumePressed += OnResumePressed;
        InputManager.OnInteractPressed += OnInteractPressed;

        Player.OnPlayerDied += OnPlayerDied;
        BonfireController.OnSaved += OnSaved;
        InteractableItemController.OnNearInteractableItem += SetActiveItemText;

        optionsUI.GetComponent<OptionsUIController>().OnOptionsOkPressed += OnOptionsOkPressed;
    }

    private void OnDestroy()
    {
        PauseUIController.OnResumePressed -= OnResumePressed;
        PauseUIController.OnOptionsPressed -= OnOptionsPressed;

        InputManager.OnPausePressed -= OnPausePressed;
        InputManager.OnResumePressed -= OnResumePressed;
        InputManager.OnInteractPressed -= OnInteractPressed;

        Player.OnPlayerDied -= OnPlayerDied;
        BonfireController.OnSaved -= OnSaved;
        InteractableItemController.OnNearInteractableItem -= SetActiveItemText;
    }

    private void OnPausePressed()
    {
        Time.timeScale = 0;
        pauseBackground.SetActive(true);
        pauseUI.SetActive(true);
        pauseUI.GetComponent<PauseUIController>().SetLevelName(GameState.CurrentLevelName);
        if (SaveLoad.IsLevelSaved())
        {
            pauseUI.GetComponent<PauseUIController>().ChangeRestartText(restartFromSaveText);
        }
    }

    private void OnResumePressed()
    {
        Time.timeScale = 1f;
        pauseBackground.SetActive(false);
        pauseUI.SetActive(false);
    }

    private void OnOptionsPressed()
    {
        pauseUI.SetActive(false);
        optionsUI.SetActive(true);
        IsOptionsOpened = true;
    }

    private void OnOptionsOkPressed()
    {
        optionsUI.SetActive(false);
        pauseUI.SetActive(true);
        IsOptionsOpened = false;
    }

    private void OnChestOpened()
    {
        transform.Find(INTERACT_PROMPTS).gameObject.SetActive(false);
        Time.timeScale = 0;
        finishUI.SetActive(true);
    }

    private void OnPlayerDied()
    {
        Time.timeScale = 0;
        finishUI.SetActive(true);
        finishUI.GetComponent<FinishUIController>().SetTextToDied(restartFromSaveText, SaveLoad.IsLevelSaved());
    }

    public void MakeSceneTransition()
    {
        crossfade.gameObject.GetComponent<Animator>().SetTrigger(START_ANIM);
    }

    private void SetActiveItemText(bool isActive, Transform item)
    {
        Transform textObject = null;
        switch ((Definitions.InteractableItems) item.GetComponent<InteractableItemController>().itemType)
        {
            case Definitions.InteractableItems.Save:
                textObject = transform.Find(SAVE_TEXT_PATH);
                if (textObject != null) textObject.gameObject.SetActive(isActive);
                break;
            case Definitions.InteractableItems.Open:
                textObject = transform.Find(OPEN_TEXT_PATH);
                if (textObject != null) textObject.gameObject.SetActive(isActive);
                break;
            case Definitions.InteractableItems.Buy:
                textObject = transform.Find(BUY_TEXT_PATH);
                if (textObject != null) textObject.gameObject.SetActive(isActive);
                break;
        }

        if (textObject != null)
        {
            Vector2 viewportPosition = Camera.main.WorldToViewportPoint(item.position);
            RectTransform canvas = transform.Find(CANVAS).GetComponent<RectTransform>();
            Vector2 itemScreenPosition = new Vector2(((viewportPosition.x * canvas.sizeDelta.x) - (canvas.sizeDelta.x * 0.5f)),
                                                    ((viewportPosition.y * canvas.sizeDelta.y) - (canvas.sizeDelta.y * 0.5f)));
            textObject.GetComponent<RectTransform>().anchoredPosition = itemScreenPosition + interactTextOffset;
        }
    }

    private void OnSaved(bool isSaved)
    {
        Transform saveTextObject = transform.Find(SAVE_TEXT_PATH);
        if (isSaved) saveTextObject.GetComponent<TMPro.TextMeshProUGUI>().text = savedSuccessfullyText;
        else saveTextObject.GetComponent<TMPro.TextMeshProUGUI>().text = savingFailedText;
        Destroy(saveTextObject.gameObject, 2f);
    }

    private void OnInteractPressed()
    {
        ChestController chest = GameObject.FindObjectOfType<ChestController>();
        if (chest != null) chest.OnChestOpened += OnChestOpened;
    }
}
