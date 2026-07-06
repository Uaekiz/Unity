using UnityEngine;
using UnityEngine.UI; // Slider ile etkileşim kurabilmek için ekledik

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [Header("UI Referansları")]
    public GameObject ayarlarPaneli;
    public Slider anaSesSlider; // YENİ: Unity arayüzünden buraya ses slider'ını sürükle

    [Header("Ses Ayarlari")]
    public float anlikMuzikSesi = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // YENİ: Oyun başladığında daha önceden kaydedilmiş sesi bul. 
        // Eğer daha önce hiç kaydedilmemişse (ilk açılış), sesi 1 (ful) yap.
        float kayitliSes = PlayerPrefs.GetFloat("OyunSesi", 1f);
        
        // Unity'nin ana şalterini bu kayıtlı sese ayarla
        AudioListener.volume = kayitliSes;

        // Ayarlar menüsündeki Slider'ın çubuğunu da bu değere getir
        if (anaSesSlider != null)
        {
            anaSesSlider.value = kayitliSes;
        }
    }

    public void AyarlariAc()
    {
        if (ayarlarPaneli != null)
        {
            ayarlarPaneli.SetActive(true);
        }
    }

    public void AyarlariKapat()
    {
        if (ayarlarPaneli != null)
        {
            ayarlarPaneli.SetActive(false);
        }
    }

    // Slider hareket ettikçe bu fonksiyon çalışır
    public void SesSeviyesiniAyarla(float sesDegeri)
    {
        AudioListener.volume = sesDegeri; 
        
        // YENİ: Oyuncu slider'ı her kaydırdığında bu değeri kalıcı olarak sisteme kaydet
        PlayerPrefs.SetFloat("OyunSesi", sesDegeri);
        PlayerPrefs.Save();
    }

    public void MuzikSeviyesiniAyarla(float muzikDegeri)
    {
        anlikMuzikSesi = muzikDegeri;
        
        // Müziği de kalıcı kaydedelim
        PlayerPrefs.SetFloat("MuzikSesi", muzikDegeri);
        PlayerPrefs.Save();
    }
}