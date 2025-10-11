using UnityEngine;

// Bu script'in �al��mas� i�in bir Animator bile�eninin ekli olmas�n� zorunlu k�lar.
[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour
{
    // Animator referans� (PlayerMove.cs'ten ta��nd�)
    private Animator _animator;

    void Awake()
    {
        // Awake'te referans� al�yoruz
        _animator = GetComponent<Animator>();
    }

    // Bu public metot, PlayerMove script'inden �a�r�lacak.
    public void UpdateMovementAnimation(int moveDirection, bool isMovingLeft)
    {
        // Animasyon parametrelerini tetikleme mant���
        _animator.SetInteger("moveDir", moveDirection);
        _animator.SetBool("left", isMovingLeft);
    }
}