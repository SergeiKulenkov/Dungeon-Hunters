using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.IO;

public static class SaveLoad
{
    ///////////////////////////////////////////
    // Fields
    private static string saveFolderPath;
    private static string settingsFileName;
    private static string saveFileName;
    
    public const string SAVE_FILE_EXTENSION = ".txt";
    public const string SETTINGS_FILE_NAME = "settings";
    public const string OPTIONS_FILE_NAME = "options";

    ///////////////////////////////////////////
    // Methods

    private static void InitializeSaveFolderPath()
    {
        if (string.IsNullOrEmpty(saveFolderPath))
        {
            saveFolderPath = Application.dataPath + Definitions.SAVE_FOLDER_NAME;
            if (!System.IO.Directory.Exists(saveFolderPath))
            {
                System.IO.Directory.CreateDirectory(saveFolderPath);
            }
        }
    }

    public static bool SaveRooms()
    {
        List<RoomData> rooms = GameObject.FindObjectOfType<MapManager>().GetRoomsData();
        return SaveRooms(rooms);
    }

    public static bool SaveRooms(List<RoomData> rooms)
    {
        StringBuilder content = new StringBuilder();
        string json = "";
        foreach (RoomData data in rooms)
        {
            json = JsonUtility.ToJson(data);
            content.AppendLine(json);
        }

        InitializeSaveFolderPath();
        File.WriteAllText(saveFolderPath + saveFileName, content.ToString());
        return IsSaved(saveFileName);
    }

    public static List<RoomData> LoadRooms()
    {
        List<RoomData> rooms = new List<RoomData>();
        InitializeSaveFolderPath();

        if (File.Exists(saveFolderPath + saveFileName))
        {
            IEnumerable<string> lines = File.ReadLines(saveFolderPath + saveFileName);
            foreach (string line in lines)
            {
                rooms.Add(JsonUtility.FromJson<RoomData>(line));
            }
        }

        return rooms;
    }

    public static void SaveSettings(PlayerSettings settings)
    {
        string json = JsonUtility.ToJson(settings);
        if (string.IsNullOrEmpty(settingsFileName) && string.IsNullOrEmpty(saveFileName))
        {
            settingsFileName = SETTINGS_FILE_NAME + saveFileName[saveFileName.IndexOf(".") - 1] + SAVE_FILE_EXTENSION;
        }

        InitializeSaveFolderPath();
        File.WriteAllText(saveFolderPath + settingsFileName, json);
    }

    public static PlayerSettings LoadSettings()
    {
        PlayerSettings settings = null;
        InitializeSaveFolderPath();
        if (string.IsNullOrEmpty(settingsFileName) && !string.IsNullOrEmpty(saveFileName))
        {
            settingsFileName = SETTINGS_FILE_NAME + saveFileName[saveFileName.IndexOf(".") - 1] + SAVE_FILE_EXTENSION;
        }

        if (File.Exists(saveFolderPath + settingsFileName))
        {
            string save = File.ReadAllText(saveFolderPath + settingsFileName);
            settings = JsonUtility.FromJson<PlayerSettings>(save);
        }

        return settings;
    }

    public static PlayerSettings LoadSettings(string fileName)
    {
        PlayerSettings settings = null;
        InitializeSaveFolderPath();

        if (File.Exists(saveFolderPath + fileName))
        {
            string save = File.ReadAllText(saveFolderPath + fileName);
            settings = JsonUtility.FromJson<PlayerSettings>(save);
        }

        return settings;
    }

    public static void SaveOptions(PlayerOptions options)
    {
        string json = JsonUtility.ToJson(options);
        string optionsFileName = OPTIONS_FILE_NAME + SAVE_FILE_EXTENSION;
        File.WriteAllText(Application.dataPath + "/" + optionsFileName, json);
    }

    public static PlayerOptions LoadOptions()
    {
        PlayerOptions options = null;
        string optionsFileName = OPTIONS_FILE_NAME + SAVE_FILE_EXTENSION;

        if (File.Exists(Application.dataPath + "/" + optionsFileName))
        {
            string save = File.ReadAllText(Application.dataPath + "/" + optionsFileName);
            options = JsonUtility.FromJson<PlayerOptions>(save);
        }

        return options;
    }

    private static bool IsSaved(string fileName) => File.Exists(saveFolderPath + fileName);
    public static bool IsLevelSaved() => IsSaved(SETTINGS_FILE_NAME + saveFileName[saveFileName.IndexOf(".") - 1] + SAVE_FILE_EXTENSION);
    public static void InitializeSaveFileName(string newSaveFileName) => saveFileName = newSaveFileName + SAVE_FILE_EXTENSION;

    public static void DeleteSavedData()
    {
        if (System.IO.Directory.GetFiles(saveFolderPath).Length == 0) System.IO.Directory.Delete(saveFolderPath);
    }

    public static void DeletePastSaveWithSameName()
    {
        if (File.Exists(saveFolderPath + saveFileName))
        {
            File.Delete(saveFolderPath + saveFileName);
            File.Delete(saveFolderPath + SETTINGS_FILE_NAME + saveFileName[saveFileName.IndexOf(".") - 1] + SAVE_FILE_EXTENSION);
        }
    }
}
