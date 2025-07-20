using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SimpleARTest : MonoBehaviour
{
    [Header("AR Components")]
    public ARSession arSession;
    public ARSessionOrigin arSessionOrigin;
    public ARCameraManager arCameraManager;
    
    [Header("Debug Info")]
    public bool arSessionStarted = false;
    public bool kaabaCreated = false;
    public string debugMessage = "Starting AR...";
    
    private GameObject kaabaInstance;
    
    void Start()
    {
        Debug.Log("SimpleARTest starting...");
        SetupAR();
    }
    
    void SetupAR()
    {
        debugMessage = "Setting up AR components...";
        
        // Create AR Session if it doesn't exist
        if (arSession == null)
        {
            GameObject sessionObj = new GameObject("AR Session");
            arSession = sessionObj.AddComponent<ARSession>();
            Debug.Log("Created AR Session");
        }
        
        // Create AR Session Origin if it doesn't exist
        if (arSessionOrigin == null)
        {
            GameObject originObj = new GameObject("AR Session Origin");
            arSessionOrigin = originObj.AddComponent<ARSessionOrigin>();
            
            // Set up camera
            Camera arCamera = originObj.GetComponentInChildren<Camera>();
            if (arCamera == null)
            {
                GameObject cameraObj = new GameObject("AR Camera");
                cameraObj.transform.SetParent(originObj.transform);
                arCamera = cameraObj.AddComponent<Camera>();
                arCamera.tag = "MainCamera";
                Debug.Log("Created AR Camera");
            }
            
            // Add AR Camera Manager
            arCameraManager = arCamera.gameObject.AddComponent<ARCameraManager>();
            Debug.Log("Added AR Camera Manager");
        }
        
        debugMessage = "AR components created. Starting session...";
        
        // Start AR session
        if (arSession != null)
        {
            arSession.enabled = true;
            arSessionStarted = true;
            debugMessage = "AR Session started!";
            Debug.Log("AR Session enabled");
        }
        
        // Create Kaaba after a short delay
        Invoke("CreateKaaba", 2f);
    }
    
    void CreateKaaba()
    {
        debugMessage = "Creating Kaaba...";
        
        // Create a simple cube for Kaaba
        kaabaInstance = GameObject.CreatePrimitive(PrimitiveType.Cube);
        kaabaInstance.name = "Kaaba";
        kaabaInstance.transform.localScale = new Vector3(2f, 3f, 2f);
        
        // Position it in front of the camera
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 cameraForward = Camera.main.transform.forward;
        kaabaInstance.transform.position = cameraPosition + (cameraForward * 5f);
        kaabaInstance.transform.position = new Vector3(kaabaInstance.transform.position.x, 1.5f, kaabaInstance.transform.position.z);
        
        // Make it black
        Renderer renderer = kaabaInstance.GetComponent<Renderer>();
        renderer.material.color = Color.black;
        
        kaabaCreated = true;
        debugMessage = "Kaaba created! Look for black cube in front of you.";
        Debug.Log("Kaaba created at position: " + kaabaInstance.transform.position);
    }
    
    void Update()
    {
        // Monitor AR session state
        if (arSession != null)
        {
            debugMessage = $"AR Session State: {ARSession.state}";
            
            if (ARSession.state == ARSessionState.Unsupported)
            {
                debugMessage = "ERROR: AR not supported on this device!";
            }
            else if (ARSession.state == ARSessionState.Ready)
            {
                debugMessage = "AR Ready! Kaaba should be visible.";
            }
        }
        
        // Add some debug controls
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // Tap to create Kaaba manually
            if (!kaabaCreated)
            {
                CreateKaaba();
            }
        }
    }
    
    void OnGUI()
    {
        // Use larger font and better positioning for iPhone
        GUI.skin.label.fontSize = 20;
        GUI.skin.label.normal.textColor = Color.white;
        
        // Position text in center of screen, not top-left
        int startY = 100; // Start lower to avoid notch/cutoff
        int lineHeight = 30;
        int textWidth = 500;
        
        // Center the text horizontally
        int screenWidth = Screen.width;
        int xPos = (screenWidth - textWidth) / 2;
        
        // Show debug information on screen
        GUI.Label(new Rect(xPos, startY, textWidth, lineHeight), $"AR Session Started: {arSessionStarted}");
        GUI.Label(new Rect(xPos, startY + lineHeight, textWidth, lineHeight), $"Kaaba Created: {kaabaCreated}");
        GUI.Label(new Rect(xPos, startY + lineHeight * 2, textWidth, lineHeight), $"AR State: {ARSession.state}");
        GUI.Label(new Rect(xPos, startY + lineHeight * 3, textWidth, lineHeight), $"Debug: {debugMessage}");
        GUI.Label(new Rect(xPos, startY + lineHeight * 4, textWidth, lineHeight), "Tap screen to create Kaaba manually");
        
        if (kaabaCreated && kaabaInstance != null)
        {
            float distance = Vector3.Distance(Camera.main.transform.position, kaabaInstance.transform.position);
            GUI.Label(new Rect(xPos, startY + lineHeight * 5, textWidth, lineHeight), $"Distance to Kaaba: {distance:F1}m");
        }
        
        // Add a simple instruction at the bottom
        GUI.Label(new Rect(xPos, Screen.height - 100, textWidth, lineHeight), "Look for a BLACK CUBE in front of you!");
    }
} 