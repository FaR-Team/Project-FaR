using UnityEngine;
using System.Linq;
using UnityEditor;
using System;
using System.Collections.Generic;

public class DebugMenu : MonoBehaviour
{
    private bool isDebugMenuVisible = false;
    private bool isUIVisible = true;
    private GameObject currentLookTarget;
    public string[] ignoreTags = { "Player", "MainCamera", "UI", "Violeta", "Pared"};

    private bool showProfiler = false;
    private Queue<float> fpsBuffer = new Queue<float>();
    private const int BUFFER_SIZE = 100;
    private Rect profilerRect = new Rect(10, 10, 200, 100);

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.F1))
        {
            ToggleUIVisibility();
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            TakeScreenshot();
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            ToggleDebugMenu();
        }

        if (showProfiler)
        {
            float fps = 1.0f / Time.deltaTime;
            fpsBuffer.Enqueue(fps);
            if (fpsBuffer.Count > BUFFER_SIZE)
                fpsBuffer.Dequeue();
        }
        
        if (isDebugMenuVisible)
        {
            UpdateLookTarget();
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

            int contentHeight = CalculateContentHeight();
            int windowHeight = Mathf.Max(baseHeight, contentHeight);

            GUI.Box(new Rect(xPosition, yPosition, windowWidth, windowHeight), "Debug Menu");
        
            if (currentLookTarget != null && !ignoreTags.Contains(currentLookTarget.tag))
            {
                GUI.Label(new Rect(xPosition + 10, yPosition + 30, windowWidth - 20, 20), $"Name: {currentLookTarget.name}");
                GUI.Label(new Rect(xPosition + 10, yPosition + 50, windowWidth - 20, 20), $"Tag: {currentLookTarget.tag}");
                GUI.Label(new Rect(xPosition + 10, yPosition + 70, windowWidth - 20, 20), $"Position: {currentLookTarget.transform.position}");
                Renderer renderer = currentLookTarget.GetComponent<Renderer>();
                if (renderer != null)
                {
                    GUI.Label(new Rect(xPosition + 10, yPosition + 90, windowWidth - 20, 20), $"Material: {renderer.material.name}");
                }

                Rigidbody rb = currentLookTarget.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    GUI.Label(new Rect(xPosition + 10, yPosition + 110, windowWidth - 20, 20), $"Velocity: {rb.velocity}");
                }

                // Add script fields information
                string scriptFields = GetScriptFields(currentLookTarget);
                Vector2 scrollPosition = Vector2.zero;
                scrollPosition = GUI.BeginScrollView(new Rect(xPosition + 10, yPosition + 150, windowWidth - 20, windowHeight - 200), scrollPosition, new Rect(0, 0, windowWidth - 40, GUI.skin.label.CalcHeight(new GUIContent(scriptFields), windowWidth - 40)));
                GUI.Label(new Rect(0, 0, windowWidth - 40, GUI.skin.label.CalcHeight(new GUIContent(scriptFields), windowWidth - 40)), scriptFields);
                GUI.EndScrollView();

                // Display GrowingBase information
                GrowingBase growingBase = currentLookTarget.GetComponent<GrowingBase>();
                if (growingBase != null)
                {
                    GUI.Label(new Rect(xPosition + 10, yPosition + 1700, windowWidth - 20, 20), "GrowingBase Script Found:");
                    string growingBaseInfo = GetScriptFields(currentLookTarget);
                    Vector2 growingBaseScrollPosition = Vector2.zero;
                    growingBaseScrollPosition = GUI.BeginScrollView(new Rect(xPosition + 10, yPosition + 190, windowWidth - 20, windowHeight - 240), growingBaseScrollPosition, new Rect(0, 0, windowWidth - 40, GUI.skin.label.CalcHeight(new GUIContent(growingBaseInfo), windowWidth - 40)));
                    GUI.Label(new Rect(0, 0, windowWidth - 40, GUI.skin.label.CalcHeight(new GUIContent(growingBaseInfo), windowWidth - 40)), growingBaseInfo);
                    GUI.EndScrollView();
                }
            }
            else
            {
                GUI.Label(new Rect(xPosition + 10, yPosition + 30, windowWidth - 20, 20), "Not looking at any relevant object");
            }
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
        
    private string GetScriptFields(GameObject obj)
    {
        string fieldInfo = "";
        MonoBehaviour[] scripts = obj.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            fieldInfo += $"\nScript: {script.GetType().Name}\n";
            System.Reflection.FieldInfo[] fields = script.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var field in fields)
            {
                fieldInfo += $"  {field.Name}: {field.GetValue(script)}\n";
            }
        }
        return fieldInfo;
    }

    private int CalculateContentHeight()
    {
        int height = 30; // Start with base height for title
        if (currentLookTarget != null && !ignoreTags.Contains(currentLookTarget.tag))
        {
            height += 140; // Base height for standard fields

            Renderer renderer = currentLookTarget.GetComponent<Renderer>();
            if (renderer != null) height += 20;

            Rigidbody rb = currentLookTarget.GetComponent<Rigidbody>();
            if (rb != null) height += 40;

            // Calculate height for script fields
            string scriptFields = GetScriptFields(currentLookTarget);
            string[] lines = scriptFields.Split('\n');
            height += lines.Length * 20;
        }
        else
        {
            height += 20; // Height for "Not looking at any relevant object" message
        }

        return height + 20;
    }

    private void TakeScreenshot()
    {
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        FaREditorUtils.SaveScreenshot(timestamp);
    }
}

