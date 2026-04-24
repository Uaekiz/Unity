using UnityEngine;

// 'static' olduðu için bu scripti hiçbir objeye sürüklemene gerek yok. 
// Kodun içinden her yerden ulaþýlabilir olur.
public static class CutsceneSettings
{
    public static Sprite[] oynatilacakGorseller; // Gösterilecek 4 resim buraya gelecek
    public static string sonrakiSahne;           // Bitince hangi sahneye gidecek?
    public static string mevcutAraSahneID;       // "Giris" veya "OdaSonu" gibi bir isim
}
