using UnityEngine;
using System.IO; // 파일 입출력을 위해 꼭 필요

public static class SaveLoadService
{
    private const string Army_SAVE_FILE = "PlayerArmy.json";

    private static string GetSavePath()
    {
        // Application.persistentDataPath는 PC, Mac, 모바일에서
        // 모두 안전하게 파일을 쓰고 읽을 수 있는 경로를 반환합니다.
        // (Unity Editor에서는 Assets 폴더 바깥의 별도 경로에 저장됩니다)
        return Path.Combine(Application.persistentDataPath, Army_SAVE_FILE);
    }

    public static void SaveArmy(PlayerArmy armyData)
    {
        string savePath = GetSavePath();

        // 1. 클래스 객체를 JSON 문자열로 변환 (true는 예쁘게 포매팅)
        string json = JsonUtility.ToJson(armyData, true);

        // 2. 파일 쓰기
        File.WriteAllText(savePath, json);

        Debug.Log($"Army saved to: {savePath}");
    }

    public static PlayerArmy LoadArmy()
    {
        string savePath = GetSavePath();

        // 3. 저장된 파일이 있는지 확인
        if (File.Exists(savePath))
        {
            // 4. 파일 읽기
            string json = File.ReadAllText(savePath);

            // 5. JSON 문자열을 다시 클래스 객체로 변환
            PlayerArmy loadedArmy = JsonUtility.FromJson<PlayerArmy>(json);

            Debug.Log("Army loaded.");
            return loadedArmy;
        }
        else
        {
            // 6. 저장된 파일이 없으면, 새 기본값 반환
            Debug.Log("No save file found. Creating new troop.");
            return new PlayerArmy();
        }
    }
}