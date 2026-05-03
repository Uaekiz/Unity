using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BulmacaSlotKontrol : MonoBehaviour, IDropHandler
{
    public enum ParcaTipi { Sigorta, Kondaktor, Kablo }

    [Header("Ses Ayarları")]
    public AudioClip sokmeSesi; // Parçayı söküp aldığımızda çıkacak ses
    public AudioClip tamirSesi; // Parçayı yerine taktığımızda çıkacak ses

    [Header("Slot Ayarlar�")]
    public ParcaTipi tip;
    public string slotID;

    [Header("Gere� Tan�mlamalar�")]
    public string sokmeAraciID = "Kontrol Kalemi";
    public string kabloTamirAraciID = "Bant";

    [Header("Lamba & G�rsel Ayarlar")]
    public Image hedefLamba;
    public Sprite yananLambaResmi;
    public Sprite sokulmusResim;
    public Sprite tamirResim;

    [Header("E�ya Verme Ayarlar�")]
    public bool sokunceEsyaVersinMi = false;
    public string verilecekEsyaID;

    private bool isSokuldu = false;
    private bool isTamirEdildi = false;
    private Image slotImage;

    void Awake() => slotImage = GetComponent<Image>();

    void Start()
    {
        if (string.IsNullOrEmpty(slotID)) return;

        // Tamir Durumu Kontrol�
        if (GlobalData.DurumNedir("T_" + slotID))
        {
            SetTamirEdildi(true);
        }
        // S�k�lme Durumu Kontrol�
        else if (GlobalData.DurumNedir(slotID))
        {
            SetSokuldu();
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        BulmacaSurukle esya = eventData.pointerDrag?.GetComponent<BulmacaSurukle>();
        if (esya == null) return;

        // 1. S�KME
        if (esya.esyaID == sokmeAraciID && !isSokuldu && tip != ParcaTipi.Kablo)
        {
            SokmeIslemi();
        }
        // 2. TAM�R (Normal Par�alar)
        else if (isSokuldu && !isTamirEdildi && esya.esyaID == tip.ToString())
        {
            EsyayiEnvanterdenSil(esya.esyaID);
            TamirEt();
        }
        // 3. TAM�R (Kablo & Bant)
        else if (tip == ParcaTipi.Kablo && !isTamirEdildi && esya.esyaID == kabloTamirAraciID)
        {
            EsyayiEnvanterdenSil(kabloTamirAraciID);
            TamirEt();
        }
    }

    void TamirEt()
    {
        SetTamirEdildi(false); 

        // Asansör Kontrolü
        Object.FindFirstObjectByType<AsansorKontrol>()?.Denetle();

        Debug.Log($"{slotID} tamir edildi!");

        // --- YENİ EKLENEN: TAMİR ETME / PARÇA TAKMA SESİ ---
        if (ArayuzSesleri.Instance != null && tamirSesi != null)
        {
            ArayuzSesleri.Instance.PanelSesiCal(tamirSesi);
        }
    }

    void SetTamirEdildi(bool loadingFromStart)
    {
        isSokuldu = true;
        isTamirEdildi = true;
        slotImage.sprite = tamirResim;

        if (hedefLamba != null && yananLambaResmi != null)
            hedefLamba.sprite = yananLambaResmi;

        if (!loadingFromStart && !string.IsNullOrEmpty(slotID))
            GlobalData.DurumKaydet("T_" + slotID, true);
    }

    void SokmeIslemi()
    {
        SetSokuldu();

        // Hafıza kaydı
        if (!string.IsNullOrEmpty(slotID))
            GlobalData.DurumKaydet(slotID, true);

        // Eşya Verme
        if (sokunceEsyaVersinMi && !string.IsNullOrEmpty(verilecekEsyaID))
        {
            GlobalData.DurumKaydet(verilecekEsyaID, true);
            ArayuzleriYenile();
        }

        // --- YENİ EKLENEN: SÖKME / EŞYA ALMA SESİ ---
        if (ArayuzSesleri.Instance != null && sokmeSesi != null)
        {
            ArayuzSesleri.Instance.PanelSesiCal(sokmeSesi);
        }
    }

    void SetSokuldu()
    {
        isSokuldu = true;
        slotImage.sprite = sokulmusResim;
    }

    void EsyayiEnvanterdenSil(string esyaID)
    {
        GlobalData.DurumKaydet(esyaID, false);
        ArayuzleriYenile();
    }

    void ArayuzleriYenile()
    {
        // Ana Envanter
        EnvanterManager.Instance?.ArayuzuGuncelle();

        // Bulmaca Envanteri (Parent �zerinden veya sahnede bulma)
        BulmacaEnvanterSistemi bulmacaEnv = GetComponentInParent<BulmacaEnvanterSistemi>()
                                          ?? Object.FindFirstObjectByType<BulmacaEnvanterSistemi>();

        bulmacaEnv?.BulmacaArayuzunuYenile();
    }
}