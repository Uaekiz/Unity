using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BulmacaSlotKontrol : MonoBehaviour, IDropHandler
{
    public enum ParcaTipi { Sigorta, Kondaktor, Kablo }
    public ParcaTipi tip;

    [Header("Lamba Ayarlarý")]
    public Image hedefLamba; // Bu slot tamir olunca hangi lamba yanacak?
    public Sprite yananLambaResmi; // Lambanýn yanan hali (Sprite)

    [Header("KÝMLÝK AYARI")]
    [Tooltip("Her slot için farklý bir isim verin (Örn: Oda_Sigorta_1, Koridor_Sigorta_1)")]
    public string slotID; // Sahne deðiþince hatýrlamasý için TC kimlik numarasý gibi düþünün.

    [Header("Durum Görselleri")]
    public Sprite sokulmusResim;
    public Sprite tamirResim;

    [Header("Eþya Verme Ayarlarý")]
    public bool sokunceEsyaVersinMi = false;
    public string verilecekEsyaID;

    private bool isSokuldu = false;
    private bool isTamirEdildi = false;
    private Image slotImage;

    void Awake()
    {
        slotImage = GetComponent<Image>();
    }

    void Start()
    {
        if (!string.IsNullOrEmpty(slotID))
        {
            // Önce tamir durumuna bak (Öncelikli durum)
            if (GlobalData.DurumNedir("T_" + slotID))
            {
                isSokuldu = true;
                isTamirEdildi = true;
                slotImage.sprite = tamirResim;

                // --- YENÝ: Sahne açýldýðýnda lamba durumunu hatýrla ---
                if (hedefLamba != null && yananLambaResmi != null)
                {
                    hedefLamba.sprite = yananLambaResmi;
                }
            }
            // Tamir edilmemiþse sökülme durumuna bak
            else if (GlobalData.DurumNedir(slotID))
            {
                isSokuldu = true;
                slotImage.sprite = sokulmusResim;
            }
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        GameObject suruklenenObje = eventData.pointerDrag;
        if (suruklenenObje == null) return;

        BulmacaSurukle esya = suruklenenObje.GetComponent<BulmacaSurukle>();
        if (esya == null) return;

        // --- 1. SÖKME (Kablo hariç her þey sökülebilir) ---
        if (esya.esyaID == "Kontrol Kalemi" && !isSokuldu && tip != ParcaTipi.Kablo)
        {
            SokmeIslemi();
        }

        else if (isSokuldu && !isTamirEdildi)
        {
            // Konsola ne geldiðini yazdýralým, hatayý þak diye göreceðiz
            Debug.Log("Sürüklenen: " + esya.esyaID + " | Yuva Beklenen: " + tip.ToString());

            if (esya.esyaID == tip.ToString())
            {
                TamirEt();
            }
            else
            {
                Debug.LogWarning("Ýsimler uyuþmuyor! Lütfen Console'daki yazýma dikkat et.");
            }
        }

        // --- 3. ÖZEL DURUM: KABLO VE BANT ---
        else if (tip == ParcaTipi.Kablo && esya.esyaID == "Bant" && !isTamirEdildi)
        {
            TamirEt();
        }
    }

    void TamirEt()
    {
        isTamirEdildi = true;
        slotImage.sprite = tamirResim; // Saðlam parça görselini tak

        // --- YENÝ: LAMBAYI YAK ---
        if (hedefLamba != null && yananLambaResmi != null)
        {
            hedefLamba.sprite = yananLambaResmi;
        }

        if (!string.IsNullOrEmpty(slotID))
        {
            GlobalData.DurumKaydet("T_" + slotID, true);
        }

        AsansorKontrol asansor = Object.FindFirstObjectByType<AsansorKontrol>();
        if (asansor != null)
        {
            asansor.Denetle();
        }
        Debug.Log(slotID + " baþarýyla tamir edildi!");
    }

    void SokmeIslemi()
    {
        isSokuldu = true;
        slotImage.sprite = sokulmusResim;

        // 1. DURUMU HAFIZAYA KAYDET (Kritik nokta)
        if (!string.IsNullOrEmpty(slotID))
        {
            GlobalData.DurumKaydet(slotID, true);
        }

        // 2. Eðer eþya verilecekse (Saðlam parçaysa)
        if (sokunceEsyaVersinMi && !string.IsNullOrEmpty(verilecekEsyaID))
        {
            GlobalData.DurumKaydet(verilecekEsyaID, true);

            if (EnvanterManager.Instance != null)
                EnvanterManager.Instance.ArayuzuGuncelle();

            BulmacaEnvanterSistemi bulmacaEnv = GetComponentInParent<BulmacaEnvanterSistemi>();
            if (bulmacaEnv != null)
                bulmacaEnv.BulmacaArayuzunuYenile();
        }

        Debug.Log(tip + " söküldü ve " + slotID + " olarak kaydedildi.");
    }
}