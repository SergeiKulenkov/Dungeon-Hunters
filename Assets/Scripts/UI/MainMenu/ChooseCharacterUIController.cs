using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;

public class ChooseCharacterUIController : MonoBehaviour
{
    ///////////////////////////////////////////
    // Fields
    [SerializeField] private Color textBackgroundEnabledColor;
    [SerializeField] private Color textBackgroundDisabledColor;

    private List<Image> textBackgrounds = new List<Image>();
    
    public static string SelectedCharacter { get; private set; }

    public Action OnChooseCharacterConfirmPressed;
    public Action OnChooseCharacterBackPressed;

    // const
    private const string CHARACTERS = "Characters";
    private const string HEALTH = "Health";
    private const string SPEED = "Speed";
    private const string BUTTON = "Button";
    private const string TEXT_BACKGROUND = "TextBackground";

    ///////////////////////////////////////////
    // Methods

    private void Awake()
    {
        PlayerSO config = PlayerSO.CreateInstance<PlayerSO>();
        TextMeshProUGUI textObject;

        foreach (Transform character in transform.Find(CHARACTERS))
        {
            if (character.name.Contains(Definitions.SPACE_GUY))
            {
                config = ConfigManager.GetPlayerConfig(Definitions.SPACE_GUY);
                Image textBackground = character.Find(TEXT_BACKGROUND).GetComponent<Image>();
                character.Find(BUTTON).GetComponent<Button>().onClick.AddListener(() =>
                                                                SetCharacter(Definitions.SPACE_GUY, textBackground));
                textBackgrounds.Add(textBackground);
            }
            else if (character.name.Contains(Definitions.RANGER))
            {
                config = ConfigManager.GetPlayerConfig(Definitions.RANGER);
                Image textBackground = character.Find(TEXT_BACKGROUND).GetComponent<Image>();
                character.Find(BUTTON).GetComponent<Button>().onClick.AddListener(() =>
                                                                SetCharacter(Definitions.RANGER, textBackground));
                textBackgrounds.Add(textBackground);
            }

            textObject = character.Find(HEALTH).GetComponent<TextMeshProUGUI>();
            textObject.text += config.MaxHealth;
            textObject = character.Find(SPEED).GetComponent<TextMeshProUGUI>();
            textObject.text += config.Speed;
        }
    }

    private void SetCharacter(string character, Image backgroundToEnable)
    {
        SelectedCharacter = character;

        foreach (Image textBackground in textBackgrounds)
        {
            if (textBackground.color == textBackgroundEnabledColor)
            {
                textBackground.color = textBackgroundDisabledColor;
                break;
            }
        }
        backgroundToEnable.color = textBackgroundEnabledColor;
    }
    
    public void OnConfirmButtonPressed()
    {
        if (!string.IsNullOrEmpty(SelectedCharacter)) OnChooseCharacterConfirmPressed?.Invoke();
    }

    public void OnBackButtonPressed()
    {
        foreach (Image textBackground in textBackgrounds)
        {
            if (textBackground.color == textBackgroundEnabledColor) textBackground.color = textBackgroundDisabledColor;
        }

        OnChooseCharacterBackPressed?.Invoke();
    }
}
