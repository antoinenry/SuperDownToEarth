using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MuncherAnimations : MonoBehaviour
{
    public Muncher muncher;

    [Header("Animator parameters")]
    public string munchTriggerName = "munch";
    public string fullBoolName = "isFull";

    private Animator animator;

    private UnityAction munchAnimation;
    private UnityAction<bool> fullAnimation;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        munchAnimation = new UnityAction(() => MunchAnimation());
        fullAnimation = new UnityAction<bool>(isFull => FullAnimation(isFull));
    }

    private void Start()
    {
        if (muncher != null)
        {
            muncher.OnMunch.AddListener(munchAnimation);
            muncher.IsFull.AddListener(fullAnimation);
            FullAnimation(muncher.IsFull.GetValue<bool>());
        }
    }

    public void MunchAnimation()
    {
        if (animator != null)
            animator.SetTrigger(munchTriggerName);
    }

    public void FullAnimation(bool play)
    {
        if (animator != null)
            animator.SetBool(fullBoolName, play);
    }
}
