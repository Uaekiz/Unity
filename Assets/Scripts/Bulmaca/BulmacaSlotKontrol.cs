using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BulmacaSlotKontrol : MonoBehaviour, IDropHandler
{
    public enum ParcaTipi { Sigorta, Kondaktor, Kablo }

    [Header("Slot Ayarlarý")]
    public ParcaTipi tip;
    public string slotID;

    [Header("Gereç Tanýmlamalarý")]
    public string sokmeAracýID = "Kontrol Kalemi";
    public string kabloTamirAracýID = "Bant";

    [Header("Lamba & Görsel Ayarlar")]
    public Image hedefLamba;
    public Sprite yananLambaResmi;
    public Sprite sokulmusResim;
    public Sprite tamirResim;

    [Header("Eþya Verme Ayarlarý")]
    public bool sokunceEsyaVersinMi = false;
    public string verilecekEsyaID;

    private bool isSokuldu = false;
    private bool isTamirEdildi = false;
    private Image slotImage;

    void Awake() => slotImage = GetComponent<Image>();

    void Start()
    {
        if (string.IsNullOrEmpty(slotID)) return;

        // Tamir Durumu Kontrolü
        if (GlobalData.DurumNedir("T_" + slotID))
        {
            SetTamirEdildi(true);
        }
        // Sökülme Durumu Kontrolü
        else if (GlobalData.DurumNedir(slotID))
        {
            SetSokuldu();
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        BulmacaSurukle esya = eventData.pointerDrag?.GetComponent<BulmacaSurukle>();
        if (esya == null) return;

        // 1. SÖKME
        if (esya.esyaID == sokmeAracýID && !isSokuldu && tip != ParcaTipi.Kablo)
        {
            SokmeIslemi();
        }
        // 2. TAMÝR (Normal Parçalar)
        else if (isSokuldu && !isTamirEdildi && esya.esyaID == tip.ToString())
        {
            EsyayiEnvanterdenSil(esya.esyaID);
            TamirEt();
        }
        // 3. TAMÝR (Kablo & Bant)
        else if (tip == ParcaTipi.Kablo && !isTamirEdildi && esya.esyaID == kabloTamirAracýID)
        {
            EsyayiEnvanterdenSil(kabloTamirAracýID);
            TamirEt();
        }
    }

    void TamirEt()
    {
        SetTamirEdildi(false); 

        // Asansör Kontrolü
        Object.FindFirstObjectByType<AsansorKontrol>()?.Denetle();

        Debug.Log($"{slotID} tamir edildi!");
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

        // Hafýza kaydý
        if (!string.IsNullOrEmpty(slotID))
            GlobalData.DurumKaydet(slotID, true);

        // Eþya Verme
        if (sokunceEsyaVersinMi && !string.IsNullOrEmpty(verilecekEsyaID))
        {
            GlobalData.DurumKaydet(verilecekEsyaID, true);
            ArayuzleriYenile();
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

        // Bulmaca Envanteri (Parent üzerinden veya sahnede bulma)
        BulmacaEnvanterSistemi bulmacaEnv = GetComponentInParent<BulmacaEnvanterSistemi>()
                                          ?? Object.FindFirstObjectByType<BulmacaEnvanterSistemi>();

        bulmacaEnv?.BulmacaArayuzunuYenile();
    }
}