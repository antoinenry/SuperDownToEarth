using System.Collections;
using UnityEngine;

public class Pilot : MonoBehaviour
{
    public Body body;
    [HideInInspector] public BodyPart[] transferPartsToVehicle;

    public ObjectChangeEvent PilotedBody;    

    private void Start()
    {
        if (body != null)
        {
            body.IsDead.AddValueListener<bool>(OnDeath);
            PilotedBody.Value = body;
        }

        if (transform.parent != null)
        {
            Vehicle startVehicle = transform.parent.GetComponent<Vehicle>();
            if (startVehicle != null) EnterVehicle(startVehicle);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GetCurrentVehicle() == null)
        {
            Vehicle vehicle = collision.gameObject.GetComponent<Vehicle>();
            if (vehicle != null) EnterVehicle(vehicle);
        }
    }

    public Vehicle GetCurrentVehicle()
    {
        Body pilotedBody = PilotedBody.Value as Body;
        if (pilotedBody != null && pilotedBody != body && pilotedBody is Vehicle)
            return pilotedBody as Vehicle;
        else
            return null;
    }

    public void EnterVehicle (Vehicle vehicle)
    {
        if (vehicle.BodyInside != null)
            return;

        if (body != null)
        {
            body.transform.SetParent(vehicle.seat);
            body.transform.SetPositionAndRotation(vehicle.seat.position, vehicle.seat.rotation);
            body.transform.localScale = vehicle.seat.localScale;
            this.transform.SetParent(vehicle.seat);

            if (body is PhysicalBody)
            {
                PhysicalBody pBody = body as PhysicalBody;
                pBody.Simulate = false;
                pBody.Move = false;
            }
        }

        vehicle.SetBodyInside(body);
        vehicle.IsFull.AddValueListener<bool>(OnVehicleIsFullChange);
        vehicle.IsDead.AddValueListener<bool>(OnVehicleDestruction);

        foreach (BodyPart part in transferPartsToVehicle)
            part.AttachedBody = vehicle;

        PilotedBody.Value = vehicle;
    }

    public void ExitCurrentVehicle()
    {
        StartCoroutine(ExitVehicleCoroutine(false));
    }

    public void ExitCurrentVehicleImmediate()
    {
        StartCoroutine(ExitVehicleCoroutine(true));
    }

    private IEnumerator ExitVehicleCoroutine(bool immediate)
    {
        Vehicle currentVehicle = GetCurrentVehicle();

        if (GetCurrentVehicle() != null)
        {
            //exitingVehicle.Stop();

            if (immediate == false)
                yield return new WaitForSeconds(currentVehicle.exitAnimationDelay);

            if (body != null)
            {
                if (body is PhysicalBody)
                {
                    PhysicalBody pBody = body as PhysicalBody;
                    pBody.Simulate = true;
                    pBody.Move = true;
                }

                this.transform.SetParent(body.transform);
                body.transform.SetParent(currentVehicle.transform.parent);
                body.transform.position = currentVehicle.exit.position;

                if (body.AttachedRigidBody != null)
                    body.AttachedRigidBody.AddForce(currentVehicle.exitForce * currentVehicle.exit.up);
            }

            currentVehicle.IsFull.RemoveValueListener<bool>(OnVehicleIsFullChange);
            currentVehicle.IsDead.RemoveValueListener<bool>(OnVehicleDestruction);
            currentVehicle.SetBodyInside(null);

            foreach (BodyPart part in transferPartsToVehicle)
                part.AttachedBody = body;
            
            PilotedBody.Value = body;
        }
    }

    private void OnVehicleIsFullChange(bool vehicleIsFull)
    {
        if (vehicleIsFull == false) ExitCurrentVehicle();
    }

    private void OnVehicleDestruction(bool vehicleIsDestroyed)
    {
        if(vehicleIsDestroyed)
        {
            ExitCurrentVehicleImmediate();
            DamageStatus status = GetComponent<DamageStatus>();
            if (status != null) status.GetDamaged();
        }
    }

    private void OnDeath(bool isDead)
    {
        if (isDead) ExitCurrentVehicle();
    }

    private void Die()
    {
        Vehicle currentVehicle = GetCurrentVehicle();

        if (currentVehicle != null)
        {
            //current.Stop();
            if (body != null)
                body.transform.SetParent(currentVehicle.transform.parent);

            currentVehicle.SetBodyInside(null);
            PilotedBody.Value = body;
        }
    }
}
