using System.Collections;
using UnityEngine;

public class Walker : BodyPart
{     
    public Feet Feet { get; private set; }

    private Coroutine walkCoroutine;
    private Vector2 currentWalkVelocity;
    private bool switchingGears;

    public FloatChangeEvent walkSpeed;
    public IntChangeEvent currentWalkDirection;

    private void Awake()
    {
        AttachedBody = GetComponent<Body>();
        Feet = GetComponent<Feet>();
    }

    private void FixedUpdate()
    {
        if (Feet.IsOnGround)
            AttachedRigidbody.velocity = Feet.GroundVelocity + currentWalkVelocity;
    }

    private void OnEnable()
    {
        currentWalkDirection.AddValueListener<int>(OnWalkDirectionChange);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        currentWalkDirection.RemoveValueListener<int>(OnWalkDirectionChange);
    }

    public void Walk(int direction)
    {
        currentWalkDirection.Value = direction;
    }

    private void OnWalkDirectionChange(int newWalkDirection)
    {
        currentWalkDirection.SetValueWithoutTriggeringEvent(Mathf.Clamp(newWalkDirection, -1, 1));

        if (walkCoroutine != null)
            StopCoroutine(walkCoroutine);

        walkCoroutine = StartCoroutine(WalkCoroutine());
    }

    private IEnumerator WalkCoroutine()
    {
        do
        {
            while (Feet.IsOnGround == false || Feet.IsTumbling == true)
                yield return new WaitForFixedUpdate();

            switch (currentWalkDirection)
            {
                case 0:
                    currentWalkVelocity = Vector2.zero;
                    break;

                case 1:
                    AttachedRigidbody.transform.localScale = Vector3.one;
                    currentWalkVelocity = Quaternion.Euler(0f, 0f, AttachedRigidbody.rotation) * Vector2.right * walkSpeed;
                    break;

                case -1:
                    AttachedRigidbody.transform.localScale = new Vector3(-1f, 1f, 1f);
                    currentWalkVelocity = Quaternion.Euler(0f, 0f, AttachedRigidbody.rotation) * Vector2.left * walkSpeed;
                    break;
            }

            yield return new WaitForFixedUpdate();
        }
        while (currentWalkDirection != 0);
    } 
}
