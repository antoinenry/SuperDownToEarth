using System;
using UnityEngine;
using Scarblab.VCE;

public class BodySpawner : MonoBehaviour
{
    public Body unitPrefab;
    [Min(0f)] public int capacity;
    [Min(0f)] public int maxUnits;

    public Trigger spawnUnit;

    private Body[] units;

    private void Awake()
    {
        ResizeUnitArray();
    }

    private void OnEnable()
    {
        spawnUnit.AddTriggerListener(OnSpawnUnit);
    }

    private void OnDisable()
    {
        spawnUnit.RemoveTriggerListener(OnSpawnUnit);
    }

    private void OnSpawnUnit()
    {
        if (units == null || units.Length != maxUnits)
            ResizeUnitArray();

        for (int i = 0; i < maxUnits; i++)
        {            
            if (units[i] == null)
            {
                units[i] = Instantiate(unitPrefab, transform, false);
                ValueChangeEvent.InitializeVCEs(units[i].gameObject);
                break;
            }

            if (units[i].IsAlive == false)
            {
                units[i].transform.position = transform.position;
                units[i].Respawn();
                break;
            }
        }
    }

    private void ResizeUnitArray()
    {
        Array.Resize(ref units, maxUnits);
    }
}
