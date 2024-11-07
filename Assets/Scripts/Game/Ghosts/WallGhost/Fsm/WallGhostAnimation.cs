using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGhostAnimation : MonoBehaviour
{
    private static readonly int IsDeath = Animator.StringToHash("IsDeath");
    private static readonly int Catched = Animator.StringToHash("Catched");
    
    [SerializeField] private Animator animator;
    
    public void SetIsDeathState(bool state)
    {
        animator.SetBool(IsDeath, state);
    }
    
    public void SetGrabTrigger()
    {
        animator.SetTrigger(Catched);
    }
}
