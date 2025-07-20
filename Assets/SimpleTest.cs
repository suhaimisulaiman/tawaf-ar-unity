using UnityEngine;

public class SimpleTest : MonoBehaviour
{
    public bool isWorking = false;
    
    void Start()
    {
        isWorking = true;
        Debug.Log("SimpleTest: Script is running!");
    }
    
    void Update()
    {
        // Test touch input
        if (Input.touchCount > 0)
        {
            Debug.Log("Touch detected!");
        }
    }
    
    void OnGUI()
    {
        GUI.color = Color.red;
        GUI.Label(new Rect(50, 50, 300, 50), "SIMPLE TEST - IF YOU SEE THIS, GUI WORKS!");
        
        GUI.color = Color.white;
        GUI.Label(new Rect(50, 100, 300, 50), $"Script Working: {isWorking}");
        GUI.Label(new Rect(50, 150, 300, 50), "Tap screen to test touch");
        
        if (GUI.Button(new Rect(50, 200, 200, 50), "Test Button"))
        {
            Debug.Log("Button pressed!");
        }
    }
} 