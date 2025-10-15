using UnityEngine;
using UnityEngine.UI; // UI kütüphanesini ekliyoruz

public class DoorController : MonoBehaviour
{
    private Animator doorAnimator; // Kapý animatörü
    public GameObject interactButton; // Etkileþim butonu referansý

    // Animator Controller'daki animasyon parametresini buraya yazýn
    private string openAnimationName = "Locked_1";

    void Start()
    {
        doorAnimator = GetComponent<Animator>();
        // Butonu baþlangýçta görünmez yap
        if (interactButton != null)
        {
            interactButton.SetActive(false);
        }
    }

    // Karakter kapýnýn trigger alanýna girdiðinde
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Butonu görünür yap
            if (interactButton != null)
            {
                interactButton.SetActive(true);
            }
        }
    }

    // Karakter kapýnýn trigger alanýndan çýktýðýnda
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Butonu görünmez yap
            if (interactButton != null)
            {
                interactButton.SetActive(false);
            }
        }
    }

    // Butona basýldýðýnda çaðrýlacak metot
    public void PlayDoorAnimation()
    {
        if (doorAnimator != null)
        {
            // playDoorAnimation trigger'ýný tetikle
            doorAnimator.SetTrigger("playDoorAnimation");
        }
    }
}