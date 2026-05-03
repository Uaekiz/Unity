using UnityEngine;
using UnityEngine.UI;
using System.Collections; // Bekleme (Coroutine) işlemleri için eklendi

public class DoorController : MonoBehaviour
{
    private Animator doorAnimator; 
    public GameObject interactButton; 
    public bool isLocked;
    private Button _interactButtonComponent;

    [Header("Ses Ayarları")]
    public AudioSource kapiSesKaynagi; // Kapının üzerindeki ses çalar
    public AudioClip kilitliSesi;      // Kapı kilitliyken çıkacak ses (Tak-tuk)
    public AudioClip acilmaSesi;       // Kapı açılırken çıkacak ses (Gıcırtı vb.)

    void Start()
    {
        doorAnimator = GetComponent<Animator>();
        
        if (interactButton != null)
        {
            _interactButtonComponent = interactButton.GetComponent<Button>();
            interactButton.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMove.aktifEtkilesimObjesi = this.gameObject;

            if (_interactButtonComponent != null)
            {
                _interactButtonComponent.onClick.RemoveAllListeners();
                _interactButtonComponent.onClick.AddListener(TryToOpen);
                interactButton.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (PlayerMove.aktifEtkilesimObjesi == this.gameObject)
            {
                if (_interactButtonComponent != null)
                {
                    _interactButtonComponent.onClick.RemoveAllListeners();
                    interactButton.SetActive(false);
                }
                
                PlayerMove.aktifEtkilesimObjesi = null;
            }
        }
    }

    public void TryToOpen()
    {
        // Peş peşe spam tıklamayı önlemek için basıldığı an butonu gizle
        if (interactButton != null) interactButton.SetActive(false);

        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("playDoorAnimation");
        }

        if (isLocked)
        {
            // --- KİLİTLİ KAPI SENARYOSU ---
            if (kapiSesKaynagi != null && kilitliSesi != null)
            {
                kapiSesKaynagi.PlayOneShot(kilitliSesi);
            }
            
            // Oyuncu kapıyı tekrar denemek isterse diye 1 saniye sonra butonu geri getir
            Invoke("ButonuGeriAc", 1f); 
        }
        else
        {
            // --- AÇILAN KAPI SENARYOSU ---
            StartCoroutine(KapiAcilmaSekansi());
        }
    }

    // Kapı başarılı şekilde açıldığında çalışacak bekleme sekansı
    IEnumerator KapiAcilmaSekansi()
    {
        if (kapiSesKaynagi != null && acilmaSesi != null)
        {
            kapiSesKaynagi.PlayOneShot(acilmaSesi);
            
            // Ses klibinin uzunluğu (saniye) kadar bekle
            yield return new WaitForSeconds(1.5f); 
        }
        else
        {
            // Ses atanmamışsa oyunun donmaması için 1 saniyelik yedek bekleme süresi
            yield return new WaitForSeconds(1f);
        }

        // Ses bittikten sonra asıl geçiş/başarı fonksiyonunu çalıştır
        OnSuccessfulInteraction();
    }

    // Kilitli kapıyı denedikten sonra butonu tekrar aktif eden metot
    private void ButonuGeriAc()
    {
        // Sadece oyuncu hala bu kapının trigger'ı içindeyse butonu geri getir
        if (PlayerMove.aktifEtkilesimObjesi == this.gameObject && interactButton != null)
        {
            interactButton.SetActive(true);
        }
    }

    protected virtual void OnSuccessfulInteraction() { }
}