using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CloudDrawing : EditorWindow
{
    GameObject cloudGO;
    GameObject brushGO;

    [MenuItem("Cloud Drawing/Drawing")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        CloudDrawing window = (CloudDrawing)EditorWindow.GetWindow(typeof(CloudDrawing));
        window.Show();
    }

    private void OnGUI()
    {
        cloudGO = (GameObject)EditorGUILayout.ObjectField("Cloud GameObject", cloudGO, typeof(GameObject));
        brushGO = (GameObject)EditorGUILayout.ObjectField("Brush GameObject", brushGO, typeof(GameObject));

        EditorGUILayout.Space();

        if(GUILayout.Button("Draw !"))
        {
            Draw(cloudGO, brushGO);
        }
    }

    /// <summary>
    /// Draw the brush texture into the cloud texture
    /// </summary>
    void Draw(GameObject cloudGO, GameObject brushGO)
    {
        //Cloud stuff
        Material cloudMat = cloudGO.GetComponent<MeshRenderer>().sharedMaterial;
        Texture3D cloudTex = (Texture3D)cloudMat.mainTexture;
        Bounds cloudBounds = cloudGO.GetComponent<BoxCollider>().bounds;
        Vector3 cloudSlicesSpace = new Vector3(cloudBounds.size.x / cloudTex.width, cloudBounds.size.y / cloudTex.height, cloudBounds.size.z / cloudTex.depth);

        //Brush Stuff
        Texture3D brushTex = (Texture3D)brushGO.GetComponent<MeshRenderer>().sharedMaterial.mainTexture;
        Bounds brushBounds = brushGO.GetComponent<BoxCollider>().bounds;
        Vector3 brushSlicesSpace = new Vector3(brushBounds.size.x / brushTex.width, brushBounds.size.y / brushTex.height, brushBounds.size.z / brushTex.depth);

        //Check if the brush is inside of the cloud
        if(!cloudBounds.Intersects(brushBounds))
        {
            Debug.LogError("Can't draw here. The brush is not inside of the cloud !");
            return;
        }

        
        Vector3 dBounds = cloudBounds.extents + brushBounds.center - cloudBounds.center - brushBounds.extents; //Distance between cloud bounds and brush bounds

        //Last slices modified
        Vector3 cloudPrevSlices = new Vector3(-1, -1, -1);
        Vector3Int brushSlicesId = Vector3Int.zero; //Current ids in the brush texture

        //Loop over brush slices
        for (float z = dBounds.z; z < dBounds.z + brushBounds.size.z; z += brushSlicesSpace.z)
        {

            int sliceIdZ = (int)(z / cloudSlicesSpace.z); //Get slice id in the cloud
            if (sliceIdZ == cloudPrevSlices.z) //If this slice as already been modified do not override
            {
                brushSlicesId.z++;
                continue;
            }

            //Reset id Y
            brushSlicesId.y = 0;

            for (float y = dBounds.y; y < dBounds.y + brushBounds.size.y; y += brushSlicesSpace.y)
            {

                int sliceIdY = (int)(y / cloudSlicesSpace.y); //Get slice id in the cloud
                if (sliceIdY == cloudPrevSlices.y) //If this slice as already been modified do not override
                {
                    brushSlicesId.y++;
                    continue;
                }

                //Reset id X
                brushSlicesId.x = 0;

                for (float x = dBounds.x; x < dBounds.x + brushBounds.size.x; x += brushSlicesSpace.x)
                {

                    int sliceIdX = (int)(x / cloudSlicesSpace.x); //Get slice id in the cloud
                    if (sliceIdX == cloudPrevSlices.x) //If this slice as already been modified do not override
                    {
                        brushSlicesId.x++;
                        continue;
                    }

                    //Draw
                    Color brushColor = brushTex.GetPixel(brushSlicesId.x, brushSlicesId.y, brushSlicesId.z);
                    Color cloudColor = cloudTex.GetPixel(sliceIdX, sliceIdY, sliceIdZ);

                    cloudTex.SetPixel(sliceIdX, sliceIdY, sliceIdZ, cloudColor + brushColor);

                    cloudPrevSlices.x = sliceIdX;
                    brushSlicesId.x++;
                }

                cloudPrevSlices.y = sliceIdY;
                brushSlicesId.y++;
            }

            cloudPrevSlices.z = sliceIdZ;
            brushSlicesId.z++;
        }

        //Apply the texture
        cloudTex.Apply();
        cloudMat.SetTexture("_MainTex", cloudTex);
    }
}
