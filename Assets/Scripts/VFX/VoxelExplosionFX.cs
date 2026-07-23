using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelExplosionFX : MonoBehaviour
{
    private static VoxelExplosionFX _instance;
    private static Stack<VoxelCube> _pool = new Stack<VoxelCube>();
    private static Material _sharedMaterial;
    private static Mesh _cubeMesh;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private static void EnsureInitialized()
    {
        if (_instance == null)
        {
            GameObject managerGO = new GameObject("[VoxelExplosionFX_Manager]");
            _instance = managerGO.AddComponent<VoxelExplosionFX>();
            DontDestroyOnLoad(managerGO);
        }

        if (_cubeMesh == null)
        {
            GameObject tempCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _cubeMesh = tempCube.GetComponent<MeshFilter>().sharedMesh;
            Destroy(tempCube);
        }

        if (_sharedMaterial == null)
        {
            Shader shader = Shader.Find("FaRTeam/FaRMainShaderURP");
            if (shader == null) shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null) shader = Shader.Find("Sprites/Default");
            if (shader == null) shader = Shader.Find("Unlit/Color");
            if (shader == null) shader = Shader.Find("Standard");

            _sharedMaterial = new Material(shader);
        }
    }

    public static void Spawn(GameObject targetObject, Vector3 position, int voxelCount = 25)
    {
        List<GameObject> targets = new List<GameObject>();
        if (targetObject != null) targets.Add(targetObject);
        Spawn(targets, position, voxelCount);
    }

    public static void Spawn(List<GameObject> targetObjects, Vector3 position, int voxelCount = 25)
    {
        EnsureInitialized();
        List<Color> palette = ExtractPalette(targetObjects);
        SpawnVoxels(palette, position, voxelCount);
    }

    public static void Spawn(List<Color> palette, Vector3 position, int voxelCount = 25)
    {
        EnsureInitialized();
        SpawnVoxels(palette, position, voxelCount);
    }

    private static List<Color> ExtractPalette(List<GameObject> targets)
    {
        List<Color> palette = new List<Color>();
        if (targets == null) return null;

        foreach (GameObject obj in targets)
        {
            if (obj == null) continue;

            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer rend in renderers)
            {
                if (rend == null || !rend.enabled) continue;

                Material[] materials = rend.sharedMaterials;
                foreach (Material mat in materials)
                {
                    if (mat == null) continue;

                    Texture mainTex = mat.mainTexture;
                    if (mainTex == null && mat.HasProperty("_MainTex")) mainTex = mat.GetTexture("_MainTex");
                    if (mainTex == null && mat.HasProperty("_BaseMap")) mainTex = mat.GetTexture("_BaseMap");

                    if (mainTex is Texture2D tex2D)
                    {
                        List<Color> texColors = SampleTextureColors(tex2D, 8);
                        palette.AddRange(texColors);
                    }

                    if (mat.HasProperty("_Color"))
                    {
                        palette.Add(mat.GetColor("_Color"));
                    }
                    else if (mat.HasProperty("_BaseColor"))
                    {
                        palette.Add(mat.GetColor("_BaseColor"));
                    }
                    else
                    {
                        palette.Add(mat.color);
                    }
                }
            }
        }

        if (palette.Count == 0)
        {
            return null;
        }

        return palette;
    }

    private static List<Color> SampleTextureColors(Texture2D tex, int sampleGrid = 8)
    {
        List<Color> result = new List<Color>();
        if (tex == null) return result;

        try
        {
            RenderTexture rt = RenderTexture.GetTemporary(sampleGrid, sampleGrid, 0, RenderTextureFormat.ARGB32);
            Graphics.Blit(tex, rt);
            RenderTexture prev = RenderTexture.active;
            RenderTexture.active = rt;

            Texture2D temp = new Texture2D(sampleGrid, sampleGrid, TextureFormat.RGBA32, false);
            temp.ReadPixels(new Rect(0, 0, sampleGrid, sampleGrid), 0, 0);
            temp.Apply();

            RenderTexture.active = prev;
            RenderTexture.ReleaseTemporary(rt);

            Color[] pixels = temp.GetPixels();
            UnityEngine.Object.Destroy(temp);

            foreach (Color c in pixels)
            {
                if (c.a > 0.15f)
                {
                    result.Add(c);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[VoxelExplosionFX] Failed sampling texture: {e.Message}");
        }

        return result;
    }

    private static void SpawnVoxels(List<Color> palette, Vector3 centerPosition, int count)
    {
        if (palette == null || palette.Count == 0)
        {
            palette = null;
        }

        float floorY = centerPosition.y - 0.35f;
        if (Physics.Raycast(centerPosition, Vector3.down, out RaycastHit hit, 5.0f, ~0, QueryTriggerInteraction.Ignore))
        {
            floorY = hit.point.y;
        }

        for (int i = 0; i < count; i++)
        {
            VoxelCube cube = GetCubeFromPool();
            Color color = palette[UnityEngine.Random.Range(0, palette.Count)];

            Vector3 offset = UnityEngine.Random.insideUnitSphere * 0.3f;
            Vector3 spawnPos = centerPosition + offset;

            Vector3 randDir = (UnityEngine.Random.onUnitSphere + Vector3.up * 1.6f).normalized;
            float speed = UnityEngine.Random.Range(2.8f, 5.8f);
            Vector3 velocity = randDir * speed;

            Vector3 angularVel = UnityEngine.Random.insideUnitSphere * 720f;
            float cubeScaleValue = 0.15f;
            Vector3 scale = Vector3.one * cubeScaleValue;

            cube.Init(color, spawnPos, velocity, angularVel, scale, floorY);
        }
    }

    private static VoxelCube GetCubeFromPool()
    {
        while (_pool.Count > 0)
        {
            VoxelCube pooled = _pool.Pop();
            if (pooled != null)
            {
                return pooled;
            }
        }

        GameObject go = new GameObject("VoxelCube");
        go.transform.SetParent(_instance.transform);

        MeshFilter mf = go.AddComponent<MeshFilter>();
        mf.sharedMesh = _cubeMesh;

        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        mr.sharedMaterial = _sharedMaterial;
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

        VoxelCube cube = go.AddComponent<VoxelCube>();
        return cube;
    }

    public static void RecycleCube(VoxelCube cube)
    {
        if (cube == null) return;
        cube.gameObject.SetActive(false);
        _pool.Push(cube);
    }
}

public class VoxelCube : MonoBehaviour
{
    private Vector3 _velocity;
    private Vector3 _angularVelocity;
    private Vector3 _initialScale;
    private float _floorY;
    private bool _hasLanded;
    private float _shrinkTimer;
    private float _shrinkDuration;
    private float _maxAirTime;
    private MeshRenderer _renderer;
    private static MaterialPropertyBlock _propBlock;

    public void Init(Color color, Vector3 position, Vector3 velocity, Vector3 angularVelocity, Vector3 scale, float floorY)
    {
        transform.position = position;
        transform.rotation = UnityEngine.Random.rotation;
        transform.localScale = scale;
        _initialScale = scale;
        _velocity = velocity;
        _angularVelocity = angularVelocity;
        _floorY = floorY + scale.y * 0.5f;
        _hasLanded = false;
        _shrinkTimer = 0f;
        _shrinkDuration = 0.3f;
        _maxAirTime = 2.5f;

        if (_renderer == null)
        {
            _renderer = GetComponent<MeshRenderer>();
        }

        if (_propBlock == null)
        {
            _propBlock = new MaterialPropertyBlock();
        }

        _renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetColor("_Color", color);
        _propBlock.SetColor("_BaseColor", color);
        _renderer.SetPropertyBlock(_propBlock);

        gameObject.SetActive(true);
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        if (!_hasLanded)
        {
            transform.position += _velocity * dt;
            _velocity += Vector3.down * 16.0f * dt;
            transform.Rotate(_angularVelocity * dt, Space.World);

            _maxAirTime -= dt;

            if ((transform.position.y <= _floorY && _velocity.y < 0) || _maxAirTime <= 0f)
            {
                _hasLanded = true;
                Vector3 landPos = transform.position;
                landPos.y = _floorY;
                transform.position = landPos;

                _shrinkDuration = UnityEngine.Random.Range(0.2f, 0.4f);
                _shrinkTimer = _shrinkDuration;
            }
        }
        else
        {
            _shrinkTimer -= dt;

            if (_shrinkTimer <= 0f)
            {
                VoxelExplosionFX.RecycleCube(this);
                return;
            }

            float scaleFactor = Mathf.Clamp01(_shrinkTimer / _shrinkDuration);
            transform.localScale = _initialScale * scaleFactor;
        }
    }
}
