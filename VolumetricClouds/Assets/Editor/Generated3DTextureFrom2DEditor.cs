using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class Generated3DTextureFrom2DEditor : EditorWindow
{
    Texture2D tex;
    string textureName = "Cloud";
    Vector3Int dims = new Vector3Int(128, 128, 256);
    Vector2Int nbImage = new Vector2Int(16, 16);

    // Add menu named "My Window" to the Window menu
    [MenuItem("Cloud Drawing/Generate Textures/From 2D texture")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        Generated3DTextureFrom2DEditor window = (Generated3DTextureFrom2DEditor)EditorWindow.GetWindow(typeof(Generated3DTextureFrom2DEditor));
        window.Show();
    }

    void OnGUI()
    {
        EditorStyles.objectField.alignment = TextAnchor.MiddleRight;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Base texture", EditorStyles.boldLabel);
        tex = (Texture2D)EditorGUILayout.ObjectField(tex, typeof(Texture2D), false, GUILayout.Width(70), GUILayout.Height(70));
        EditorGUILayout.EndHorizontal();

        dims = EditorGUILayout.Vector3IntField("3D texture dimensions", dims);
        nbImage = EditorGUILayout.Vector2IntField("Number of images", nbImage);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Texture name", EditorStyles.boldLabel);
        textureName = EditorGUILayout.TextField(textureName);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Generate 3D texture !"))
        {
            Create3DTexture(tex, dims, nbImage ,textureName);
        }
    }

    static void Create3DTexture(Texture2D tex, Vector3Int dims, Vector2Int nbImage, string name)
    {
        // Configure the texture
        TextureFormat format = TextureFormat.RGBA32;
        TextureWrapMode wrapMode = TextureWrapMode.Clamp;

        // Create the texture and apply the configuration
        Texture3D texture = new Texture3D(dims[0], dims[1], dims[2], format, false);
        texture.wrapMode = wrapMode;

        // Create a 3-dimensional array to store color data
        Color[] colors = new Color[dims[0] * dims[1] * dims[2]];

        // Populate the array so that the x, y, and z values of the texture will map to red, blue, and green colors
        for (int z = 0; z < dims[2]; z++)
        {
            int zOffset = z * dims[0] * dims[1];
            for (int y = 0; y < dims[1]; y++)
            {
                int yOffset = y * dims[0];
                for (int x = 0; x < dims[0]; x++)
                {
                    int texX = x + z * dims.x % tex.width;
                    int texY = tex.height - (y + (int)(z / nbImage.x) * dims.y);
                    float alpha = tex.GetPixel(texX, texY).r;
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
        AssetDatabase.CreateAsset(texture, "Assets/Textures/" + name + ".asset");
    }
}
