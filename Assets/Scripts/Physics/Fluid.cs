using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Fluid : MonoBehaviour
{
    public struct ShapeVertex
    {
        public int shapeIndex;
        public int vertexIndex;
    }

    public class FluidPoint
    {
        public ShapeVertex[] vertices;
        public Vector3 startPosition;
        public Vector3 perturbation;
    }

    [Header("Fluid movements")]
    public float ondulationRadius = .5f;
    public float ondulationSpeed = 20f;
    public float angularIncrement = 60f;

    private LowPolyShape[] shapes;
    private FluidPoint[] points;
    private List<Rigidbody2D> immergedBodies;

    private void Awake()
    {
        immergedBodies = new List<Rigidbody2D>();
    }

    private void Start()
    {
        InitShapes();
        InitPoints();
    }

    private void FixedUpdate()
    {
        MakeFluidMove();
    }

    private void InitShapes()
    {
        Component[] shapeComponents = LowPolyShape.GetShapeComponentsInGameObject(gameObject, true);
        if (shapeComponents == null || shapeComponents.Length == 0)
        {
            shapes = new LowPolyShape[0];
            points = new FluidPoint[0];
            return;
        }

        int shapeCount = shapeComponents.Length;
        shapes = new LowPolyShape[shapeCount];

        for (int i = 0; i < shapeCount; i++)
            shapes[i] = LowPolyShape.New(shapeComponents[i]);
    }

    private void InitPoints()
    {
        List<FluidPoint> pointList = new List<FluidPoint>();

        foreach (LowPolyShape shape in shapes)
        {
            foreach (Vector3 pos in shape.GetPositions())
            {
                if (pointList.FindIndex(pt => pt.startPosition == pos) == -1)
                {
                    FluidPoint pt = InitPoint(pos);
                    if (pt != null)
                        pointList.Add(pt);
                }
            }
        }

        points = pointList.ToArray();
    }

    private FluidPoint InitPoint(Vector3 pointPos)
    {
        if (shapes == null) return null;

        List<ShapeVertex> shapeVertices = new List<ShapeVertex>();

        for(int si = 0; si < shapes.Length; si++)
        {
            Vector3[] shapePositions = shapes[si].GetPositions();

            for(int vi = 0; vi < shapePositions.Length; vi++)
            {
                if (shapePositions[vi] == pointPos)
                {
                    shapeVertices.Add(new ShapeVertex() { shapeIndex = si, vertexIndex = vi });
                }
            }
        }

        if (shapeVertices.Count == 0)
            return null;
        else
            return new FluidPoint()
            {
                vertices = shapeVertices.ToArray(),
                startPosition = pointPos,
            };
    }

    private void MovePoint(FluidPoint pt, Vector3 toPos)
    {
        foreach(ShapeVertex sv in pt.vertices)
            shapes[sv.shapeIndex].SetPosition(sv.vertexIndex, toPos);
    }

    private void MakeFluidMove()
    {
        float time = Time.fixedTime;
        float angleOffset = 0f;

        foreach (FluidPoint fluidPoint in points)
        {
            MovePoint(fluidPoint, fluidPoint.startPosition + fluidPoint.perturbation + Quaternion.Euler(0f, 0f, angleOffset + ondulationSpeed * time) * new Vector3(ondulationRadius, 0f, 0f));
            angleOffset += angularIncrement;
        }

        foreach (LowPolyShape shape in shapes)
            shape.UpdateShape();
    }
}