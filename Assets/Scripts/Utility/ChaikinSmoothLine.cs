using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaikinSmoothLine
{
    static public Vector2[] Smoothen(Vector2[] positions, bool loop)
    {
        List<Vector2> newPositions = new List<Vector2>();
        for (int i = 0, imax = loop ? positions.Length : positions.Length - 1; i < imax; i++)
        {
            Vector2[] sub = DivideAngle(positions[i], positions[(i + 1) % imax], positions[(i + 2) % imax]);
            newPositions.AddRange(sub);
        }
        return newPositions.ToArray();
    }

    static public Vector2[] Smoothen(Vector2[] positions, bool loop, int numIterations)
    {
        if (numIterations < 1) return positions;

        Vector2[] newPositions = positions;
        for (int i = 0; i < numIterations; i++)
            newPositions = ChaikinSmoothLine.Smoothen(newPositions, loop);

        return newPositions;
    }

    static public Vector2[] Smoothen(Vector2[] positions, bool loop, float cornerRadius, int numIterations = 1)
    {
        if(cornerRadius <= 0f) return ChaikinSmoothLine.Smoothen(positions, loop, numIterations);

        List<Vector2> newPositions = new List<Vector2>();
        for (int i = 0, imax = loop ? positions.Length : positions.Length - 1; i < imax; i++)
        {
            Vector2 current = positions[i];
            Vector2 next = positions[(i+1) % imax];
            float segmentLength = Vector2.Distance(current, next);

            newPositions.Add(current);

            if(segmentLength > 2f * cornerRadius)
            {
                Vector2 segmentDirection = (next - current).normalized;
                newPositions.Add(current + cornerRadius * segmentDirection);
                newPositions.Add(next - cornerRadius * segmentDirection);
            }
            else if (segmentLength > cornerRadius)
            {
                newPositions.Add((current + next) / 2f);
            }
        }

        return ChaikinSmoothLine.Smoothen(newPositions.ToArray(), loop, numIterations);
    }

    static public Vector2[] DivideAngle(Vector2 A, Vector2 B, Vector2 C)
    {
        if (A == B || B == C || C == A) return new Vector2[] { B };


        if (Mathf.Pow(Vector2.Dot(A-B, A-C), 2f) == Vector2.SqrMagnitude(A-B) * Vector2.SqrMagnitude(A-C))
            return new Vector2[] { B };

        Vector2[] output = new Vector2[2];
        output[0] = A / 4f + 3f * B / 4f;
        output[1] = 3f * B / 4f + C / 4f;

        return output;
    }
}