using UnityEngine;
using System.Collections.Generic;

public class StartUIManager : MonoBehaviour
{
    ///////////////////////////////////////////
    // Fields
    private GameObject startUI;
    private GameObject optionsUI;
    private GameObject chooseFileUI;
    private GameObject chooseCharacterUI;

    private bool isLoadedOptionsSet;

    private bool isLoadPressed;

    // const
    private const string CANVAS = "Canvas";
    private const string START_UI_PATH = CANVAS + "/StartUI";
    private const string OPTIONS_UI_PATH = CANVAS + "/OptionsUI";
    private const string CHOOSE_FILE_UI_PATH = CANVAS + "/ChooseFileUI";
    private const string CHOOSE_CHARACTER_UI_PATH = CANVAS + "/ChooseCharacterUI";

    ///////////////////////////////////////////
    // Methods

    private void Awake()
    {
        startUI = transform.Find(START_UI_PATH).gameObject;
        optionsUI = transform.Find(OPTIONS_UI_PATH).gameObject;
        chooseFileUI = transform.Find(CHOOSE_FILE_UI_PATH).gameObject;
        chooseCharacterUI = transform.Find(CHOOSE_CHARACTER_UI_PATH).gameObject;

        startUI.SetActive(true);
        optionsUI.SetActive(false);
        chooseFileUI.SetActive(false);
        chooseCharacterUI.SetActive(false);

        StartUIController startUIScript = startUI.GetComponent<StartUIController>();
        startUIScript.OnStartPressed += OnStartPressed;
        startUIScript.OnLoadPressed += OnLoadPressed;
        startUIScript.OnOptionsPressed += OnOptionsPressed;
        startUIScript.OnQuitPressed += OnQuitPressed;

        if (!System.IO.Directory.Exists(Application.dataPath + Definitions.SAVE_FOLDER_NAME))
        {
            startUIScript.DisableLoadButton();
        }

        ChooseFileUIController chooseFileUIScript = chooseFileUI.GetComponent<ChooseFileUIController>();
        chooseFileUIScript.OnChooseFileBackPressed += OnChooseFileBackPressed;
    }

    private void OnStartPressed()
    {
        ChooseCharacterUIController chooseCharacterUIScript = chooseCharacterUI.GetComponent<ChooseCharacterUIController>();
        chooseCharacterUIScript.OnChooseCharacterConfirmPressed += OnChooseCharacterConfirmPressed;
        chooseCharacterUIScript.OnChooseCharacterBackPressed += OnChooseCharacterBackPressed;
        
        startUI.SetActive(false);
        chooseCharacterUI.SetActive(true);
    }

    private void OnLoadPressed()
    {
        startUI.SetActive(false);
        chooseFileUI.SetActive(true);
        if (!isLoadPressed)
        {
            chooseFileUI.GetComponent<ChooseFileUIController>().SetAvailableSaveFilesData(GetAvailableSaveFileNames());
            isLoadPressed = true;
        }
    }

    private void OnOptionsPressed()
    {
        if (!isLoadedOptionsSet && StartMenuGameState.IsOptionsLoaded)
        {
            optionsUI.GetComponent<OptionsUIController>().SetLoadedOptions();
            isLoadedOptionsSet = true;
        }
        optionsUI.GetComponent<OptionsUIController>().OnOptionsOkPressed += OnOptionsOkPressed;

        startUI.SetActive(false);
        optionsUI.SetActive(true);
    }

    private void OnQuitPressed()
    {
        Application.Quit();
    }

    private void OnOptionsOkPressed()
    {
        optionsUI.SetActive(false);
        startUI.SetActive(true);
    }

    private void OnChooseCharacterConfirmPressed()
    {
        chooseCharacterUI.SetActive(false);
        chooseFileUI.GetComponent<ChooseFileUIController>().RemoveLoadData();
        chooseFileUI.SetActive(true);
        isLoadPressed = false;
    }

    private void OnChooseCharacterBackPressed()
    {
        chooseCharacterUI.SetActive(false);
        startUI.SetActive(true);
    }

    private void OnChooseFileBackPressed()
    {
        if (isLoadPressed)
        {
            startUI.SetActive(true);
            chooseFileUI.SetActive(false);
        }
        else
        {
            chooseFileUI.SetActive(false);
            chooseCharacterUI.SetActive(true);
        }
    }

    private List<string> GetAvailableSaveFileNames()
    {
        List<string> fileNames = new List<string>();
        string[] files = System.IO.Directory.GetFiles(Application.dataPath + Definitions.SAVE_FOLDER_NAME);
        foreach (string file in files)
        {
            if (file.Contains("settings"))
            {
                if (!file.Contains(".meta")) fileNames.Add(System.IO.Path.GetFileName(file));
            }
        }

        return fileNames;
    }
}
