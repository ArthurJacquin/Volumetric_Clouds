using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class D3Brush : EditorWindow
{

    private Texture2D brush2D;
    private float gradientThreshold;
    private float gradientCoeff;

    [MenuItem("Cloud Drawing/Create New Brush")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        D3Brush window = (D3Brush)EditorWindow.GetWindow(typeof(D3Brush));
        window.Show();
    }

    public void create3DTextureFromBrush(Texture2D brush2D, float gradientCoeff, float gradientThreshold)
    {
        // Configure the texture
        int size = brush2D.width;
        TextureFormat format = TextureFormat.RGBA32;
        TextureWrapMode wrapMode = TextureWrapMode.Clamp;

        // Create the texture and apply the configuration
        Texture3D texture = new Texture3D(size, size, size, format, false);
        texture.wrapMode = wrapMode;

        // Create a 3-dimensional array to store color data
        Color[] colors = new Color[size * size * size];
        Vector3 middlePixel = new Vector3(size / 2, size / 2, size / 2);

        //1 : commencement de l'algo au milieu de la texture3D
        //ajout des pixels dans la partie haute de la texture (z / 2)
        for (int z = size / 2; z < size; z++)
        {
            int zOffset = z * size * size;
            for (int y = 0; y < size; y++)
            {
                int yOffset = y * size;
                for (int x = 0; x < size; x++)
                {
                    float alpha = brush2D.GetPixel(x, y).r;

                    alpha /= Mathf.Abs(Vector3.Distance(middlePixel, new Vector3(x, y, z))) * gradientCoeff;
                    if (alpha < gradientThreshold)
                        alpha = 0.0f;

                    Color col = new Color(1.0f, 1.0f, 1.0f, alpha);
                    colors[x + yOffset + zOffset] = col;
                }
            }
        }

        //Deuxieme moitié
        for (int z = size / 2 - 1; z >= 0; z--)
        {
            int zOffset = z * size * size;
            for (int y = 0; y < size; y++)
            {
                int yOffset = y * size;
                for (int x = 0; x < size; x++)
                {
                    float alpha = brush2D.GetPixel(x, y).r;

                    if(gradientCoeff != 0)
                        alpha /= Mathf.Abs(Vector3.Distance(middlePixel, new Vector3(x, y, z))) * gradientCoeff;

                    if (alpha < gradientThreshold)
                        alpha = 0.0f;

                    Color col = new Color(1.0f, 1.0f, 1.0f, alpha);
                    colors[x + yOffset + zOffset] = col;
                }
            }
        }

        // Copy the color values to the texture
        texture.SetPixels(colors);

        // Apply the changes to the texture and upload the updated texture to the GPU
        texture.Apply();

        // Save the texture to your Unity Project
        AssetDatabase.CreateAsset(texture, "Assets/Textures/" + brush2D.name + ".asset");
    }


    private void OnGUI()
    {
        EditorStyles.objectField.alignment = TextAnchor.MiddleRight;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Base texture", EditorStyles.boldLabel);
        brush2D = (Texture2D)EditorGUILayout.ObjectField(brush2D, typeof(Texture2D), false, GUILayout.Width(70), GUILayout.Height(70));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.PrefixLabel("Parameters", EditorStyles.boldLabel);
        gradientCoeff = EditorGUILayout.Slider("Gradient Coefficient", gradientCoeff, 0, 5);
        gradientThreshold = EditorGUILayout.Slider("Gradient Threshold", gradientThreshold, 0, 1);

        if (GUILayout.Button("Generate 3D brush !"))
        {
            create3DTextureFromBrush(brush2D, gradientCoeff, gradientThreshold);
        }
    }
}
