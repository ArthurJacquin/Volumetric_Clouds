using UnityEditor;
using UnityEngine;

public class Generate3DTextureEditor : EditorWindow
{
    enum TextureType
    {
        Empty,
        Default,
        Noise
    }

    string name;
    int size = 64;
    TextureType type;
    

    /// <summary>
    /// Create an empty 3D texture with all pixels to (0, 0, 0, 0)
    /// </summary>
    /// <param name="size">x, y and z dimensions</param>
    [MenuItem("Cloud Drawing/Generate Textures/Basic 3D texture")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        Generate3DTextureEditor window = (Generate3DTextureEditor)EditorWindow.GetWindow(typeof(Generate3DTextureEditor));
        window.Show();
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);
        name = EditorGUILayout.TextField("Texture name", name);
        size = EditorGUILayout.IntField("Texture size", size);
        type = (TextureType)EditorGUILayout.EnumPopup("Texture type", type);

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate 3D texture !"))
        {
            switch (type)
            {
                case TextureType.Empty:
                    CreateEmpty3DTexture(name, size);
                    break;
                case TextureType.Default:
                    CreateDefault3DTexture(name, size);
                    break;
                case TextureType.Noise:
                    CreateNoise3DTexture(name, size);
                    break;
            }
        }
    }

    static void CreateEmpty3DTexture(string name, int size)
    {
        // Configure the texture
        TextureFormat format = TextureFormat.RGBA32;
        TextureWrapMode wrapMode = TextureWrapMode.Clamp;

        // Create the texture and apply the configuration
        Texture3D texture = new Texture3D(size, size, size, format, false);
        texture.wrapMode = wrapMode;

        // Create a 3-dimensional array to store color data
        Color[] colors = new Color[size * size * size];

        for (int z = 0; z < size; z++)
        {
            int zOffset = z * size * size;
            for (int y = 0; y < size; y++)
            {
                int yOffset = y * size;
                for (int x = 0; x < size; x++)
                {
                    colors[x + yOffset + zOffset] = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                }
            }
        }

        // Copy the color values to the texture
        texture.SetPixels(colors);

        // Apply the changes to the texture and upload the updated texture to the GPU
        texture.Apply();

        // Save the texture to your Unity Project
        AssetDatabase.CreateAsset(texture, "Assets/Textures/Texture3D/" + name + ".asset");
    }

    /// <summary>
    /// Create a rainbow 3D texture
    /// </summary>
    /// /// <param name="size">x, y and z dimensions</param>
    static void CreateDefault3DTexture(string name, int size)
    {
        // Configure the texture
        TextureFormat format = TextureFormat.RGBA32;
        TextureWrapMode wrapMode = TextureWrapMode.Clamp;

        // Create the texture and apply the configuration
        Texture3D texture = new Texture3D(size, size, size, format, false);
        texture.wrapMode = wrapMode;

        // Create a 3-dimensional array to store color data
        Color[] colors = new Color[size * size * size];

        // Populate the array so that the x, y, and z values of the texture will map to red, blue, and green colors
        float inverseResolution = 1.0f / (size - 1.0f);
        for (int z = 0; z < size; z++)
        {
            int zOffset = z * size * size;
            for (int y = 0; y < size; y++)
            {
                int yOffset = y * size;
                for (int x = 0; x < size; x++)
                {
                    colors[x + yOffset + zOffset] = new Color(x * inverseResolution,
                        y * inverseResolution, z * inverseResolution, 1.0f);
                }
            }
        }

        // Copy the color values to the texture
        texture.SetPixels(colors);

        // Apply the changes to the texture and upload the updated texture to the GPU
        texture.Apply();

        // Save the texture to your Unity Project
        AssetDatabase.CreateAsset(texture, "Assets/Textures/Texture3D/" + name + ".asset");
    }


    /// <summary>
    /// Create a perlin 3D texture
    /// </summary>
    /// /// <param name="size">x, y and z dimensions</param>
    static void CreateNoise3DTexture(string name, int size)
    {
        // Configure the texture
        TextureFormat format = TextureFormat.RGBA32;
        TextureWrapMode wrapMode = TextureWrapMode.Clamp;

        // Create the texture and apply the configuration
        Texture3D texture = new Texture3D(size, size, size, format, false);
        texture.wrapMode = wrapMode;

        // Create a 3-dimensional array to store color data
        Color[] colors = new Color[size * size * size];

        // Populate the array so that the x, y, and z values of the texture will map to red, blue, and green colors
        for (int z = 0; z < size; z++)
        {
            int zOffset = z * size * size;
            for (int y = 0; y < size; y++)
            {
                int yOffset = y * size;
                for (int x = 0; x < size; x++)
                {
                    colors[x + yOffset + zOffset] = new Color(1f, 1f, 1f, Perlin.Fbm(x, y, z, 1) );
                    Debug.Log(Perlin.Fbm(x, y, z, 1));
                }
            }
        }

        // Copy the color values to the texture
        texture.SetPixels(colors);

        // Apply the changes to the texture and upload the updated texture to the GPU
        texture.Apply();

        // Save the texture to your Unity Project
        AssetDatabase.CreateAsset(texture, "Assets/Textures/Texture3D/" + name + ".asset");
    }
}