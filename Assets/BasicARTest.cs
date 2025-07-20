using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class BasicARTest : MonoBehaviour
{
    public bool scriptRunning = false;
    public string status = "Script not started";
    
    void Start()
    {
        scriptRunning = true;
        status = "Script started successfully!";
        Debug.Log("BasicARTest: Script is running!");
    }
    
    void Update()
    {
        // Update status every frame
        status = $"Script running: {scriptRunning}, Time: {Time.time:F1}s";
        
        // Check for touch input
        if (Input.touchCount > 0)
        {
            status = "Touch detected!";
        }
    }
    
    void OnGUI()
    {
        // Very simple GUI - just to test if GUI works at all
        GUI.color = Color.red;
        GUI.Label(new Rect(50, 50, 300, 50), "BASIC AR TEST");
        
        GUI.color = Color.white;
        GUI.Label(new Rect(50, 100, 300, 50), status);
        
        GUI.color = Color.yellow;
        GUI.Label(new Rect(50, 150, 300, 50), "If you see this, GUI is working!");
        
        // Add a button
        if (GUI.Button(new Rect(50, 200, 200, 50), "Test Button"))
        {
            status = "Button pressed!";
        }
    }
} 