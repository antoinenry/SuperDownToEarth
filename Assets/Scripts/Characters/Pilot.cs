using System.Collections;
using UnityEngine;

public class Pilot : MonoBehaviour
{
    public Body body;
    public bool enterVehicleOnCollision = true;
    //[HideInInspector] public BodyPart[] transferPartsToVehicle;

    public BoolChangeEvent isPilotingVehicle;
    public ComponentChangeEvent currentVehicle;

    private void Start()
    {
        if (body != null)
        {
            body.destroyBody.AddTriggerListener(OnDeath);
            currentVehicle.Value = body;
            isPilotingVehicle.Value = false;
        }

        if (transform.parent != null)
        {
            Vehicle startVehicle = transform.parent.GetComponent<Vehicle>();
            if (startVehicle != null) EnterVehicle(startVehicle);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (enterVehicleOnCollision && isPilotingVehicle == false)
        {
            Vehicle vehicle = collision.gameObject.GetComponent<Vehicle>();
            if (vehicle != null) EnterVehicle(vehicle);
        }
    }

    public void EnterVehicle (Vehicle vehicle)
    {
        if (vehicle.IsFull == true)
            return;

        if (body != null)
        {
            body.transform.SetParent(vehicle.seat);
            body.transform.SetPositionAndRotation(vehicle.seat.position, vehicle.seat.rotation);
            body.transform.localScale = Vector3.one;
            //this.transform.SetParent(vehicle.seat);

            if (body is PhysicalBody)
            {
                PhysicalBody pBody = body as PhysicalBody;
                pBody.Simulate = false;
                pBody.Move = false;
            }
        }

        vehicle.SetBodyInside(body);
        vehicle.IsFull.AddValueListener<bool>(OnVehicleIsFullChange);
        vehicle.destroyBody.AddTriggerListener(OnVehicleDestruction);
        
        //foreach (BodyPart part in transferPartsToVehicle)
        //    part.AttachedBody = vehicle;

        currentVehicle.Value = vehicle;
        isPilotingVehicle.Value = true;
    }

    public void ExitCurrentVehicle(bool immediate = false)
    {
        StartCoroutine(ExitVehicleCoroutine(immediate));
    }

    private IEnumerator ExitVehicleCoroutine(bool immediate)
    {
        if (isPilotingVehicle == true)
        {
            //exitingVehicle.Stop();

            Vehicle exitVehicle = currentVehicle.Value as Vehicle;
            if (immediate == false)
                yield return new WaitForSeconds(exitVehicle.exitAnimationDelay);

            if (body != null)
            {
                if (body is PhysicalBody)
                {
                    PhysicalBody pBody = body as PhysicalBody;
                    pBody.Simulate = true;
                    pBody.Move = true;
                }

                //this.transform.SetParent(body.transform);
                body.transform.SetParent(exitVehicle.transform.parent);
                body.transform.position = exitVehicle.exit.position;

                if (body.AttachedRigidBody != null)
                    body.AttachedRigidBody.AddForce(exitVehicle.exitForce * exitVehicle.exit.up);
            }

            exitVehicle.IsFull.RemoveValueListener<bool>(OnVehicleIsFullChange);
            exitVehicle.destroyBody.RemoveTriggerListener(OnVehicleDestruction);
            exitVehicle.SetBodyInside(null);

            //foreach (BodyPart part in transferPartsToVehicle)
            //    part.AttachedBody = body;
            
            currentVehicle.Value = body;
            isPilotingVehicle.Value = false;
        }
    }

    private void OnVehicleIsFullChange(bool vehicleIsFull)
    {
        if (vehicleIsFull == false) ExitCurrentVehicle();
    }

    private void OnVehicleDestruction()
    {
        StartCoroutine(ExitVehicleCoroutine(true));
    }

    private void OnDeath()
    {
        ExitCurrentVehicle(true);
    }

    private void Die()
    {
        Vehicle vehicle = currentVehicle.Value as Vehicle;

        if (vehicle != null)
        {
            //current.Stop();
            if (body != null)
                body.transform.SetParent(vehicle.transform.parent);

            vehicle.SetBodyInside(null);
            currentVehicle.Value = body;
            isPilotingVehicle.Value = false;
        }
    }
}
