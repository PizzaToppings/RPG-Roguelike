using System.IO;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    // Singleton instance
    private static SaveData _instance;

    // Public property to access the instance
    public static SaveData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SaveData();
            }
            return _instance;
        }
    }

    public static List<SaveDataCharacter> Characters;


    // --------

    public static void SaveGame()
    {
        string json = JsonUtility.ToJson(Instance);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
        Debug.Log("Game Saved!");
    }

    public static void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/savefile.json"))
        {
            string json = File.ReadAllText(Application.persistentDataPath + "/savefile.json");
            _instance = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            Debug.Log("No save file found. Using default values.");
        }
    }
}
