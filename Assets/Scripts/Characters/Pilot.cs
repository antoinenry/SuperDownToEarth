using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class Pilot : MonoBehaviour, IEventsHubElement
{
    public Body body;
    [HideInInspector] public BodyPart[] transferPartsToVehicle;
    
    public enum ExposedEvents { isPilotingVehicle }
    public ValueChangeEvent<bool> IsPilotingVehicle = new ValueChangeEvent<bool>();
    public Vehicle CurrentVehicle { get; private set; }
    public Body PilotedBody { get => IsPilotingVehicle.Value ? CurrentVehicle : body; }

    private UnityAction<bool> DeadBodyAction;

    private void Awake()
    {
        DeadBodyAction = new UnityAction<bool>(dead => ExitCurrentVehicle());
    }

    private void Start()
    {
        if (body != null)
            body.IsDead.AddListener(DeadBodyAction);

        if (transform.parent != null)
        {
            Vehicle startVehicle = transform.parent.GetComponent<Vehicle>();
            if (startVehicle != null) EnterVehicle(startVehicle);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsPilotingVehicle.Value == false)
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
        vehicle.IsFull.AddListener(OnVehicleIsFullChange);
        vehicle.IsDead.AddListener(OnVehicleDestruction);

        foreach (BodyPart part in transferPartsToVehicle)
            part.AttachedBody = vehicle;

        CurrentVehicle = vehicle;
        IsPilotingVehicle.Value = true;
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
        if (IsPilotingVehicle.Value == true && CurrentVehicle != null)
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

            CurrentVehicle.IsFull.RemoveListener(OnVehicleIsFullChange);
            CurrentVehicle.IsDead.RemoveListener(OnVehicleDestruction);
            CurrentVehicle.SetBodyInside(null);

            foreach (BodyPart part in transferPartsToVehicle)
                part.AttachedBody = body;

            CurrentVehicle = null;
            IsPilotingVehicle.Value = false;
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

    private void Die()
    {
        if (IsPilotingVehicle.Value == true && CurrentVehicle != null)
        {
            //current.Stop();
            if (body != null)
                body.transform.SetParent(CurrentVehicle.transform.parent);

            CurrentVehicle.SetBodyInside(null);
            CurrentVehicle = null;
            IsPilotingVehicle.Value = false;
        }
    }

    public bool GetValueChangeEvent(int index, out IValueChangeEvent iValueChangeEvent)
    {
        switch (index)
        {
            case (int)ExposedEvents.isPilotingVehicle:
                iValueChangeEvent = IsPilotingVehicle;
                return true;
        }

        iValueChangeEvent = null;
        return false;
    }

    public void GetValueChangeEventsNamesAndTypes(out string[] names, out Type[] types)
    {
        names = Enum.GetNames(typeof(ExposedEvents));
        types = new Type[] { typeof(bool) };
    }

    public int GetValueChangeEventIndex(string vceName)
    {
        List<string> names = new List<string>(Enum.GetNames(typeof(ExposedEvents)));
        return names.IndexOf(vceName);
    }
}
