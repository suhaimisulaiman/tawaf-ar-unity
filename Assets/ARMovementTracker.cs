using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARMovementTracker : MonoBehaviour
{
    [Header("AR Components")]
    public ARSession arSession;
    public ARSessionOrigin arSessionOrigin;
    public ARCameraManager arCameraManager;
    
    [Header("Kaaba Settings")]
    public GameObject kaabaPrefab;
    public Vector3 kaabaSize = new Vector3(2f, 3f, 2f);
    
    [Header("Movement Settings")]
    public float minimumDistance = 2f;
    public float maximumDistance = 8f;
    public float idealDistance = 4f;
    
    [Header("Round Tracking")]
    public int currentRound = 0;
    public int totalRounds = 7;
    
    private GameObject kaabaInstance;
    private Vector3 playerPosition;
    private float lastAngle = 0f;
    private float totalAngleChange = 0f;
    private bool isValidDistance = false;
    private bool kaabaPlaced = false;
    
    void Start()
    {
        SetupAR();
        CreateKaaba();
    }
    
    void SetupAR()
    {
        // Find or create AR components
        if (arSession == null)
            arSession = FindFirstObjectByType<ARSession>();
        if (arSessionOrigin == null)
            arSessionOrigin = FindFirstObjectByType<ARSessionOrigin>();
        if (arCameraManager == null)
            arCameraManager = FindFirstObjectByType<ARCameraManager>();
            
        if (arSession == null || arSessionOrigin == null || arCameraManager == null)
        {
            Debug.LogError("AR components not found! Make sure AR Session and AR Session Origin are in the scene.");
        }
    }
    
    void CreateKaaba()
    {
        // Create Kaaba as a simple cube for now
        kaabaInstance = GameObject.CreatePrimitive(PrimitiveType.Cube);
        kaabaInstance.name = "Kaaba";
        kaabaInstance.transform.localScale = kaabaSize;
        
        // Position it in front of the camera
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 cameraForward = Camera.main.transform.forward;
        kaabaInstance.transform.position = cameraPosition + (cameraForward * 5f);
        kaabaInstance.transform.position = new Vector3(kaabaInstance.transform.position.x, 1.5f, kaabaInstance.transform.position.z);
        
        // Make it black
        Renderer renderer = kaabaInstance.GetComponent<Renderer>();
        renderer.material.color = Color.black;
        
        kaabaPlaced = true;
        Debug.Log("AR Kaaba created!");
    }
    
    void Update()
    {
        if (kaabaPlaced && kaabaInstance != null)
        {
            TrackRealMovement();
        }
        
        // Optional: Keep some debug controls for testing
        HandleDebugInput();
    }
    
    void HandleDebugInput()
    {
        // Only enable debug controls in editor, not on device
        #if UNITY_EDITOR
        // Debug controls for testing in editor
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Auto-completing round for testing!");
            CompleteRound();
        }
        #endif
    }
    
    void TrackRealMovement()
    {
        // Get real camera position (this is the phone's position in real world)
        playerPosition = Camera.main.transform.position;
        
        // Calculate distance to Kaaba
        Vector3 toKaaba = kaabaInstance.transform.position - playerPosition;
        float distance = toKaaba.magnitude;
        
        // Check if player is in valid range
        isValidDistance = (distance >= minimumDistance && distance <= maximumDistance);
        
        if (isValidDistance)
        {
            // Calculate angle around Kaaba (X-Z plane only)
            Vector2 flatDirection = new Vector2(toKaaba.x, toKaaba.z);
            float currentAngle = Mathf.Atan2(flatDirection.y, flatDirection.x) * Mathf.Rad2Deg;
            
            // Calculate angle change
            float angleDifference = Mathf.DeltaAngle(lastAngle, currentAngle);
            totalAngleChange += angleDifference;
            
            // Debug info (less frequent)
            if (Time.frameCount % 60 == 0)
            {
                Debug.Log($"Real Movement - Distance: {distance:F1}m, Valid: {isValidDistance}, Angle Change: {totalAngleChange:F1}°, Current Angle: {currentAngle:F1}°");
            }
            
            // Check if completed a full round (360 degrees)
            if (Mathf.Abs(totalAngleChange) >= 300f) // 300 degrees for easier testing
            {
                CompleteRound();
                totalAngleChange = 0f;
            }
            
            lastAngle = currentAngle;
        }
        else
        {
            if (Time.frameCount % 60 == 0)
            {
                Debug.Log($"Real Movement - Distance: {distance:F1}m - TOO FAR! Need to be between {minimumDistance}-{maximumDistance}m");
            }
        }
    }
    
    void CompleteRound()
    {
        currentRound++;
        Debug.Log($"Round {currentRound} completed! ({totalRounds - currentRound} remaining)");
        
        if (currentRound >= totalRounds)
        {
            Debug.Log("🎉 TAWAF COMPLETED! All 7 rounds finished!");
        }
    }
    
    // Method to get device movement data
    public Vector3 GetDeviceMovement()
    {
        // Get accelerometer data
        Vector3 acceleration = Input.acceleration;
        
        // Get gyroscope data if available
        Vector3 rotationRate = Vector3.zero;
        if (Input.gyro.enabled)
        {
            rotationRate = Input.gyro.rotationRate;
        }
        
        return acceleration;
    }
    
    // Method to check if user is moving
    public bool IsUserMoving()
    {
        Vector3 movement = GetDeviceMovement();
        return movement.magnitude > 0.1f; // Threshold for movement detection
    }
    
    void OnGUI()
    {
        // Simple UI for testing
        GUI.Label(new Rect(10, 10, 300, 20), $"Round: {currentRound}/{totalRounds}");
        GUI.Label(new Rect(10, 30, 300, 20), $"Distance: {Vector3.Distance(playerPosition, kaabaInstance.transform.position):F1}m");
        GUI.Label(new Rect(10, 50, 300, 20), $"Valid Range: {isValidDistance}");
        GUI.Label(new Rect(10, 70, 300, 20), $"Angle Change: {totalAngleChange:F1}°");
        GUI.Label(new Rect(10, 90, 300, 20), $"Device Moving: {IsUserMoving()}");
        
        // Show instructions
        GUI.Label(new Rect(10, 120, 400, 60), "Move your phone around the Kaaba to complete Tawaf!\nKeep 2-8 meters distance from the Kaaba.");
    }
} 