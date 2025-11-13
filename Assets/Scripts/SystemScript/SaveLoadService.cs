
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveLoadService
{
    private static string GetSaveDirectoryPath()
    {
        string path = Application.persistentDataPath;
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        return path;
    }

    private static string GetSavePath(string armyName)
    {
        return Path.Combine(GetSaveDirectoryPath(), armyName + ".json");
    }

    public static void SaveArmy(PlayerArmy armyToSave)
    {
        string jsonString = JsonUtility.ToJson(armyToSave, true);
        string filePath = GetSavePath(armyToSave.armyName);
        try
        {
            File.WriteAllText(filePath, jsonString);
            Debug.Log($"부대 저장 성공: {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"부대 저장 실패: {e.Message}");
        }
    }

    public static PlayerArmy LoadArmy(string armyName)
    {
        string filePath = GetSavePath(armyName);
        if (!File.Exists(filePath))
        {
            Debug.LogError($"저장 파일 없음: {filePath}");
            return null;
        }

        string jsonString = File.ReadAllText(filePath);
        PlayerArmy loadedArmy = JsonUtility.FromJson<PlayerArmy>(jsonString);
        return loadedArmy;
    }

    public static List<string> GetAllSavedArmyNames()
    {
        string saveDirectory = GetSaveDirectoryPath();
        List<string> armyNames = new List<string>();

        string[] saveFiles = Directory.GetFiles(saveDirectory, "*.json");

        foreach (string filePath in saveFiles)
        {
            string armyName = Path.GetFileNameWithoutExtension(filePath);
            armyNames.Add(armyName);
        }

        return armyNames;
    }

    public static void DeleteArmy(string armyName)
    {
        string filePath = GetSavePath(armyName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log($"부대 삭제: {armyName}");
        }
    }
}