using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CutscenePlayer : MonoBehaviour
{
    public Image displayImage; // Inspector'da objenin kendisini buraya sürükle

    private int currentIndex = 0;

    void Start()
    {
        // 1. Kutuda (CutsceneSettings) resim var mý kontrol et
        if (CutsceneSettings.oynatilacakGorseller != null && CutsceneSettings.oynatilacakGorseller.Length > 0)
        {
            displayImage.sprite = CutsceneSettings.oynatilacakGorseller[0];
        }
        else
        {
            Debug.LogError("Görsel bulunamadý! CutsceneSettings doldurulmamýþ.");
        }
    }

    // Bu fonksiyonu Image üzerindeki Button'ýn 'OnClick' kýsmýna baðla
    public void SonrakiGorsel()
    {
        currentIndex++;

        // Eðer hala gösterilecek resim varsa...
        if (currentIndex < CutsceneSettings.oynatilacakGorseller.Length)
        {
            displayImage.sprite = CutsceneSettings.oynatilacakGorseller[currentIndex];
        }
        else
        {
            // Resimler bitti, sahneleri yükleme vaktidir
            SahneyiBitir();
        }
    }

    void SahneyiBitir()
    {
        // Önce izlendi bilgisini kaydedelim (Opsiyonel: Bunu SaveManager'a baðlayabiliriz)
        // GlobalData.Guncelle("Izlendi_" + CutsceneSettings.mevcutAraSahneID, true);

        // Hedef sahneye git
        SceneManager.LoadScene(CutsceneSettings.sonrakiSahne);
    }
}