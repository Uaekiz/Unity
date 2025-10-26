using UnityEngine;

// Bu script'in çalýþmasý için bir Animator bileþeninin ekli olmasýný zorunlu kýlar.
[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour
{
    // Animator referansý (PlayerMove.cs'ten taþýndý)
    private Animator _animator;
    private AudioSource _audioSource;

    void Awake()
    {
        // Awake'te referansý alýyoruz
        _animator = GetComponent<Animator>();
        _audioSource = GetComponentInParent<AudioSource>();
    }

    // Bu public metot, PlayerMove script'inden çaðrýlacak.
    public void UpdateMovementAnimation(int moveDirection, bool isMovingLeft)
    {
        // Animasyon parametrelerini tetikleme mantýðý
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
                // PlayOneShot kullanmak, hýzlý adýmlarda seslerin üst üste binmesine izin verir.
                _audioSource.PlayOneShot(_audioSource.clip);
            }
        }
    }
}