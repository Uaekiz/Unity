using UnityEngine;
using UnityEngine.UI; // UI k�t�phanesini ekliyoruz

public class DoorController : MonoBehaviour
{
    private Animator doorAnimator; // Kap� animat�r�
    public GameObject interactButton; // Etkile�im butonu referans�
    public bool isLocked;
    private Button _interactButtonComponent;

    // Animator Controller'daki animasyon parametresini buraya yaz�n

    void Start()
    {
        doorAnimator = GetComponent<Animator>();
        // Butonu ba�lang��ta g�r�nmez yap
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
            // YENİ EKLENEN KISIM: Butonun sahibi benim! Hafızaya yaz.
            PlayerMove.aktifEtkilesimObjesi = this.gameObject;

            if (_interactButtonComponent != null)
            {
                // 1. Önceki tüm dinleyicileri (başka kapılara ait olabilir) temizle.
                _interactButtonComponent.onClick.RemoveAllListeners();

                // 2. Butonun OnClick olayına, bu kapının TryToOpen metodunu ekle.
                _interactButtonComponent.onClick.AddListener(TryToOpen);

                // 3. Butonu görünür yap
                interactButton.SetActive(true);
            }
        }
    }

    // Karakter kapının trigger alanından çıktığında
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // YENİ EKLENEN KISIM: Butonu sadece asıl sahibi bensem gizle!
            if (PlayerMove.aktifEtkilesimObjesi == this.gameObject)
            {
                if (_interactButtonComponent != null)
                {
                    // Butonu gizlerken, butondan bağlantıyı SÖK.
                    _interactButtonComponent.onClick.RemoveAllListeners();
                    interactButton.SetActive(false);
                }
                
                // Sahibi kalmadı diye belirt
                PlayerMove.aktifEtkilesimObjesi = null;
            }
        }
    }

    // Butona bas�ld���nda �a�r�lacak metot
    public void TryToOpen()
    {
        if (doorAnimator != null)
        {
            // playDoorAnimation trigger'�n� tetikle
            doorAnimator.SetTrigger("playDoorAnimation");
        }

        if(!isLocked)
        {
            OnSuccessfulInteraction();
        }
       
    }

    protected virtual void OnSuccessfulInteraction() { }
}