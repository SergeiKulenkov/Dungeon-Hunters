using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class ChooseFileUIController : MonoBehaviour
{
    ///////////////////////////////////////////
    // Fields
    public string SaveFileName { get; private set; }
    public string LastSavedScene { get; private set; }

    public Action OnChooseFileConfirmPressed;
    public Action OnChooseFileBackPressed;

    // const
    private const string SAVE_FILE_BUTTONS_PATH = "SaveFileButtons";
    private const string SAVE_FILE_TEXT_PATH = "SaveFileText";
    private const string SAVE_FILE_TEXT = "SAVE ";
    private const string COINS_OBJECT_PATH = "CoinsObject";
    private const string HEARTS_OBJECT_PATH = "HeartsObject";

    ///////////////////////////////////////////
    // Methods

    private void Awake()
    {
        InitializeSaveButtons();
    }

    private void InitializeSaveButtons()
    {
        int saveFileTextIndex = 1;
        foreach(Transform saveButton in transform.Find(SAVE_FILE_BUTTONS_PATH))
        {
            string saveFileName = SAVE_FILE_TEXT + saveFileTextIndex.ToString();
            saveButton.GetComponent<Button>().onClick.AddListener(() => SetSaveFileName(saveFileName));

            saveButton.Find(SAVE_FILE_TEXT_PATH).GetComponent<TextMeshProUGUI>().text = saveFileName;
            saveFileTextIndex++;
        }
    }

    private void SetSaveFileName(string newSaveFileName)
    {
        SaveFileName = newSaveFileName.ToLower();
        SaveFileName = SaveFileName.Replace(" ", "");
    }

    public void OnConfirmButtonPressed()
    {
        if (!string.IsNullOrEmpty(SaveFileName)) OnChooseFileConfirmPressed?.Invoke();
    }
    
    public void OnBackButtonPressed() => OnChooseFileBackPressed?.Invoke();

    public void SetAvailableSaveFilesData(List<string> fileNames)
    {
        List<int> fileNumbers = new List<int>();
        List<PlayerSettings> saves = new List<PlayerSettings>();

        foreach (string name in fileNames)
        {
            fileNumbers.Add((int)System.Char.GetNumericValue(name[name.IndexOf(".") - 1]));
            saves.Add(SaveLoad.LoadSettings(name));
        }

        Transform saveFileButtons = transform.Find(SAVE_FILE_BUTTONS_PATH);
        Button button;
        Transform coinsObject;
        Transform heartsObject;
        int buttonIndex = 1;
        bool buttonIndexEqualsFileIndex = false;
        string saveData = "";
        foreach (Transform saveFileButton in saveFileButtons)
        {
            buttonIndexEqualsFileIndex = false;
            button = saveFileButton.GetComponent<Button>();

            for( int i = 0; i < fileNumbers.Count; i++)
            {
                if (buttonIndex == fileNumbers[i])
                {
                    saveData = saves[i].CharacterName + "\n";
                    if (saves[i].LastSavedScene.Contains(Definitions.FINAL_LEVEL)) saveData += "The " + Definitions.FINAL + " " + Definitions.LEVEL;
                    else
                    {
                        if (saves[i].LastSavedScene.Contains(Definitions.FIRST_LOCATION)) saveData += " Loc1";
                        else if (saves[i].LastSavedScene.Contains(Definitions.SECOND_LOCATION)) saveData += " Loc2";

                        saveData += " " + saves[i].LastSavedLevelName;
                    }

                    saveFileButton.Find(SAVE_FILE_TEXT_PATH).GetComponent<TextMeshProUGUI>().text = saveData;
                    coinsObject = saveFileButton.Find(COINS_OBJECT_PATH);
                    coinsObject.GetComponentInChildren<TextMeshProUGUI>().text = saves[i].LastSavedCoins.ToString();
                    coinsObject.gameObject.SetActive(true);
                    heartsObject = saveFileButton.Find(HEARTS_OBJECT_PATH);
                    heartsObject.GetComponentInChildren<TextMeshProUGUI>().text = ((float)saves[i].LastSavedHealth / Definitions.ONE_HEALTH_POINT).ToString();
                    heartsObject.gameObject.SetActive(true);

                    button.onClick.AddListener(() => SetLastSavedSceneName(saves[i].LastSavedScene));
                    buttonIndexEqualsFileIndex = true;
                    break;
                }
            }

            if (!buttonIndexEqualsFileIndex)
            {
                button.interactable = false;
            }
            

            buttonIndex++;
        }
    }

    public void RemoveLoadData()
    {
        Transform saveFileButtons = transform.Find(SAVE_FILE_BUTTONS_PATH);
        Button button;

        foreach (Transform saveFileButton in saveFileButtons)
        {
            button = saveFileButton.GetComponent<Button>();
            
            saveFileButton.Find(SAVE_FILE_TEXT_PATH).GetComponent<TextMeshProUGUI>().text = "";
            saveFileButton.Find(COINS_OBJECT_PATH).gameObject.SetActive(false);
            saveFileButton.Find(HEARTS_OBJECT_PATH).gameObject.SetActive(false);
            button.onClick.RemoveAllListeners();
            button.interactable = true;
        }

        InitializeSaveButtons();
    }

    private void SetLastSavedSceneName(string lastSavedScene)
    {
        LastSavedScene = lastSavedScene;
    }
}
