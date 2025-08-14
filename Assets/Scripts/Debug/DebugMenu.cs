using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class DebugMenu : MonoBehaviour
{
    private bool isDebugMenuVisible = false;
    private bool isUIVisible = true;
    private GameObject currentLookTarget;
    public string[] ignoreTags = { "Player", "MainCamera", "UI", "Violeta", "Pared"};
    
    private bool showTransformSection = true;
    private bool showRendererSection = true;
    private bool showPhysicsSection = true;
    private bool showScriptsSection = true;
    
    private int currentSection = 0;
    private int totalSections = 4;
    private float navigationCooldown = 0f;
    private const float NAVIGATION_DELAY = 0.2f;
    
    private int currentScriptIndex = 0;
    private List<MonoBehaviour> availableScripts = new List<MonoBehaviour>();
    private Dictionary<string, bool> scriptExpanded = new Dictionary<string, bool>();
    
    private bool showProfiler = false;
    private Queue<float> fpsBuffer = new Queue<float>();
    private const int BUFFER_SIZE = 100;
    private Rect profilerRect = new Rect(10, 10, 200, 100);
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            ToggleDebugMenu();
        }
        
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ToggleUIVisibility();
        }
        
        if (Input.GetKeyDown(KeyCode.F2))
        {
            TakeScreenshot();
        }
        
        if (isDebugMenuVisible)
        {
            UpdateLookTarget();
            
            if (navigationCooldown > 0)
            {
                navigationCooldown -= Time.deltaTime;
            }
            
            if (navigationCooldown <= 0)
            {
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
                
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
                {
                    ToggleCurrentSection();
                    navigationCooldown = NAVIGATION_DELAY;
                }
                
                if (showScriptsSection && currentSection == 3 && currentLookTarget != null)
                {
                    MonoBehaviour[] scripts = currentLookTarget.GetComponents<MonoBehaviour>();
                    int scriptCount = scripts.Where(s => s != null).Count();
                    
                    if (Input.GetKeyDown(KeyCode.RightArrow) && scriptCount > 0)
                    {
                        currentScriptIndex = (currentScriptIndex + 1) % scriptCount;
                        navigationCooldown = NAVIGATION_DELAY;
                    }
                    else if (Input.GetKeyDown(KeyCode.LeftArrow) && scriptCount > 0)
                    {
                        currentScriptIndex = (currentScriptIndex - 1 + scriptCount) % scriptCount;
                        navigationCooldown = NAVIGATION_DELAY;
                    }
                }
            }
        }
        
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
            case 0:
                showTransformSection = !showTransformSection;
                break;
            case 1:
                showRendererSection = !showRendererSection;
                break;
            case 2:
                showPhysicsSection = !showPhysicsSection;
                break;
            case 3:
                showScriptsSection = !showScriptsSection;
                break;
        }
    }
    
    void OnGUI()
    {
        if (isDebugMenuVisible && currentLookTarget != null && !ignoreTags.Contains(currentLookTarget.tag))
        {
            MonoBehaviour[] scripts = currentLookTarget.GetComponents<MonoBehaviour>();
            availableScripts.Clear();
            foreach (var script in scripts)
            {
                if (script != null)
                    availableScripts.Add(script);
            }
            
            if (currentScriptIndex >= availableScripts.Count)
                currentScriptIndex = 0;
            
            Vector3 worldPos = currentLookTarget.transform.position;
            
            Renderer renderer = currentLookTarget.GetComponent<Renderer>();
            if (renderer != null)
            {
                worldPos.y = renderer.bounds.max.y + 0.5f;
            }
            else
            {
                worldPos.y += 1f;
            }
            
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            
            if (screenPos.z > 0)
            {
                screenPos.y = Screen.height - screenPos.y;
                
                int boxWidth = 320;
                int lineHeight = 18;
                int padding = 10;
                int headerHeight = 25;
                
                int totalLines = 4;
                totalLines += 4;
                
                if (showTransformSection) totalLines += 3;
                if (showRendererSection && renderer != null) totalLines += 1;
                
                Rigidbody rb = currentLookTarget.GetComponent<Rigidbody>();
                if (showPhysicsSection && rb != null) totalLines += 2;
                
                if (showScriptsSection && availableScripts.Count > 0)
                {
                    totalLines += 1;
                    if (currentScriptIndex < availableScripts.Count && availableScripts[currentScriptIndex] != null)
                    {
                        var fields = availableScripts[currentScriptIndex].GetType().GetFields(
                            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                        totalLines += Mathf.Min(fields.Length, 3);
                    }
                }
                
                int boxHeight = (totalLines * lineHeight) + (padding * 2) + 10;
                
                int xPos = (int)screenPos.x - (boxWidth / 2);
                int yPos = (int)screenPos.y - boxHeight - 20;
                
                xPos = Mathf.Clamp(xPos, 10, Screen.width - boxWidth - 10);
                yPos = Mathf.Clamp(yPos, 10, Screen.height - boxHeight - 10);
                
                Color originalColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(0, 0, 0, 0.85f);
                GUI.Box(new Rect(xPos, yPos, boxWidth, boxHeight), "");
                GUI.backgroundColor = originalColor;
                
                int currentY = yPos + padding;
                
                GUI.color = Color.yellow;
                GUI.Label(new Rect(xPos + padding, currentY, boxWidth - padding * 2, 20), 
                    $"🔍 {currentLookTarget.name}");
                GUI.color = Color.white;
                currentY += 22;
                
                GUI.color = Color.cyan;
                GUI.Label(new Rect(xPos + padding, currentY, boxWidth - padding * 2, lineHeight), 
                    $"Tag: {currentLookTarget.tag}");
                GUI.color = Color.white;
                currentY += lineHeight + 5;
                
                string transformIndicator = (currentSection == 0) ? "▶ " : "  ";
                GUI.color = (currentSection == 0) ? Color.white : Color.green;
                GUI.Label(new Rect(xPos + padding, currentY, boxWidth - padding * 2, lineHeight), 
                    $"{transformIndicator}📍 Transform {(showTransformSection ? "[-]" : "[+]")}");
                GUI.color = Color.white;
                currentY += lineHeight;
                
                if (showTransformSection)
                {
                    GUI.Label(new Rect(xPos + padding + 20, currentY, boxWidth - padding * 2 - 20, lineHeight), 
                        $"Pos: {FormatVector3(currentLookTarget.transform.position)}");
                    currentY += lineHeight;
                    GUI.Label(new Rect(xPos + padding + 20, currentY, boxWidth - padding * 2 - 20, lineHeight), 
                        $"Rot: {FormatVector3(currentLookTarget.transform.eulerAngles)}");
                    currentY += lineHeight;
                    GUI.Label(new Rect(xPos + padding + 20, currentY, boxWidth - padding * 2 - 20, lineHeight), 
                        $"Scale: {FormatVector3(currentLookTarget.transform.localScale)}");
                    currentY += lineHeight;
                }
                
                if (renderer != null)
                {
                    string rendererIndicator = (currentSection == 1) ? "▶ " : "  ";
                    GUI.color = (currentSection == 1) ? Color.white : Color.magenta;
                    GUI.Label(new Rect(xPos + padding, currentY, boxWidth - padding * 2, lineHeight), 
                        $"{rendererIndicator}🎨 Renderer {(showRendererSection ? "[-]" : "[+]")}");
                    GUI.color = Color.white;
                    currentY += lineHeight;
                    
                    if (showRendererSection)
                    {
                        GUI.Label(new Rect(xPos + padding + 20, currentY, boxWidth - padding * 2 - 20, lineHeight), 
                            $"Material: {renderer.material.name}");
                        currentY += lineHeight;
                    }
                }
                
                if (rb != null)
                {
                    string physicsIndicator = (currentSection == 2) ? "▶ " : "  ";
                    GUI.color = (currentSection == 2) ? Color.white : Color.red;
                    GUI.Label(new Rect(xPos + padding, currentY, boxWidth - padding * 2, lineHeight), 
                        $"{physicsIndicator}⚡ Physics {(showPhysicsSection ? "[-]" : "[+]")}");
                    GUI.color = Color.white;
                    currentY += lineHeight;
                    
                    if (showPhysicsSection)
                    {
                        GUI.Label(new Rect(xPos + padding + 20, currentY, boxWidth - padding * 2 - 20, lineHeight), 
                            $"Velocity: {FormatVector3(rb.velocity)}");
                        currentY += lineHeight;
                        GUI.Label(new Rect(xPos + padding + 20, currentY, boxWidth - padding * 2 - 20, lineHeight), 
                            $"Mass: {rb.mass}");
                        currentY += lineHeight;
                    }
                }
                
                string scriptsIndicator = (currentSection == 3) ? "▶ " : "  ";
                GUI.color = (currentSection == 3) ? Color.white : Color.yellow;
                GUI.Label(new Rect(xPos + padding, currentY, boxWidth - padding * 2, lineHeight), 
                    $"{scriptsIndicator}📜 Scripts {(showScriptsSection ? "[-]" : "[+]")}");
                GUI.color = Color.white;
                currentY += lineHeight;
                
                if (showScriptsSection && availableScripts.Count > 0)
                {
                    if (currentScriptIndex < availableScripts.Count && availableScripts[currentScriptIndex] != null)
                    {
                        MonoBehaviour currentScript = availableScripts[currentScriptIndex];
                        GUI.Label(new Rect(xPos + padding + 20, currentY, boxWidth - padding * 2 - 20, lineHeight), 
                            $"{currentScript.GetType().Name} ({currentScriptIndex + 1}/{availableScripts.Count})");
                        currentY += lineHeight;
                        
                        var fields = currentScript.GetType().GetFields(
                            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                        
                        int fieldsShown = 0;
                        foreach (var field in fields)
                        {
                            if (fieldsShown >= 3) break;
                            
                            string value = field.GetValue(currentScript)?.ToString() ?? "null";
                            if (value.Length > 30) value = value.Substring(0, 27) + "...";
                            
                            GUI.Label(new Rect(xPos + padding + 30, currentY, boxWidth - padding * 2 - 30, lineHeight), 
                                $"{field.Name}: {value}");
                            currentY += lineHeight;
                            fieldsShown++;
                        }
                    }
                }
                
                GUI.color = Color.gray;
                GUI.Label(new Rect(xPos + padding, yPos + boxHeight - 18, boxWidth - padding * 2, 15), 
                    "F3: Toggle | ↑↓: Navigate | Space: Toggle | ←→: Scripts");
                GUI.color = Color.white;
            }
        }
        
        if (isDebugMenuVisible && (currentLookTarget == null || ignoreTags.Contains(currentLookTarget.tag)))
        {
            GUI.color = Color.gray;
            GUI.Label(new Rect(Screen.width / 2 - 100, 50, 200, 20), "Look at an object to inspect");
            GUI.color = Color.white;
        }
    }
    
    private string FormatVector3(Vector3 vector)
    {
        return $"({vector.x:F2}, {vector.y:F2}, {vector.z:F2})";
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