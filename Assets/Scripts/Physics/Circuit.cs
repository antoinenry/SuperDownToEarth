using UnityEngine;

public class Circuit : MonoBehaviour
{
    public enum CycleType { Single, Loop, PingPong }

    public Vector2[] steps = new Vector2[0];
    [Range(0, 5)] public int numCornerVertices;
    [Min(0f)] public float cornerRadius;
    public CycleType cycleType;
    
    [HideInInspector] [SerializeField]
    private Vector2[] trajectory = new Vector2[0];

    private void Awake()
    {
        UpdateTrajectory();
    }
        
    public void OnDrawGizmosSelected()
    {
        Vector2 circuitPosition = transform.position;

        int length = steps.Length;
        if (length < 2) return;
        Gizmos.color = Color.gray;
        for (int i = 0; i < length; i++)
            Gizmos.DrawLine(circuitPosition, circuitPosition + steps[i]);

        length = trajectory.Length;
        if (length < 2) return;
        Gizmos.color = Color.magenta;
        for (int i = 0; i < length - 1; i++)
            Gizmos.DrawLine(circuitPosition + trajectory[i], circuitPosition + trajectory[i+1]);
        if (cycleType == CycleType.Loop) Gizmos.DrawLine(circuitPosition + trajectory[length - 1], circuitPosition + trajectory[0]);
    }

    public Vector2 GetStepPosition(int index)
    {
        if (steps == null || index < 0 || index >= steps.Length) return transform.position;
        else return (Vector2)transform.position + steps[index];
    }

    public int GetCorrectStepIndex(int index)
    {
        return GetCorrectIndex(index, ref steps);
    }
    
    private int GetCorrectIndex(int index, ref Vector2[] points)
    {
        int correctIndex = -1;
        if (points != null && points.Length != 0)
        {
            if (points.Length == 1)
            {
                correctIndex = 0;
            }
            else
            {
                switch (cycleType)
                {
                    case CycleType.Single:
                        correctIndex = Mathf.Clamp(index, 0, points.Length - 1);
                        break;
                    case CycleType.Loop:
                        correctIndex = (int)Mathf.Repeat(index, points.Length);
                        break;
                    case CycleType.PingPong:
                        correctIndex = (int)Mathf.PingPong(index, points.Length - 1);
                        break;
                }
            }
        }

        return correctIndex;
    }    

    public void UpdateTrajectory()
    {
        trajectory = ChaikinSmoothLine.Smoothen(steps, cycleType == CycleType.Loop, cornerRadius, numCornerVertices);
    }
}