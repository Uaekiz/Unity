using UnityEngine;
using UnityEngine.UI;
// using System.Security.Cryptography; // Bu kütüphaneye ihtiyacýmýz yok, kaldýrdým.

public class GunAimController : MonoBehaviour
{
    public Joystick joystick;

    private Animator animator;

    // Joystick'in algýlanma eþiði
    public float aimThreshold = 0.0001f;

    // Yön kontrolü için özel deðiþken. Bu, animasyonun sadece bir kez baþlamasýný saðlar.
    private bool isCurrentlyAiming = false;

    // Yatay (Sað/Sol) hareketin ne kadar kayacaðýný belirler
    public float horizontalSensitivity = 6.0f;
    // Dikey (Yukarý/Aþaðý) hareketin ne kadar kayacaðýný belirler
    public float verticalSensitivity = 1.0f;

    // Kolun baþlangýç pozisyonunu tutmak için
    private Vector3 initialPosition;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Baþlangýç pozisyonunu kaydetme
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;
        float inputMagnitude = new Vector2(horizontal, vertical).magnitude;

        bool wantsToAim = inputMagnitude > aimThreshold;

        // 1. Niþan Almaya Baþlama Aný (Joystick'e ilk dokunuþ)
        if (wantsToAim && !isCurrentlyAiming)
        {
            isCurrentlyAiming = true;

            // ÝLERÝ geçiþ animasyonunu baþlatma sinyalini gönder (Holding -> Aiming Transition)
            animator.SetTrigger("StartAim");

            // Pozisyonu kaydýr
            AimPosition(horizontal, vertical);
        }
        // 2. Niþan Almayý Býrakma Aný (Joystick'ten el çekildi)
        else if (!wantsToAim && isCurrentlyAiming)
        {
            isCurrentlyAiming = false;

            // GERÝ geçiþ animasyonunu baþlatma sinyalini gönder (Aim Hold Pose -> Holding Transition)
            animator.SetTrigger("StopAim");

            // Pozisyonu sýfýrla (Yumuþakça geri döner)
            ResetPosition();
        }
        // 3. Niþan Almaya Devam Ediliyorsa (Sadece pozisyonu güncelle)
        else if (isCurrentlyAiming)
        {
            AimPosition(horizontal, vertical);
        }
        // 4. Normal Tutuþta Duruyorsa (Animasyon bitmiþ, pozisyon sýfýrlanýyor)
        else
        {
            // ResetPosition metodu, isCurrentlyAiming false olsa bile 
            // objenin tam olarak initialPosition'a dönmesini saðlar.
            ResetPosition();
        }
    }

    void AimPosition(float x, float y)
    {
        // YATAY KAYDIRMA HESAPLAMASI
        float offsetX = x * horizontalSensitivity;
        // DÝKEY KAYDIRMA HESAPLAMASI
        float offsetY = y * verticalSensitivity;

        // Yeni Kaydýrma Ofseti (X ve Y için farklý hassasiyet kullanýlarak oluþturulur)
        Vector3 aimOffset = new Vector3(offsetX, offsetY, 0);

        // Hedef pozisyon: Baþlangýç pozisyonu + Kaydýrma Ofseti
        Vector3 targetPosition = initialPosition + aimOffset;

        // Yumuþak hareket için Lerp kullan
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * 10f);

        // Tabancanýn açýsýný sabit tut
        transform.localRotation = Quaternion.identity;
    }

    void ResetPosition()
    {
        // Baþlangýç pozisyonuna geri dön (Objenin pozisyonu hala initialPosition'a doðru hareket ediyorsa devam eder)
        transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition, Time.deltaTime * 5f);

        // Açýyý sýfýrda tut
        transform.localRotation = Quaternion.identity;
    }
}