using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaussianDistribution : MonoBehaviour
{
    
    public static float NormInv(float probability, float mean, float sigma)
    {
        float x = NormInv(probability);
        return sigma * x + mean;
    }
    
    public static float NormInv(float probability)
    {
        float q = 0f;
        float r = 0f;
        float x = 0f;
        
        float[] a = new float[] { -3.969683028665376e+01f, 2.209460984245205e+02f, -2.759285104469687e+02f, 1.383577518672690e+02f, -3.066479806614716e+01f, 2.506628277459239e+00f };
        float[] b = new float[] { -5.447609879822406e+01f, 1.615858368580409e+02f, -1.556989798598866e+02f, 6.680131188771972e+01f, -1.328068155288572e+01f };
        float[] c = new float[] { -7.784894002430293e-03f, -3.223964580411365e-01f, -2.400758277161838e+00f, -2.549732539343734e+00f, 4.374664141464968e+00f, 2.938163982698783e+00f };
        float[] d = new float[] { 7.784695709041462e-03f, 3.224671290700398e-01f, 2.445134137142996e+00f, 3.754408661907416e+00f };
        
        float pLow = 0.02425f;
        float pHigh = 1f - pLow;
        

        if (probability <= 0f)
        {
            probability = Mathf.Epsilon;
        }
        else if (probability >= 1f)
        {
            probability = 1f - Mathf.Epsilon;
        }
        
        if (probability < pLow)
        {
            q = Mathf.Sqrt(-2f * Mathf.Log(probability));
            x = (((((c[0] * q + c[1]) * q + c[2]) * q + c[3]) * q + c[4]) * q + c[5]) / ((((d[0] * q + d[1]) * q + d[2]) * q + d[3]) * q + 1f);
        }
        
        if (pLow <= probability && probability <= pHigh)
        {
            q = probability - 0.5f;
            r = q * q;
            x = (((((a[0] * r + a[1]) * r + a[2]) * r + a[3]) * r + a[4]) * r + a[5]) * q / (((((b[0] * r + b[1]) * r + b[2]) * r + b[3]) * r + b[4]) * r + 1f);
        }
        
        if (pHigh < probability)
        {
            q = Mathf.Sqrt(-2 * Mathf.Log(1f - probability));
            x = -(((((c[0] * q + c[1]) * q + c[2]) * q + c[3]) * q + c[4]) * q + c[5]) / ((((d[0] * q + d[1]) * q + d[2]) * q + d[3]) * q + 1f);
        }

        return x;
    }
}