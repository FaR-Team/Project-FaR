using UnityEngine;

[CreateAssetMenu(fileName = "TelekineticRayTexture", menuName = "Custom/Telekinetic Ray Texture Generator")]
public class TelekineticRayTextureGenerator : ScriptableObject
{
    [Header("Texture Settings")]
    [SerializeField] private int textureWidth = 256;
    [SerializeField] private int textureHeight = 32;
    [SerializeField] private bool generateOnValidate = true;
    
    [Header("Ray Pattern")]
    [SerializeField] private Color coreColor = Color.white;
    [SerializeField] private Color edgeColor = new Color(0.2f, 0.8f, 1f, 0f);
    [SerializeField] private AnimationCurve intensityCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private float noiseScale = 10f;
    [SerializeField] private float electricalIntensity = 0.5f;
    
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
                
                // Crear gradiente desde el centro hacia los bordes
                float distanceFromCenter = Mathf.Abs(normalizedY - 0.5f) * 2f;
                
                // Aplicar curva de intensidad
                float intensity = intensityCurve.Evaluate(1f - distanceFromCenter);
                
                // Agregar ruido eléctrico
                float noise = Mathf.PerlinNoise(normalizedX * noiseScale, normalizedY * noiseScale * 2f);
                float electricalNoise = Mathf.Sin(normalizedX * noiseScale * 2f) * 0.5f + 0.5f;
                
                // Combinar ruido
                float finalNoise = Mathf.Lerp(noise, electricalNoise, electricalIntensity);
                intensity *= (0.7f + finalNoise * 0.3f);
                
                // Interpolar color
                Color finalColor = Color.Lerp(edgeColor, coreColor, intensity);
                
                // Aplicar intensidad al alpha
                finalColor.a *= intensity;
                
                // Hacer que los bordes sean más transparentes
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
        
        // Crear directorio si no existe
        if (!System.IO.Directory.Exists(outputPath))
        {
            System.IO.Directory.CreateDirectory(outputPath);
        }
        
        // Convertir a PNG
        byte[] pngData = texture.EncodeToPNG();
        string fullPath = outputPath + fileName + ".png";
        System.IO.File.WriteAllBytes(fullPath, pngData);
        
        // Refresh para que Unity vea el archivo
        UnityEditor.AssetDatabase.Refresh();
        
        // Importar con configuración correcta
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
