using UnityEngine;

public static class CutsceneSettings
{
    public static Sprite[] oynatilacakGorseller;

    // Yeni: Her resim için bir string dizisi. 
    // Örn: metinGruplari[0] -> Ýlk resmin 2-3 cümlelik metinleri.
    public static string[][] metinGruplari;

    public static string sonrakiSahne;
    public static string mevcutAraSahneID;
}