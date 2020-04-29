using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BotAnimations : MonoBehaviour
{
    /*
    public GameObject botObject;

    [Header("Animator parameters")]
    public string walkIntName = "walkingPace";
    public string jumpTriggerName = "jumps";
    public string fallBoolName = "isFalling";
    public string onGroundBoolName = "isOnGround";
    public string tumbleBoolName = "isTumbling";
    public string closedBoolName = "isClosed";

    [Header("Special parameters")]
    public float groundDelay;

    private Animator animator;

    private Vehicle vehicle;
    private Walker walk;
    private Jumper jump;
    private LocalGravity gravity;
    private Feet feet;

    private UnityAction<bool> walkAnimation;
    private UnityAction jumpAnimation;
    private UnityAction<bool> fallAnimation;
    private UnityAction<bool> onGroundAnimation;
    private UnityAction<bool> tumbleAnimation;
    private UnityAction<Body> openCloseAnimation;

    private bool reallyOnGround;
    private float groundTimer;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        walkAnimation = new UnityAction<bool>(isWalking => WalkAnimation(isWalking));
        jumpAnimation = new UnityAction(() => JumpAnimation());
        fallAnimation = new UnityAction<bool>(isFalling => FallAnimation(isFalling));
        onGroundAnimation = new UnityAction<bool>(onGround => OnGroundAnimation(onGround));
        tumbleAnimation = new UnityAction<bool>(isBalanced => TumbleAnimation(isBalanced));
        openCloseAnimation = new UnityAction<Body>(body => OpenCloseAnimation(body != null));

        if (botObject != null)
        {
            walk = botObject.GetComponent<Walker>();
            jump = botObject.GetComponent<Jumper>();
            gravity = botObject.GetComponent<LocalGravity>();
            feet = botObject.GetComponent<Feet>();
            vehicle = botObject.GetComponent<Vehicle>();
        }
    }

    private void Start()
    {
        if (walk != null)
        {
            walk.IsWalking.AddListener(walkAnimation);
            WalkAnimation(walk.IsWalking.Value);
        }

        if (jump != null) jump.OnJump.AddListener(jumpAnimation);

        if (gravity != null)
        {
            gravity.IsFalling.AddListener(fallAnimation);
            FallAnimation(gravity.IsFalling.Value);
        }

        if (feet != null)
        {
            feet.IsOnGround.AddListener(onGroundAnimation);
            OnGroundAnimation(feet.IsOnGround.Value);

            feet.IsTumbling.AddListener(tumbleAnimation);
            TumbleAnimation(feet.IsTumbling.Value);
        }

        if (vehicle != null)
        {
            vehicle.BodyInside.AddListener(openCloseAnimation);
            OpenCloseAnimation(vehicle.BodyInside.Value != null);
        }
    }

    private void Update()
    {
        if (reallyOnGround == false)
            OnGroundAnimationUpdate();
    }

    public bool HasAnimator()
    {
        return animator != null && animator.runtimeAnimatorController != null;
    }

    public void SetAnimatorController(RuntimeAnimatorController controller)
    {
        if (controller == null)
        {
            WalkAnimation(false);
            FallAnimation(false);
            OnGroundAnimation(true);
            TumbleAnimation(false);
            OpenCloseAnimation(false);

            animator.runtimeAnimatorController = null;
        }
        else
        {
            animator.runtimeAnimatorController = controller;

            WalkAnimation(walk.IsWalking.Value);
            FallAnimation(gravity.IsFalling.Value);
            OnGroundAnimation(feet.IsOnGround.Value);
            TumbleAnimation(feet.IsTumbling.Value);
            OpenCloseAnimation(vehicle.BodyInside.Value != null);
        }
    }

    public void WalkAnimation(bool play)
    {
        if (HasAnimator())
            animator.SetInteger(walkIntName, play ? 0 : -1);
    }

    public void JumpAnimation()
    {
        if (HasAnimator())
            animator.SetTrigger(jumpTriggerName);
    }

    public void FallAnimation(bool play)
    {
        if (HasAnimator())
            animator.SetBool(fallBoolName, play);
    }

    public void OnGroundAnimation(bool play)
    {
        if (HasAnimator())
        {
            if (play == true)
            {
                reallyOnGround = true;
                animator.SetBool(onGroundBoolName, true);
            }
            else if (reallyOnGround == true)
            {
                reallyOnGround = false;
                groundTimer = 0f;
            }
        }
    }

    private void OnGroundAnimationUpdate()
    {
        if (groundTimer > groundDelay)
        {
            reallyOnGround = false;
            animator.SetBool(onGroundBoolName, false);
        }
        else
            groundTimer += Time.deltaTime;
    }

    public void TumbleAnimation(bool play)
    {
        if (HasAnimator())
            animator.SetBool(tumbleBoolName, play);
    }

    public void OpenCloseAnimation(bool close)
    {
        if (HasAnimator())
            animator.SetBool(closedBoolName, close);
    }
    */
}
