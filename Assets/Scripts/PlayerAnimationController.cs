using UnityEngine;

// Bu script'in çalýþmasý için bir Animator bileþeninin ekli olmasýný zorunlu kýlar.
[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour
{
    // Animator referansý (PlayerMove.cs'ten taþýndý)
    private Animator _animator;

    void Awake()
    {
        // Awake'te referansý alýyoruz
        _animator = GetComponent<Animator>();
    }

    // Bu public metot, PlayerMove script'inden çaðrýlacak.
    public void UpdateMovementAnimation(int moveDirection, bool isMovingLeft)
    {
        // Animasyon parametrelerini tetikleme mantýðý
        _animator.SetInteger("moveDir", moveDirection);
        _animator.SetBool("left", isMovingLeft);
    }
}