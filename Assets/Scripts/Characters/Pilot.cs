using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class Pilot : MonoBehaviour, IValueChangeEventsComponent
{
    public Body body;
    [HideInInspector] public BodyPart[] transferPartsToVehicle;
        
    public Vehicle CurrentVehicle { get; private set; }
    public Body PilotedBody { get => IsPilotingVehicle.GetValue<bool>() ? CurrentVehicle : body; }
    
    public ValueChangeEvent IsPilotingVehicle = ValueChangeEvent.New<bool>();

    public int GetValueChangeEvents(out ValueChangeEvent[] vces)
    {
        vces = new ValueChangeEvent[] { IsPilotingVehicle };
        return vces.Length;
    }

    public int SetValueChangeEventsID()
    {
        IsPilotingVehicle.SetID("IsPilotingVehicle", this, 0);
        return 1;
    }

    public void EnslaveValueChangeEvents(bool enslave)
    {
        IsPilotingVehicle.Enslave(enslave);
    }

    private void Start()
    {
        if (body != null)
            body.IsDead.AddListener<bool>(OnDeath);

        if (transform.parent != null)
        {
            Vehicle startVehicle = transform.parent.GetComponent<Vehicle>();
            if (startVehicle != null) EnterVehicle(startVehicle);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsPilotingVehicle.GetValue<bool>() == false)
        {
            Vehicle vehicle = collision.gameObject.GetComponent<Vehicle>();
            if (vehicle != null) EnterVehicle(vehicle);
        }
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
        vehicle.IsFull.AddListener<bool>(OnVehicleIsFullChange);
        vehicle.IsDead.AddListener<bool>(OnVehicleDestruction);

        foreach (BodyPart part in transferPartsToVehicle)
            part.AttachedBody = vehicle;

        CurrentVehicle = vehicle;
        IsPilotingVehicle.SetValue(true);
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
        if (IsPilotingVehicle.GetValue<bool>() == true && CurrentVehicle != null)
        {
            //exitingVehicle.Stop();

            if (immediate == false)
                yield return new WaitForSeconds(CurrentVehicle.exitAnimationDelay);

            if (body != null)
            {
                if (body is PhysicalBody)
                {
                    PhysicalBody pBody = body as PhysicalBody;
                    pBody.Simulate = true;
                    pBody.Move = true;
                }

                this.transform.SetParent(body.transform);
                body.transform.SetParent(CurrentVehicle.transform.parent);
                body.transform.position = CurrentVehicle.exit.position;

                if (body.AttachedRigidBody != null)
                    body.AttachedRigidBody.AddForce(CurrentVehicle.exitForce * CurrentVehicle.exit.up);
            }

            CurrentVehicle.IsFull.RemoveListener<bool>(OnVehicleIsFullChange);
            CurrentVehicle.IsDead.RemoveListener<bool>(OnVehicleDestruction);
            CurrentVehicle.SetBodyInside(null);

            foreach (BodyPart part in transferPartsToVehicle)
                part.AttachedBody = body;

            CurrentVehicle = null;
            IsPilotingVehicle.SetValue(false);
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
        if (IsPilotingVehicle.GetValue<bool>() == true && CurrentVehicle != null)
        {
            //current.Stop();
            if (body != null)
                body.transform.SetParent(CurrentVehicle.transform.parent);

            CurrentVehicle.SetBodyInside(null);
            CurrentVehicle = null;
            IsPilotingVehicle.SetValue(false);
        }
    }
}
