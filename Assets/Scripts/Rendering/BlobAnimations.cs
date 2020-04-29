using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BlobAnimations : MonoBehaviour
{
    /*
    [Header("Ground blob animation")]
    public FlatGroundProbe probe;
    public string groundFlatnessIntName = "groundFlatness";

    [Header("Ground blob animation")]
    public Pilot pilot;
    public string isPilotingBoolName = "isPiloting";

    private Animator animator;
    private bool isPiloting;
    
    private UnityAction<int> groundFlatnessAnimation;
    private UnityAction<Body> pilotingAnimation;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        groundFlatnessAnimation = new UnityAction<int>(groundFlatness => GroundFlatnessAnimation(groundFlatness));
        pilotingAnimation = new UnityAction<Body>(pilotedBody => PilotAnimation(pilot.IsInsideVehicle));
    }

    private void Start()
    {
        AddAnimationListeners();
        InitAnimations();
    }

    public void AddAnimationListeners()
    {
        if (probe != null) probe.GroundFlatness.AddListener(groundFlatnessAnimation);
        if (pilot != null) pilot.PilotedBody.AddListener(pilotingAnimation);
    }

    public void RemoveAnimationListeners()
    {
        if (probe != null) probe.GroundFlatness.RemoveListener(groundFlatnessAnimation);
        if (pilot != null) pilot.PilotedBody.RemoveListener(pilotingAnimation);
    }

    public void InitAnimations()
    {
        if (probe != null) groundFlatnessAnimation(probe.GroundFlatness.Value);
        if (pilot != null) PilotAnimation(pilot.IsInsideVehicle);
    }

    public void GroundFlatnessAnimation(int flatness)
    {
        animator.SetInteger(groundFlatnessIntName, flatness);
    }

    public void PilotAnimation(bool play)
    {
        if (play != isPiloting)
        {
            isPiloting = play;            
            animator.SetBool(isPilotingBoolName, play);
        }
    }

    private void OnDestroy()
    {
        RemoveAnimationListeners();
    }
    */
}
