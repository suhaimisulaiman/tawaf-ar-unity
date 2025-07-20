using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARSceneSetup : MonoBehaviour
{
    [Header("AR Components")]
    public ARSession arSession;
    public ARSessionOrigin arSessionOrigin;
    public ARCameraManager arCameraManager;
    public ARPlaneManager arPlaneManager;
    
    [Header("Tracking Components")]
    public ARMovementTracker movementTracker;
    
    void Awake()
    {
        SetupARComponents();
    }
    
    void SetupARComponents()
    {
        // Create AR Session if it doesn't exist
        if (arSession == null)
        {
            GameObject sessionObj = new GameObject("AR Session");
            arSession = sessionObj.AddComponent<ARSession>();
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
            }
            
            // Add AR Camera Manager
            arCameraManager = arCamera.gameObject.AddComponent<ARCameraManager>();
            
            // Add AR Plane Manager for better tracking
            arPlaneManager = originObj.AddComponent<ARPlaneManager>();
        }
        
        // Create Movement Tracker if it doesn't exist
        if (movementTracker == null)
        {
            GameObject trackerObj = new GameObject("AR Movement Tracker");
            movementTracker = trackerObj.AddComponent<ARMovementTracker>();
            
            // Connect AR components to the tracker
            movementTracker.arSession = arSession;
            movementTracker.arSessionOrigin = arSessionOrigin;
            movementTracker.arCameraManager = arCameraManager;
        }
        
        Debug.Log("AR Scene setup complete!");
    }
    
    void Start()
    {
        // Enable device sensors
        Input.gyro.enabled = true;
        
        // Check AR availability
        CheckARAvailability();
    }
    
    void CheckARAvailability()
    {
        if (ARSession.state == ARSessionState.None)
        {
            Debug.Log("AR Session not started. Starting AR...");
            arSession.enabled = true;
        }
        
        Debug.Log($"AR Session State: {ARSession.state}");
        Debug.Log($"Gyro Enabled: {Input.gyro.enabled}");
        Debug.Log($"Accelerometer Available: {SystemInfo.supportsAccelerometer}");
    }
    
    void Update()
    {
        // Monitor AR session state
        if (ARSession.state == ARSessionState.Unsupported)
        {
            Debug.LogError("AR is not supported on this device!");
        }
        else if (ARSession.state == ARSessionState.None)
        {
            Debug.LogWarning("AR Session not initialized.");
        }
    }
} 