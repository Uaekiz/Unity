using UnityEngine;

// Bu script'in �al��mas� i�in bir Animator bile�eninin ekli olmas�n� zorunlu k�lar.
[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour
{
    // Animator referans� (PlayerMove.cs'ten ta��nd�)
    private Animator _animator;
    private AudioSource _audioSource;

    void Awake()
    {
        // Awake'te referans� al�yoruz
        _animator = GetComponent<Animator>();
        _audioSource = GetComponentInParent<AudioSource>();
    }

    // Bu public metot, PlayerMove script'inden �a�r�lacak.
    public void UpdateMovementAnimation(int moveDirection, bool isMovingLeft)
    {
        // Animasyon parametrelerini tetikleme mant���
        _animator.SetInteger("moveDir", moveDirection);
        _animator.SetBool("left", isMovingLeft);
    }

    public void PlayFootstepSound()
    {
        if (_animator != null && _animator.GetInteger("moveDir") != 0)
        {
            // Karakter hareket ediyorsa...
            if (_audioSource != null && _audioSource.clip != null)
            {
                // PlayOneShot kullanmak, h�zl� ad�mlarda seslerin �st �ste binmesine izin verir.
                _audioSource.PlayOneShot(_audioSource.clip);
            }
        }
    }
}