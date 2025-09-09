using UnityEngine;

[CreateAssetMenu(fileName = "TelekineticRayTexture", menuName = "Custom/Telekinetic Ray Texture Generator")]
public class TelekineticRayTextureGenerator : ScriptableObject
{
    [Header("Texture Settings")]
    [SerializeField] private int textureWidth = 256;
    [SerializeField] private int textureHeight = 32;
    [SerializeField] private bool generateOnValidate = true;
    
    [Header("Ray Pattern")]
    [SerializeField] private Color coreColor = new Color(1f, 0.9f, 1f, 1f);
    [SerializeField] private Color edgeColor = new Color(0.9f, 0.3f, 1f, 0f);
    [SerializeField] private Color midColor = new Color(1f, 0.4f, 0.8f, 0.7f);
    [SerializeField] private AnimationCurve intensityCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private float noiseScale = 12f;
    [SerializeField] private float electricalIntensity = 0.7f;
    [SerializeField] private float magicalSparkles = 0.3f;
    
    [Header("Output")]
    [SerializeField] private string outputPath = "Assets/Gravity Gun uwu/Textures/";
    [SerializeField] private string fileName = "TelekineticRayTexture";
    
    public Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[textureWidth * textureHeight];
        
        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                float normalizedY = (float)y / (textureHeight - 1);
                float normalizedX = (float)x / (textureWidth - 1);
                
                float distanceFromCenter = Mathf.Abs(normalizedY - 0.5f) * 2f;
                
                float intensity = intensityCurve.Evaluate(1f - distanceFromCenter);
                
                float noise = Mathf.PerlinNoise(normalizedX * noiseScale, normalizedY * noiseScale * 2f);
                float electricalNoise = Mathf.Sin(normalizedX * noiseScale * 2f) * 0.5f + 0.5f;
                
                float sparkleNoise = Mathf.PerlinNoise(normalizedX * noiseScale * 3f, normalizedY * noiseScale * 4f);
                float magicalFlicker = Mathf.Sin(normalizedX * noiseScale * 5f) * Mathf.Cos(normalizedY * noiseScale * 3f);
                magicalFlicker = (magicalFlicker + 1f) * 0.5f;
                
                float baseNoise = Mathf.Lerp(noise, electricalNoise, electricalIntensity);
                float sparkleEffect = sparkleNoise * magicalFlicker * magicalSparkles;
                float finalNoise = baseNoise + sparkleEffect;
                
                intensity *= (0.6f + finalNoise * 0.4f);
                
                Color baseColor;
                if (distanceFromCenter < 0.3f)
                {
                    float centerBlend = distanceFromCenter / 0.3f;
                    baseColor = Color.Lerp(coreColor, midColor, centerBlend);
                }
                else
                {
                    float edgeBlend = (distanceFromCenter - 0.3f) / 0.7f;
                    baseColor = Color.Lerp(midColor, edgeColor, edgeBlend);
                }
                
                Color finalColor = baseColor;
                finalColor.a *= intensity;
                
                if (sparkleNoise > 0.8f && magicalFlicker > 0.7f)
                {
                    finalColor.r += 0.3f;
                    finalColor.g += 0.2f;
                    finalColor.b += 0.4f;
                    finalColor.a += 0.2f;
                }
                
                if (distanceFromCenter > 0.8f)
                {
                    float edgeFade = (distanceFromCenter - 0.8f) / 0.2f;
                    finalColor.a *= (1f - edgeFade);
                }
                
                pixels[y * textureWidth + x] = finalColor;
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        return texture;
    }
    
    #if UNITY_EDITOR
    [ContextMenu("Generate and Save Texture")]
    public void GenerateAndSaveTexture()
    {
        Texture2D texture = GenerateTexture();
        
        if (!System.IO.Directory.Exists(outputPath))
        {
            System.IO.Directory.CreateDirectory(outputPath);
        }
        
        byte[] pngData = texture.EncodeToPNG();
        string fullPath = outputPath + fileName + ".png";
        System.IO.File.WriteAllBytes(fullPath, pngData);
        
        UnityEditor.AssetDatabase.Refresh();
        
        UnityEditor.TextureImporter importer = UnityEditor.AssetImporter.GetAtPath(fullPath) as UnityEditor.TextureImporter;
        if (importer != null)
        {
            importer.textureType = UnityEditor.TextureImporterType.Default;
            importer.alphaSource = UnityEditor.TextureImporterAlphaSource.FromInput;
            importer.alphaIsTransparency = true;
            importer.wrapMode = TextureWrapMode.Repeat;
            importer.filterMode = FilterMode.Bilinear;
            importer.SaveAndReimport();
        }
        
        Debug.Log($"Telekinetic ray texture generated and saved to: {fullPath}");
        
        DestroyImmediate(texture);
    }
    
    private void OnValidate()
    {
        if (generateOnValidate && Application.isPlaying)
        {
            GenerateAndSaveTexture();
        }
    }
    #endif
}
