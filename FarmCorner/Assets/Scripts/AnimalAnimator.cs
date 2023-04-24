using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AnimalAnimator : MonoBehaviour
{
    [SerializeField] AnimalManager animalManager;
    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        animalManager.IsWalkEvent += isWalk;
        animalManager.IsIdleEvent += isIdle;
    }
    private void OnDisable()
    {
        animalManager.IsWalkEvent -= isWalk;
        animalManager.IsIdleEvent -= isIdle;
    }
    void isWalk()
    {
        animator.SetBool("isWalk", true);
    }
    void isIdle()
    {
        animator.SetBool("isWalk", false);
    }
}
