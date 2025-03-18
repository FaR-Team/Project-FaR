using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class DebugMenu : MonoBehaviour
{
    private bool isDebugMenuVisible = false;
    private bool isUIVisible = true;
    private GameObject currentLookTarget;
    public string[] ignoreTags = { "Player", "MainCamera", "UI", "Violeta", "Pared"};
    
    // Section toggles
    private bool showTransformSection = true;
    private bool showRendererSection = true;
    private bool showPhysicsSection = true;
    private bool showScriptsSection = true;
    
    // Navigation
    private int currentSection = 0;
    private int totalSections = 4; // Transform, Renderer, Physics, Scripts
    private float navigationCooldown = 0f;
    private const float NAVIGATION_DELAY = 0.2f;
    
    // Script navigation
    private int currentScriptIndex = 0;
    private List<MonoBehaviour> availableScripts = new List<MonoBehaviour>();
    private Dictionary<string, bool> scriptExpanded = new Dictionary<string, bool>();
    
    // Profiler
    private bool showProfiler = false;
    private Queue<float> fpsBuffer = new Queue<float>();
    private const int BUFFER_SIZE = 100;
    private Rect profilerRect = new Rect(10, 10, 200, 100);
    
    void Update()
    {
        // Toggle debug menu
        if (Input.GetKeyDown(KeyCode.F3))
        {
            ToggleDebugMenu();
        }
        
        // Toggle UI visibility
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ToggleUIVisibility();
        }
        
        // Screenshot
        if (Input.GetKeyDown(KeyCode.F2))
        {
            TakeScreenshot();
        }
        
        if (isDebugMenuVisible)
        {
            UpdateLookTarget();
            
            // Handle navigation cooldown
            if (navigationCooldown > 0)
            {
                navigationCooldown -= Time.deltaTime;
            }
            
            // Section navigation
            if (navigationCooldown <= 0)
            {
                // Navigate between sections with up/down arrows
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    currentSection = (currentSection + 1) % totalSections;
                    navigationCooldown = NAVIGATION_DELAY;
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    currentSection = (currentSection - 1 + totalSections) % totalSections;
                    navigationCooldown = NAVIGATION_DELAY;
                }
                
                // Toggle current section with Enter or Space
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
                {
                    ToggleCurrentSection();
                    navigationCooldown = NAVIGATION_DELAY;
                }
                
                // Script navigation
                if (showScriptsSection && currentSection == 3) // Scripts section
                {
                    if (Input.GetKeyDown(KeyCode.RightArrow) && availableScripts.Count > 0)
                    {
                        currentScriptIndex = (currentScriptIndex + 1) % availableScripts.Count;
                        navigationCooldown = NAVIGATION_DELAY;
                    }
                    else if (Input.GetKeyDown(KeyCode.LeftArrow) && availableScripts.Count > 0)
                    {
                        currentScriptIndex = (currentScriptIndex - 1 + availableScripts.Count) % availableScripts.Count;
                        navigationCooldown = NAVIGATION_DELAY;
                    }
                }
            }
        }
        
        // Update profiler data
        if (showProfiler)
        {
            float fps = 1.0f / Time.deltaTime;
            fpsBuffer.Enqueue(fps);
            if (fpsBuffer.Count > BUFFER_SIZE)
                fpsBuffer.Dequeue();
        }
    }
    
    private void ToggleCurrentSection()
    {
        switch (currentSection)
        {
            case 0: // Transform
                showTransformSection = !showTransformSection;
                break;
            case 1: // Renderer
                showRendererSection = !showRendererSection;
                break;
            case 2: // Physics
                showPhysicsSection = !showPhysicsSection;
                break;
            case 3: // Scripts
                showScriptsSection = !showScriptsSection;
                break;
        }
    }
    
    void OnGUI()
    {
        if (isDebugMenuVisible)
        {
            int windowWidth = 300;
            int baseHeight = 300;
            int rightMargin = 10;
            int topMargin = 100;
            
            int xPosition = Screen.width - windowWidth - rightMargin;
            int yPosition = topMargin;
            
            GUI.Box(new Rect(xPosition, yPosition, windowWidth, baseHeight), "Debug Menu (↑↓: Navigate, Space/Enter: Toggle)");
            
            if (currentLookTarget != null && !ignoreTags.Contains(currentLookTarget.tag))
            {
                int yOffset = 30;
                
                // Basic object info
                GUI.Label(new Rect(xPosition + 10, yPosition + yOffset, windowWidth - 20, 20), $"Name: {currentLookTarget.name}");
                yOffset += 20;
                GUI.Label(new Rect(xPosition + 10, yPosition + yOffset, windowWidth - 20, 20), $"Tag: {currentLookTarget.tag}");
                yOffset += 20;
                
                // Section headers with selection indicator
                yOffset += 10;
                
                // Transform section
                string transformPrefix = (currentSection == 0) ? "▶ " : "  ";
                GUI.Label(new Rect(xPosition + 10, yPosition + yOffset, windowWidth - 20, 20), 
                    $"{transformPrefix}Transform {(showTransformSection ? "[-]" : "[+]")}");
                yOffset += 25;
                
                if (showTransformSection)
                {
                    GUI.Label(new Rect(xPosition + 20, yPosition + yOffset, windowWidth - 30, 20), 
                        $"Position: {FormatVector3(currentLookTarget.transform.position)}");
                    yOffset += 20;
                    GUI.Label(new Rect(xPosition + 20, yPosition + yOffset, windowWidth - 30, 20), 
                        $"Rotation: {FormatVector3(currentLookTarget.transform.eulerAngles)}");
                    yOffset += 20;
                    GUI.Label(new Rect(xPosition + 20, yPosition + yOffset, windowWidth - 30, 20), 
                        $"Scale: {FormatVector3(currentLookTarget.transform.localScale)}");
                    yOffset += 20;
                }
                
                // Renderer section
                Renderer renderer = currentLookTarget.GetComponent<Renderer>();
                if (renderer != null)
                {
                    string rendererPrefix = (currentSection == 1) ? "▶ " : "  ";
                    GUI.Label(new Rect(xPosition + 10, yPosition + yOffset, windowWidth - 20, 20), 
                        $"{rendererPrefix}Renderer {(showRendererSection ? "[-]" : "[+]")}");
                    yOffset += 25;
                    
                    if (showRendererSection)
                    {
                        GUI.Label(new Rect(xPosition + 20, yPosition + yOffset, windowWidth - 30, 20), 
                            $"Material: {renderer.material.name}");
                        yOffset += 20;
                    }
                }
                
                // Physics section
                Rigidbody rb = currentLookTarget.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    string physicsPrefix = (currentSection == 2) ? "▶ " : "  ";
                    GUI.Label(new Rect(xPosition + 10, yPosition + yOffset, windowWidth - 20, 20), 
                        $"{physicsPrefix}Physics {(showPhysicsSection ? "[-]" : "[+]")}");
                    yOffset += 25;
                    
                    if (showPhysicsSection)
                    {
                        GUI.Label(new Rect(xPosition + 20, yPosition + yOffset, windowWidth - 30, 20), 
                            $"Velocity: {FormatVector3(rb.velocity)}");
                        yOffset += 20;
                        GUI.Label(new Rect(xPosition + 20, yPosition + yOffset, windowWidth - 30, 20), 
                            $"Mass: {rb.mass}");
                        yOffset += 20;
                    }
                }
                
                // Scripts section
                string scriptsPrefix = (currentSection == 3) ? "▶ " : "  ";
                GUI.Label(new Rect(xPosition + 10, yPosition + yOffset, windowWidth - 20, 20), 
                    $"{scriptsPrefix}Scripts {(showScriptsSection ? "[-]" : "[+]")} (←→: Navigate Scripts)");
                yOffset += 25;
                
                if (showScriptsSection)
                {
                    DisplayScriptInfo(xPosition, yPosition, yOffset, windowWidth);
                }
            }
            else
            {
                GUI.Label(new Rect(xPosition + 10, yPosition + 30, windowWidth - 20, 20), 
                    "Not looking at any relevant object");
            }
        }
    }
    
    // Helper method to format Vector3 values
    private string FormatVector3(Vector3 vector)
    {
        return $"({vector.x:F2}, {vector.y:F2}, {vector.z:F2})";
    }
    
    // Display script information
    private void DisplayScriptInfo(int xPosition, int yPosition, int yOffset, int windowWidth)
    {
        // Get all scripts
        MonoBehaviour[] scripts = currentLookTarget.GetComponents<MonoBehaviour>();
        
        if (scripts.Length == 0)
        {
            GUI.Label(new Rect(xPosition + 20, yPosition + yOffset, windowWidth - 30, 20), "No scripts attached");
            availableScripts.Clear();
            return;
        }
        
        // Update available scripts list
        if (availableScripts.Count != scripts.Length)
        {
            availableScripts.Clear();
            foreach (var script in scripts)
            {
                if (script != null)
                    availableScripts.Add(script);
            }
            
            // Reset current index if needed
            if (currentScriptIndex >= availableScripts.Count)
                currentScriptIndex = 0;
        }
        
        if (availableScripts.Count == 0)
            return;
        
        // Display current script
        MonoBehaviour currentScript = availableScripts[currentScriptIndex];
        string scriptName = currentScript.GetType().Name;
        
        GUI.Label(new Rect(xPosition + 20, yPosition + yOffset, windowWidth - 30, 20), 
            $"Script {currentScriptIndex + 1}/{availableScripts.Count}: {scriptName}");
        yOffset += 20;
        
        // Display script fields
        System.Reflection.FieldInfo[] fields = currentScript.GetType().GetFields(
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        
        if (fields.Length == 0)
        {
            GUI.Label(new Rect(xPosition + 30, yPosition + yOffset, windowWidth - 40, 20), "No public fields");
            return;
        }
        
        // Display up to 5 fields to avoid cluttering
        int maxFieldsToShow = Mathf.Min(fields.Length, 5);
        for (int i = 0; i < maxFieldsToShow; i++)
        {
            var field = fields[i];
            string value = field.GetValue(currentScript)?.ToString() ?? "null";
            
            // Truncate long values
            if (value.Length > 30)
                value = value.Substring(0, 27) + "...";
            
            GUI.Label(new Rect(xPosition + 30, yPosition + yOffset, windowWidth - 40, 20), 
                $"{field.Name}: {value}");
            yOffset += 20;
        }
        
        // Show indicator if there are more fields
        if (fields.Length > maxFieldsToShow)
        {
            GUI.Label(new Rect(xPosition + 30, yPosition + yOffset, windowWidth - 40, 20), 
                $"... and {fields.Length - maxFieldsToShow} more fields");
        }
    }
    
    private void ToggleDebugMenu()
    {
        isDebugMenuVisible = !isDebugMenuVisible;
    }
    
    private void ToggleUIVisibility()
    {
        isUIVisible = !isUIVisible;
        
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in allCanvases)
        {
            canvas.enabled = isUIVisible;
        }
        
        GameObject[] handObjects = new GameObject[] {
            GameObject.Find("Hand"),
            GameObject.Find("Bucket"),
            GameObject.Find("Hoe")
        };
        
        foreach (GameObject hand in handObjects)
        {
            if (hand != null && hand.activeSelf)
            {
                hand.SetActive(isUIVisible);
                break;
            }
        }
    }
    
    private void UpdateLookTarget()
    {
        int layerMask = ~((1 << LayerMask.NameToLayer("Jugador")) | (1 << LayerMask.NameToLayer("Hand")));
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 100f, layerMask))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (!ignoreTags.Contains(hitObject.tag))
            {
                currentLookTarget = hitObject;
            }
            else
            {
                currentLookTarget = null;
            }
        }
        else
        {
            currentLookTarget = null;
        }
    }
    
    private void TakeScreenshot()
    {
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        FaREditorUtils.SaveScreenshot(timestamp);
    }
}