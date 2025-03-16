using UnityEngine;
using System.Collections.Generic;

namespace autovoxel {

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(AutoVoxel))]
public class AutoVoxelEditor : Editor
{
    private string outputPath = "Assets/AutoVoxel/Meshes";
    private bool enableOptimization = true;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        outputPath = EditorGUILayout.TextField("Output Path", outputPath);
        enableOptimization = EditorGUILayout.Toggle("Enable Optimization", enableOptimization);

        AutoVoxel script = (AutoVoxel)target;
        if (GUILayout.Button("Generate Mesh"))
        {
            FaceManager.Reset();
            script.Setup();
            script.GenerateMesh(enableOptimization, outputPath);
        }
    }
}
#endif


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class AutoVoxel : MonoBehaviour { 

    private Texture2D _uvTextureRaw;
    private int _tileResolution;
    private float _stepSize;
    private float _uvTileSize;
    private float _uvStepSize;
    private Mesh _mesh;
    private List<Vector3> vertices;
    private List<Vector3> normals;
    private List<int> triangles;
    private List<Vector2> uvs;

    private int textureRows = 3;

    public void Setup () {
        try
        {
            // Check MeshRenderer
            var meshRenderer = gameObject.GetComponent<MeshRenderer>();
            if (meshRenderer.sharedMaterial == null)
            {
                throw new System.Exception("MeshRenderer Material missing.");
            }

            // Check MeshRenderer Material
            Texture2D originalTexture = meshRenderer.sharedMaterial.mainTexture as Texture2D;
            if (originalTexture == null)
            {
                throw new System.Exception("MeshRenderer Material is missing a Texture.");
            }

            // Check Texture Aspect Ratio
            _uvTextureRaw = new Texture2D(originalTexture.width, originalTexture.height, originalTexture.format, originalTexture.mipmapCount > 1);
            Graphics.CopyTexture(originalTexture, _uvTextureRaw);
            _uvTextureRaw.Apply();
            int gcd = Mathf.Min(_uvTextureRaw.width, _uvTextureRaw.height);
            if (_uvTextureRaw.width % gcd != 0 || _uvTextureRaw.height % gcd != 0)
            {
                throw new System.Exception("MeshRenderer Material Texture has to be in 1:1 aspect ratio.");
            }

            // Setup Variables
            _tileResolution = gcd / textureRows;
            _stepSize       = 1f / _tileResolution;
            _uvTileSize     = 1f / textureRows;
            _uvStepSize     = _uvTileSize / _tileResolution;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public void GenerateMesh(bool optimise, string outputPath)
    {
        _mesh = new Mesh();
        vertices = new List<Vector3>();
        normals = new List<Vector3>();
        triangles = new List<int>();
        uvs = new List<Vector2>();
        
        FaceManager.Initialize(_uvTextureRaw, _tileResolution);

        foreach(Face face in FaceManager.faces)
        {
            GenerateTileMeshData(face);
        }

        if (optimise)
        {
            int vertexCount = vertices.Count;
            int optimisedCount = -1;

            while (optimisedCount != vertices.Count)
            {
                optimisedCount = vertices.Count;
                RemoveFloatingQuads();
            }
        }
        
        _mesh.Clear();
        _mesh.name = $"TexToMesh_{_uvTextureRaw.name}";
        _mesh.SetVertices(vertices);
        _mesh.SetNormals(normals);
        _mesh.SetTriangles(triangles, 0);
        _mesh.SetUVs(0, uvs);
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
        _mesh.Optimize();

        GetComponent<MeshFilter>().mesh = _mesh;

        #if UNITY_EDITOR
            string path = outputPath + "/" + gameObject.name + ".asset";
            AssetDatabase.CreateAsset(_mesh, path);
            AssetDatabase.SaveAssets();
        #endif
    }

    void GenerateTileMeshData(Face face)
    {
        Vector2 uvBaseOffset = new Vector2(face.TileIndex.x * _uvTileSize, face.TileIndex.y * _uvTileSize);

        for (float y = 0; y < 1; y += _stepSize)
        {
            for (float x = 0; x < 1; x += _stepSize)
            {
                Vector2Int pixelCoords = FaceManager.WorldSpaceXYToPixelCoord(face,x,y);
                bool isPixelSolid = face.IsPixelSolid(pixelCoords.x, pixelCoords.y);
                List<float> zOffsets = FaceManager.GetZOffset(face, x, y);

                foreach (float z in zOffsets)
                {
                    if (isPixelSolid)
                    {
                        GenerateQuadMesh(face, x, y, z, uvBaseOffset);
                    }
                }
            }
        }
    }

    private void GenerateQuadMesh(Face face, float x, float y, float z, Vector2 uvBaseOffset)
    {
        //Debug.Log($"Generating Quad at {x}, {y}, {z}");
        Vector3 right;
        if (Vector3.Dot(face.Normal, Vector3.up) == 1f || Vector3.Dot(face.Normal, Vector3.up) == -1f)
        {
            right = Vector3.right; // Up or down facing
        }
        else
        {
            right = Vector3.Cross(Vector3.up, face.Normal).normalized; // General case
        }

        Vector3 up = Vector3.Cross(face.Normal, right).normalized;   // Compute the up vector

        // Calculate quad vertices in local face space
        Vector3 bottomLeft  = face.PosOffset + x * right                + y * up                + z * face.Normal;
        Vector3 bottomRight = face.PosOffset + (x + _stepSize) * right  + y * up                + z * face.Normal;
        Vector3 topLeft     = face.PosOffset + x * right                + (y + _stepSize) * up  + z * face.Normal;
        Vector3 topRight    = face.PosOffset + (x + _stepSize) * right  + (y + _stepSize) * up  + z * face.Normal;

        int startIndex = vertices.Count;

        vertices.Add(bottomLeft);
        vertices.Add(bottomRight);
        vertices.Add(topLeft);
        vertices.Add(topRight);

        normals.Add(face.Normal);
        normals.Add(face.Normal);
        normals.Add(face.Normal);
        normals.Add(face.Normal);

        triangles.Add(startIndex);
        triangles.Add(startIndex + 2);
        triangles.Add(startIndex + 1);

        triangles.Add(startIndex + 1);
        triangles.Add(startIndex + 2);
        triangles.Add(startIndex + 3);

        Vector2 uvOffset = uvBaseOffset + new Vector2(x * _uvTileSize, y * _uvTileSize);

        Vector2 uvBottomLeft = uvOffset;
        Vector2 uvBottomRight = uvOffset + new Vector2(_uvStepSize, 0);
        Vector2 uvTopLeft = uvOffset + new Vector2(0, _uvStepSize);
        Vector2 uvTopRight = uvOffset + new Vector2(_uvStepSize, 0);

        uvs.Add(uvBottomLeft);
        uvs.Add(uvBottomRight);
        uvs.Add(uvTopLeft);
        uvs.Add(uvTopRight);
    }

    public void OptimiseMesh()
    {
        // Map to track unique vertices and their new indices
        Dictionary<(Vector3 position, Vector3 normal, Vector2 uv), int> uniqueVertexMap = new Dictionary<(Vector3, Vector3, Vector2), int>();
        List<Vector3> newVertices = new List<Vector3>();
        List<Vector3> newNormals = new List<Vector3>();
        List<Vector2> newUvs = new List<Vector2>();

        // New triangle list
        List<int> newTriangles = new List<int>();

        // Populate unique vertices and update triangles
        for (int i = 0; i < triangles.Count; i++)
        {
            int oldIndex = triangles[i];
            var key = (
                position: RoundVector3(vertices[oldIndex], 6),
                normal: RoundVector3(normals[oldIndex], 6),
                uv: RoundVector2(uvs[oldIndex], 6)
            );

            if (!uniqueVertexMap.TryGetValue(key, out int newIndex))
            {
                // Add new unique vertex
                newIndex = newVertices.Count;
                uniqueVertexMap[key] = newIndex;

                newVertices.Add(vertices[oldIndex]);
                newNormals.Add(normals[oldIndex]);
                newUvs.Add(uvs[oldIndex]);
            }

            // Update triangle index
            newTriangles.Add(newIndex);
        }

        // Replace old lists with optimized lists
        vertices = newVertices;
        normals = newNormals;
        uvs = newUvs;
        triangles = newTriangles;
    }

    // Helper function to round Vector3 to handle floating-point precision issues
    private Vector3 RoundVector3(Vector3 vector, int decimalPlaces)
    {
        float multiplier = Mathf.Pow(10, decimalPlaces);
        return new Vector3(
            Mathf.Round(vector.x * multiplier) / multiplier,
            Mathf.Round(vector.y * multiplier) / multiplier,
            Mathf.Round(vector.z * multiplier) / multiplier
        );
    }

    // Helper function to round Vector2 to handle floating-point precision issues
    private Vector2 RoundVector2(Vector2 vector, int decimalPlaces)
    {
        float multiplier = Mathf.Pow(10, decimalPlaces);
        return new Vector2(
            Mathf.Round(vector.x * multiplier) / multiplier,
            Mathf.Round(vector.y * multiplier) / multiplier
        );
    }


public void RemoveFloatingQuads()
{
    // Step 1: Identify and remove floating quads (triangles connected to floating vertices)
    Dictionary<Vector3, int> vertexPositionCount = new Dictionary<Vector3, int>();

    // Count occurrences of each vertex position
    foreach (Vector3 vertex in vertices)
    {
        if (vertexPositionCount.ContainsKey(vertex))
        {
            vertexPositionCount[vertex]++;
        }
        else
        {
            vertexPositionCount[vertex] = 1;
        }
    }

    // Identify floating vertex indices
    HashSet<int> floatingVertexIndices = new HashSet<int>();
    for (int i = 0; i < vertices.Count; i++)
    {
        if (vertexPositionCount[vertices[i]] == 1)
        {
            floatingVertexIndices.Add(i);
        }
    }

    // Create a new triangle list excluding triangles with floating vertices
    List<int> newTriangles = new List<int>();
    for (int i = 0; i < triangles.Count; i += 3)
    {
        int v0 = triangles[i];
        int v1 = triangles[i + 1];
        int v2 = triangles[i + 2];

        // Check if any vertex in the triangle is floating
        if (!floatingVertexIndices.Contains(v0) &&
            !floatingVertexIndices.Contains(v1) &&
            !floatingVertexIndices.Contains(v2))
        {
            // If all vertices are valid, add the triangle
            newTriangles.Add(v0);
            newTriangles.Add(v1);
            newTriangles.Add(v2);
        }
    }

    // Step 2: Clean up unused vertices
    HashSet<int> usedVertexIndices = new HashSet<int>(newTriangles);
    List<Vector3> newVertices = new List<Vector3>();
    List<Vector3> newNormals = new List<Vector3>();
    List<Vector2> newUvs = new List<Vector2>();
    Dictionary<int, int> oldToNewIndexMap = new Dictionary<int, int>();

    // Create new vertices list with only those used in triangles
    for (int i = 0; i < vertices.Count; i++)
    {
        if (usedVertexIndices.Contains(i))
        {
            oldToNewIndexMap[i] = newVertices.Count;
            newVertices.Add(vertices[i]);
            newNormals.Add(normals[i]);
            newUvs.Add(uvs[i]);
        }
    }

    // Step 3: Update triangle indices to use the new vertex list
    for (int i = 0; i < newTriangles.Count; i++)
    {
        newTriangles[i] = oldToNewIndexMap[newTriangles[i]];
    }

    // Replace old lists with updated lists
    vertices = newVertices;
    normals = newNormals;
    uvs = newUvs;
    triangles = newTriangles;
}


}

}