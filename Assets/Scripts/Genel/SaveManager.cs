using UnityEngine;
using System.IO;

public static class SaveManager
{
    private static string savePath = Application.persistentDataPath + "/saveData.json";

    public static void Kaydet(bool durum)
    {
        SaveData data = new SaveData();
        data.oda1Temizlendi = durum;

        string json = JsonUtility.ToJson(data, true); // Veriyi metne çevirir
        File.WriteAllText(savePath, json); // Metni dosyaya yazar
        Debug.Log("Oyun Kaydedildi: " + savePath);
    }

    public static bool Yukle()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            return data.oda1Temizlendi;
        }
        return false; // Dosya yoksa savaş henüz bitmemiştir
    }
}