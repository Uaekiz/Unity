using UnityEngine;
using UnityEngine.UI; // UI k�t�phanesini ekliyoruz

public class DoorController : MonoBehaviour
{
    private Animator doorAnimator; // Kap� animat�r�
    public GameObject interactButton; // Etkile�im butonu referans�

    // Animator Controller'daki animasyon parametresini buraya yaz�n
    private string openAnimationName = "Locked_1";

    void Start()
    {
        doorAnimator = GetComponent<Animator>();
        // Butonu ba�lang��ta g�r�nmez yap
        if (interactButton != null)
        {
            interactButton.SetActive(false);
        }
    }

    // Karakter kap�n�n trigger alan�na girdi�inde
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Butonu g�r�n�r yap
            if (interactButton != null)
            {
                interactButton.SetActive(true);
            }
        }
    }

    // Karakter kap�n�n trigger alan�ndan ��kt���nda
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Butonu g�r�nmez yap
            if (interactButton != null)
            {
                interactButton.SetActive(false);
            }
        }
    }

    // Butona bas�ld���nda �a�r�lacak metot
    public void PlayDoorAnimation()
    {
        if (doorAnimator != null)
        {
            // playDoorAnimation trigger'�n� tetikle
            doorAnimator.SetTrigger("playDoorAnimation");
        }
    }
}