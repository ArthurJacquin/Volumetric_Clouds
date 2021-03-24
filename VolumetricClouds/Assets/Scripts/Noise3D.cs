using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise3D
{
    public static float PerlinNoise3D(float x, float y, float z)
    {
        float ab = Mathf.PerlinNoise(x, y);
        float bc = Mathf.PerlinNoise(y, z);
        float ac = Mathf.PerlinNoise(x, z);

        float ba = Mathf.PerlinNoise(y, x);
        float cb = Mathf.PerlinNoise(z, y);
        float ca = Mathf.PerlinNoise(z, x);

        float abc = ab + bc + ac + cb + ca;
        return abc / 6.0f;

    }

   /* public static float Cloud()
    { 
        
    }*/

}
