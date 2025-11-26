using UnityEngine;
using UnityEngine.UI;
// using System.Security.Cryptography; // Bu k�t�phaneye ihtiyac�m�z yok, kald�rd�m.

public class GunAimController : MonoBehaviour
{
    public Joystick joystick;

    private Animator animator;

    // Joystick'in alg�lanma e�i�i
    public float aimThreshold = 0.0001f;

    // Y�n kontrol� i�in �zel de�i�ken. Bu, animasyonun sadece bir kez ba�lamas�n� sa�lar.
    private bool isCurrentlyAiming = false;

    // Yatay (Sa�/Sol) hareketin ne kadar kayaca��n� belirler
    public float horizontalSensitivity = 6.0f;
    // Dikey (Yukar�/A�a��) hareketin ne kadar kayaca��n� belirler
    public float verticalSensitivity = 1.0f;

    // Kolun ba�lang�� pozisyonunu tutmak i�in
    private Vector3 initialPosition;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Ba�lang�� pozisyonunu kaydetme
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;
        float inputMagnitude = new Vector2(horizontal, vertical).magnitude;

        bool wantsToAim = inputMagnitude > aimThreshold;

        // 1. Ni�an Almaya Ba�lama An� (Joystick'e ilk dokunu�)
        if (wantsToAim && !isCurrentlyAiming)
        {
            isCurrentlyAiming = true;

            // �LER� ge�i� animasyonunu ba�latma sinyalini g�nder (Holding -> Aiming Transition)
            animator.SetTrigger("StartAim");

            // Pozisyonu kayd�r
            AimPosition(horizontal, vertical);
        }
        // 2. Ni�an Almay� B�rakma An� (Joystick'ten el �ekildi)
        else if (!wantsToAim && isCurrentlyAiming)
        {
            isCurrentlyAiming = false;

            // GER� ge�i� animasyonunu ba�latma sinyalini g�nder (Aim Hold Pose -> Holding Transition)
            animator.SetTrigger("StopAim");

            // Pozisyonu s�f�rla (Yumu�ak�a geri d�ner)
            ResetPosition();
        }
        // 3. Ni�an Almaya Devam Ediliyorsa (Sadece pozisyonu g�ncelle)
        else if (isCurrentlyAiming)
        {
            AimPosition(horizontal, vertical);
        }
        // 4. Normal Tutu�ta Duruyorsa (Animasyon bitmi�, pozisyon s�f�rlan�yor)
        else
        {
            // ResetPosition metodu, isCurrentlyAiming false olsa bile 
            // objenin tam olarak initialPosition'a d�nmesini sa�lar.
            ResetPosition();
        }
    }

    void AimPosition(float x, float y)
    {
        // YATAY KAYDIRMA HESAPLAMASI
        float offsetX = x * horizontalSensitivity;
        // D�KEY KAYDIRMA HESAPLAMASI
        float offsetY = y * verticalSensitivity;

        // Yeni Kayd�rma Ofseti (X ve Y i�in farkl� hassasiyet kullan�larak olu�turulur)
        Vector3 aimOffset = new Vector3(offsetX, offsetY, 0);

        // Hedef pozisyon: Ba�lang�� pozisyonu + Kayd�rma Ofseti
        Vector3 targetPosition = initialPosition + aimOffset;

        // Yumu�ak hareket i�in Lerp kullan
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * 10f);

        // Tabancan�n a��s�n� sabit tut
        transform.localRotation = Quaternion.identity;
    }

    void ResetPosition()
    {
        // Ba�lang�� pozisyonuna geri d�n (Objenin pozisyonu hala initialPosition'a do�ru hareket ediyorsa devam eder)
        transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition, Time.deltaTime * 5f);

        // A��y� s�f�rda tut
        transform.localRotation = Quaternion.identity;
    }

    public void ShootGun()
    {
        // Ate� etme sinyalini Animator'a g�nder
        animator.SetTrigger("shoot");

        // NOT: Ate� etme sesini, mermi ��karma, hasar verme gibi di�er mant�klar buraya eklenecektir.
        Debug.Log("Bang! Ate� edildi.");
    }
}