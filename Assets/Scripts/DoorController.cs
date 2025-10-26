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
            if (_interactButtonComponent != null)
            {
                // 1. �nceki t�m dinleyicileri (ba�ka kap�lara ait olabilir) temizle.
                _interactButtonComponent.onClick.RemoveAllListeners();

                // 2. Butonun OnClick olay�na, bu kap�n�n TryToOpen metodunu ekle.
                // Dinamik olarak bu GameObject'in TryToOpen metodunu ba�l�yoruz.
                _interactButtonComponent.onClick.AddListener(TryToOpen);

                // 3. Butonu g�r�n�r yap
                interactButton.SetActive(true);
            }
        }
    }

    // Karakter kap�n�n trigger alan�ndan ��kt���nda
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (_interactButtonComponent != null)
            {
                // Butonu gizlerken, butondan ba�lant�y� S�K.
                _interactButtonComponent.onClick.RemoveAllListeners();
                interactButton.SetActive(false);
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