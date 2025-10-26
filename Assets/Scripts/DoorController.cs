using UnityEngine;
using UnityEngine.UI; // UI kütüphanesini ekliyoruz

public class DoorController : MonoBehaviour
{
    private Animator doorAnimator; // Kapý animatörü
    public GameObject interactButton; // Etkileþim butonu referansý
    public bool isLocked;
    private Button _interactButtonComponent;

    // Animator Controller'daki animasyon parametresini buraya yazýn

    void Start()
    {
        doorAnimator = GetComponent<Animator>();
        // Butonu baþlangýçta görünmez yap
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
            if (_interactButtonComponent != null)
            {
                // 1. Önceki tüm dinleyicileri (baþka kapýlara ait olabilir) temizle.
                _interactButtonComponent.onClick.RemoveAllListeners();

                // 2. Butonun OnClick olayýna, bu kapýnýn TryToOpen metodunu ekle.
                // Dinamik olarak bu GameObject'in TryToOpen metodunu baðlýyoruz.
                _interactButtonComponent.onClick.AddListener(TryToOpen);

                // 3. Butonu görünür yap
                interactButton.SetActive(true);
            }
        }
    }

    // Karakter kapýnýn trigger alanýndan çýktýðýnda
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (_interactButtonComponent != null)
            {
                // Butonu gizlerken, butondan baðlantýyý SÖK.
                _interactButtonComponent.onClick.RemoveAllListeners();
                interactButton.SetActive(false);
            }
        }
    }

    // Butona basýldýðýnda çaðrýlacak metot
    public void TryToOpen()
    {
        if (doorAnimator != null)
        {
            // playDoorAnimation trigger'ýný tetikle
            doorAnimator.SetTrigger("playDoorAnimation");
        }

        if(!isLocked)
        {
            OnSuccessfulInteraction();
        }
       
    }

    protected virtual void OnSuccessfulInteraction() { }
}