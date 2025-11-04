
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

    // [기존 함수] PlayerArmy 객체를 받아서 그대로 JSON으로 변환
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

    // [기존 함수] JSON 파일을 읽어서 그대로 PlayerArmy 객체로 변환
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

    // --- [새로 추가할 함수] ---
    /// <summary>
    /// 저장 폴더에 있는 모든 부대 파일(.json)의 이름 목록을 불러옵니다.
    /// </summary>
    /// <returns>부대 이름(파일 이름)의 List<string></returns>
    public static List<string> GetAllSavedArmyNames()
    {
        string saveDirectory = GetSaveDirectoryPath();
        List<string> armyNames = new List<string>();

        // 1. 저장 폴더에서 ".json" 확장자를 가진 모든 파일 검색
        // (참고: .json.meta 파일도 검색될 수 있으나, Path.GetFileNameWithoutExtension가 처리해 줍니다)
        string[] saveFiles = Directory.GetFiles(saveDirectory, "*.json");

        // 2. 파일 경로에서 파일 이름(확장자 제외)만 추출하여 리스트에 추가
        foreach (string filePath in saveFiles)
        {
            string armyName = Path.GetFileNameWithoutExtension(filePath);
            armyNames.Add(armyName);
        }

        return armyNames;
    }

    // (옵션) 부대 삭제 기능
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