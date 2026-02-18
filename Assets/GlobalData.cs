using System.Collections.Generic;
using UnityEngine;

public static class GlobalData
{
    // --- 1. OYUNCU KONUMU ---
    // Hangi kapıdan çıktık? (Koridor sahnesi açılınca buraya bakacak)
    public static string sonCikisKapisi = ""; 

    // --- 2. ENVANTER VE DURUMLAR ---
    // Burası çok esnek. "Bant_Alindi", "Kablo_Kesildi", "Cekmece_Acik" her şeyi buraya atacağız.
    // String (Anahtar) -> Bool (Değer)
    public static Dictionary<string, bool> oyunDurumlari = new Dictionary<string, bool>();

    // --- YARDIMCI FONKSİYONLAR ---

    // Bir şeyin durumunu kaydet (Örn: "Bant", true)
    public static void DurumKaydet(string anahtar, bool durum)
    {
        if (oyunDurumlari.ContainsKey(anahtar))
        {
            oyunDurumlari[anahtar] = durum;
        }
        else
        {
            oyunDurumlari.Add(anahtar, durum);
        }
        Debug.Log("KAYIT: " + anahtar + " = " + durum);
    }

    // Bir şeyin durumunu öğren (Örn: "Bant" alındı mı?)
    public static bool DurumNedir(string anahtar)
    {
        if (oyunDurumlari.ContainsKey(anahtar))
        {
            return oyunDurumlari[anahtar];
        }
        return false; // Kayıt yoksa varsayılan olarak yapılmamış (false) sayarız.
    }
}