using UnityEngine;

public class BotVehicle : Vehicle
{
    public override void SetBodyInside(Body body)
    {
        base.SetBodyInside(body);

        LocalGravity gravity = GetComponent<LocalGravity>();
        if (gravity == null) return;

        if (body == null)
            gravity.enabled = false;
        else
            gravity.enabled = true;
    }
}
