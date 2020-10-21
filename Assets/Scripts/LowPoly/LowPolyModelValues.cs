using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LowPolyModelValues
{
    #region Values operations
    public static bool Equality(Component componentA, Component componentB)
    {
        if (componentA is LineRenderer && componentB is LineRenderer)
        {
            LineRendererValues valuesA = new LineRendererValues(componentA as LineRenderer);
            LineRendererValues valuesB = new LineRendererValues(componentB as LineRenderer);
            return valuesA.Equals(valuesB);
        }

        if (componentA is WaveLine && componentB is WaveLine)
        {
            WaveLineValues valuesA = new WaveLineValues(componentA as WaveLine);
            WaveLineValues valuesB = new WaveLineValues(componentB as WaveLine);
            return valuesA.Equals(valuesB);
        }

        return false;
    }

    public static void CopyFromTo(Component fromComponent, Component toComponent)
    {
        if (toComponent is LineRenderer && fromComponent is LineRenderer)
            new LineRendererValues(fromComponent as LineRenderer).CopyToComponent(toComponent as LineRenderer);

        else if (toComponent is WaveLine && fromComponent is WaveLine)
            new WaveLineValues(fromComponent as WaveLine).CopyToComponent(toComponent as WaveLine);
    }
    #endregion


    #region Values types
    public struct LineRendererValues
    {
        public bool loop;
        public AnimationCurve widthCurve;
        public Gradient colorGradient;
        public int numCornerVertices;
        public int numCapVertices;
        public LineAlignment alignment;
        public LineTextureMode textureMode;

        public LineRendererValues(LineRenderer lr)
        {
            loop = lr.loop;
            widthCurve = lr.widthCurve;
            colorGradient = lr.colorGradient;
            numCornerVertices = lr.numCornerVertices;
            numCapVertices = lr.numCapVertices;
            alignment = lr.alignment;
            textureMode = lr.textureMode;
        }

        public void CopyToComponent(LineRenderer lr)
        {
            lr.loop = loop;
            lr.widthCurve = widthCurve;
            lr.colorGradient = colorGradient;
            lr.numCornerVertices = numCornerVertices;
            lr.numCapVertices = numCapVertices;
            lr.alignment = alignment;
            lr.textureMode = textureMode;
        }
    }

    public struct MeshRendererValues
    {

    }

    public struct PolygonCollider2DValues
    {

    }

    public struct LightValues
    {

    }

    public struct FluidValues
    {

    }

    public struct WaveLineValues
    {
        public float waveWidth;
        public float waveAmplitude;
        public float waveFrequency;
        public int waveCount;

        public WaveLineValues(WaveLine wl)
        {
            waveWidth = wl.waveWidth;
            waveAmplitude = wl.waveAmplitude;
            waveFrequency = wl.waveFrequency;
            waveCount = wl.waveCount;
        }

        public void CopyToComponent(WaveLine wl)
        {
            wl.waveWidth = waveWidth;
            wl.waveAmplitude = waveAmplitude;
            wl.waveFrequency = waveFrequency;
            wl.waveCount = waveCount;
        }
    }
    #endregion
}

