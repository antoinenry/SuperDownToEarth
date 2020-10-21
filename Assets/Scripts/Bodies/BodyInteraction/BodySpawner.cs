using System;
using System.Collections;
using UnityEngine;
using Scarblab.VCE;

public class BodySpawner : MonoBehaviour
{
    public Body unitPrefab;
    [Min(-1)] public int magazineSize;
    [Min(0)] public int maxUnits;
    public float spawnVelocity;
    public float spawnDelay = 1f;
    public Transform[] spawnPoints;

    public Trigger spawnUnit;
    public BoolChangeEvent isSpawning;
    public IntChangeEvent magazineCount;

    public int CurrentSpawnedUnitCount { get; private set; }

    private Body[] units;
    private Coroutine currentSpawnCoroutine;

    private void Awake()
    {        
        ResizeUnitArray();        
    }

    private void Start()
    {
        if (magazineSize >= 0)
            magazineCount.Value = magazineSize;
        else
            magazineCount.Value = int.MaxValue;

        currentSpawnCoroutine = StartCoroutine(SpawnCoroutine());
    }

    private void OnEnable()
    {
        spawnUnit.AddTriggerListener(OnSpawnUnit);
        currentSpawnCoroutine = StartCoroutine(SpawnCoroutine());
    }

    private void OnDisable()
    {
        spawnUnit.RemoveTriggerListener(OnSpawnUnit);
        StopAllCoroutines();
        currentSpawnCoroutine = null;
    }

    public void SpawnUnit()
    {
        spawnUnit.Trigger();
    }

    private void OnSpawnUnit()
    {
        if (units == null || units.Length != maxUnits)
            ResizeUnitArray();

        if (magazineCount <= 0) return;

        Transform spawnPoint;
        int spawnPointCount = spawnPoints != null ? spawnPoints.Length : 0;
        spawnPoint = spawnPointCount > 0 ? spawnPoints[UnityEngine.Random.Range(0, spawnPointCount)] : transform;

        for (int i = 0; i < maxUnits; i++)
        {
            Body spawnedUnit = null;

            if (units[i] == null)
            {
                spawnedUnit = Instantiate(unitPrefab, spawnPoint.position, spawnPoint.rotation, transform);
                ValueChangeEvent.InitializeVCEs(spawnedUnit.gameObject);
                spawnedUnit.destroyBody.AddTriggerListener(OnUnitDeath);
                units[i] = spawnedUnit;
            }
            else if (units[i].IsAlive == false)
            {
                spawnedUnit = units[i];
                spawnedUnit.transform.position = spawnPoint.position;
                spawnedUnit.transform.rotation = spawnPoint.rotation;
                spawnedUnit.Respawn();
            }

            if (spawnedUnit != null)
            {
                CurrentSpawnedUnitCount += 1;
                if (magazineSize >= 0)
                    magazineCount.Value = magazineCount - 1;

                if (spawnedUnit is PhysicalBody)
                {
                    PhysicalBody physicalUnit = spawnedUnit as PhysicalBody;
                    physicalUnit.AttachedRigidBody.velocity = transform.up * spawnVelocity;
                    physicalUnit.AttachedRigidBody.angularVelocity = 0f;
                }

                break;
            }
        }
    }

    private void OnUnitDeath()
    {
        CurrentSpawnedUnitCount -= 1;
        if (currentSpawnCoroutine == null && magazineCount > 0)
            currentSpawnCoroutine = StartCoroutine(SpawnCoroutine());
    }

    private IEnumerator SpawnCoroutine()
    {
        while (magazineCount > 0 && CurrentSpawnedUnitCount < maxUnits)
        {
            isSpawning.Value = true;
            yield return new WaitForSeconds(spawnDelay);
            SpawnUnit();
        }

        isSpawning.Value = false;
        currentSpawnCoroutine = null;
    }

    private void ResizeUnitArray()
    {
        int arrayLength = units != null ? units.Length : 0;

        if (arrayLength > maxUnits)
        {
            for (int i = arrayLength; i < maxUnits; i++)
                Destroy(units[i].gameObject);
        }

        Array.Resize(ref units, maxUnits);
    }
}
