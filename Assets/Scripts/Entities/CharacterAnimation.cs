using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimation : MonoBehaviour
{
    private Animator _animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void Idle()
    {
        _animator.SetBool("isWalking", false);
        _animator.SetBool("isSprinting", false);
    }

    public void Walk()
    {
        _animator.SetBool("isWalking", true);
        _animator.SetBool("isSprinting", false);
    }

    public void Sprint()
    {
        _animator.SetBool("isWalking", true);
        _animator.SetBool("isSprinting", true);
    }
}
