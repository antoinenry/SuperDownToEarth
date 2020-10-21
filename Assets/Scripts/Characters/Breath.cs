using UnityEngine;

public class Breath : MonoBehaviour
{
    public string breathableTag = "Atmosphere";
    public float maxHoldBreathTime = 1f;

    private float holdBreathTimer = 0f;
    private bool airIsBreathable;

    public BoolChangeEvent canBreathe;
    public FloatChangeEvent reserve;
    public Trigger suffocate;

    private void FixedUpdate()
    {
        canBreathe.Value = airIsBreathable;

        if (canBreathe == true)
        {
            holdBreathTimer = 0f;
            reserve.Value = 1f;
        }
        else
        {
            if (maxHoldBreathTime <= 0f)
                reserve.Value = 0f;
            else
                reserve.Value = 1f - holdBreathTimer / maxHoldBreathTime;

            if (holdBreathTimer > maxHoldBreathTime)
            {
                holdBreathTimer = maxHoldBreathTime;
                suffocate.Trigger();
            }
            else
                holdBreathTimer += Time.fixedDeltaTime;
        }

        airIsBreathable = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == breathableTag)
            airIsBreathable = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == breathableTag)
            airIsBreathable = true;
    }
}
