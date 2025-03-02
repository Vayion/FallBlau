using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class GameSaves : MonoBehaviour
{
    private static string saveFilePath;

    void Start()
    {
        saveFilePath = Application.persistentDataPath + "/savegame.json";
        Debug.Log("Save Path: " + saveFilePath);
    }


    public static void SaveGame()
    {
        List<DivisionData> divisionSaveData = new List<DivisionData>();
        List<Division> divisions = TheGameManager.GetDivisions();

        for (int i = 0; i < divisions.Count; i++)
        {
            divisionSaveData.Add(TheGameManager.CreateDivisionData(divisions[i]));
        }

        SaveData saveData = new SaveData()
        {
            divisions = divisionSaveData.ToArray()
        };

        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(saveFilePath, json);
    }

    public static void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

            TheGameManager.ClearDivisions();

            for (int i = 0; i < saveData.divisions.Length; i++)
            {
                TheGameManager.LoadDivisionButDontSendUpdate(saveData.divisions[i]);
            }

            TheGameManager.instance.SendAllDivisionData();
        }
        else
        {
            Debug.Log("No save file found!");
        }
    }
}

[System.Serializable]
public class SaveData
{
    public DivisionData[] divisions;
}
