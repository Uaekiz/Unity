using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnvanterManager : MonoBehaviour
{
    // Singleton yapısı
    public static EnvanterManager Instance;

    [Header("Arayüz Panelleri")]
    public GameObject envanterPaneli; // Alt köşedeki 4'lü eşya slotu paneli
    public GameObject tumBulmacaPanelleriParent; // Hiyerarşideki tüm panelleri içine koyduğun boş obje

    [Header("Eşya Slotları")]
    public Image[] slotlar;

    [System.Serializable]
    public struct EsyaResmi
    {
        public string esyaID;
        public Sprite esyaGorseli;
    }

    [Header("Eşya Veritabanı")]
    public List<EsyaResmi> esyaKutuphanesi;

    [Header("Asansör Özel Ayarları")]
    public GameObject asansorPaneli;
    public CanvasGroup asansorCanvasGroup;

    private void Awake()
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

    private void Start()
    {
        ArayuzuGuncelle();
    }

    // Bu fonksiyonu sahnede açık kalmış olabilecek TÜM bulmaca panellerini kapatmak için kullanabilirsin
    public void TumBulmacaPanelleriniKapat()
    {
        if (tumBulmacaPanelleriParent != null)
        {
            // Parent'ın altındaki tüm çocukları (panelleri) döngüyle kapatır
            foreach (Transform panel in tumBulmacaPanelleriParent.transform)
            {
                panel.gameObject.SetActive(false);
            }
        }
    }

    public void ArayuzuGuncelle()
    {
        foreach (Image slot in slotlar)
        {
            slot.sprite = null;
            slot.color = new Color(1, 1, 1, 0);
        }

        int siradakiBosSlot = 0;

        foreach (EsyaResmi esya in esyaKutuphanesi)
        {
            if (GlobalData.DurumNedir(esya.esyaID) == true)
            {
                if (siradakiBosSlot < slotlar.Length)
                {
                    slotlar[siradakiBosSlot].sprite = esya.esyaGorseli;
                    slotlar[siradakiBosSlot].color = new Color(1, 1, 1, 1);
                    siradakiBosSlot++;
                }
            }
        }
    }

    public void SavasModu(bool savastaMi)
    {
        if (envanterPaneli != null)
        {
            envanterPaneli.SetActive(!savastaMi);
        }

        // Savaşa girince eğer bir bulmaca paneli açıksa onu da kapatır
        if (savastaMi) TumBulmacaPanelleriniKapat();
    }


    [Header("Duraklatma Ayarlari")]
    public GameObject duraklatmaPaneli; // Inspector'dan yeni paneli buraya sürükle

    public void DuraklatmaPaneliniAc()
    {
        if (duraklatmaPaneli != null)
        {
            duraklatmaPaneli.SetActive(true);
            // Arkadaki her şeyi durdurmak için zamanı donduruyoruz
            Time.timeScale = 0f;
        }
    }

    public void DevamEt()
    {
        if (duraklatmaPaneli != null)
        {
            duraklatmaPaneli.SetActive(false);
            // Zamanı normale döndürüyoruz
            Time.timeScale = 1f;
        }
    }

    public void AnaSayfayaDon()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("AnaSayfa");
    }
}