using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int IsCleaning = Animator.StringToHash("IsCleaning");
    
    [SerializeField] private Animator animator;
    
    public void SetWalkState(bool state)
    {
        animator.SetBool(IsWalking, state);
    }
    
    public void SetCleaning(bool state)
    {
        animator.SetBool(IsCleaning, state);
    }
}