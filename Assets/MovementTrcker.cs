using UnityEngine;

public class MovementTracker : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform kaabaTransform;
    public float minimumDistance = 2f; // Minimum distance to count as circling
    public float maximumDistance = 8f; // Maximum distance to count as circling
    
    [Header("Round Tracking")]
    public int currentRound = 0;
    public int totalRounds = 7;
    
    private Vector3 playerPosition;
    private float lastAngle = 0f;
    private float totalAngleChange = 0f;
    private bool isValidDistance = false;
    
    void Start()
    {
        // Find the Kaaba automatically
        GameObject kaaba = GameObject.Find("Kaaba");
        if (kaaba != null)
        {
            kaabaTransform = kaaba.transform;
            Debug.Log("Found Kaaba for tracking!");
        }
    }
    
    void Update()
    {
        TrackMovement();
        
        // Debug controls (for testing in editor)
        HandleDebugInput();
    }
    
    void TrackMovement()
    {
        // Get current position (camera represents player)
        playerPosition = Camera.main.transform.position;
        
        // Calculate distance to Kaaba
        Vector3 toKaaba = kaabaTransform.position - playerPosition;
        float distance = toKaaba.magnitude;
        
        // Check if player is in valid range for Tawaf
        isValidDistance = (distance >= minimumDistance && distance <= maximumDistance);
        
        if (isValidDistance)
        {
            // Calculate angle around Kaaba (only X-Z plane, ignore Y)
            Vector2 flatDirection = new Vector2(toKaaba.x, toKaaba.z);
            float currentAngle = Mathf.Atan2(flatDirection.y, flatDirection.x) * Mathf.Rad2Deg;
            
            // Calculate angle change
            float angleDifference = Mathf.DeltaAngle(lastAngle, currentAngle);
            totalAngleChange += angleDifference;
            
            // DEBUG: Show what's happening (less frequent to avoid spam)
            if (Time.frameCount % 60 == 0) // Log every 60 frames (about once per second)
            {
                Debug.Log($"Distance: {distance:F1}m, Valid: {isValidDistance}, Angle Change: {totalAngleChange:F1}°, Current Angle: {currentAngle:F1}°");
            }
            
            // Check if completed a full round (360 degrees) - make it easier for testing
            if (Mathf.Abs(totalAngleChange) >= 300f) // Reduced from 360 to 300 for easier testing
            {
                CompleteRound();
                totalAngleChange = 0f; // Reset for next round
            }
            
            lastAngle = currentAngle;
        }
        else
        {
            if (Time.frameCount % 60 == 0) // Log every 60 frames to avoid spam
            {
                Debug.Log($"Distance: {distance:F1}m - TOO FAR! Need to be between {minimumDistance}-{maximumDistance}m");
            }
        }
    }
    
    void CompleteRound()
    {
        currentRound++;
        Debug.Log($"Round {currentRound} completed! ({totalRounds - currentRound} remaining)");
        Debug.Log($"MovementTracker currentRound is now: {currentRound}");
        
        if (currentRound >= totalRounds)
        {
            Debug.Log("🎉 TAWAF COMPLETED! All 7 rounds finished!");
            // Could trigger celebration, prayer completion, etc.
        }
    }
    
    void HandleDebugInput()
    {
        // WASD movement 
        float speed = 3f;
        Vector3 movement = Vector3.zero;
        
        if (Input.GetKey(KeyCode.W)) movement += Camera.main.transform.forward;
        if (Input.GetKey(KeyCode.S)) movement -= Camera.main.transform.forward;
        if (Input.GetKey(KeyCode.A)) movement -= Camera.main.transform.right;
        if (Input.GetKey(KeyCode.D)) movement += Camera.main.transform.right;
        
        // Apply movement
        Camera.main.transform.position += movement * speed * Time.deltaTime;
        
        // ALWAYS look at Kaaba (this solves your problem!)
        if (kaabaTransform != null)
        {
            Camera.main.transform.LookAt(kaabaTransform.position);
        }
        
        // Orbital movement (easier for Tawaf!)
        if (Input.GetKey(KeyCode.Q)) // Rotate left around Kaaba
        {
            RotateAroundKaaba(-30f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.E)) // Rotate right around Kaaba  
        {
            RotateAroundKaaba(30f * Time.deltaTime);
        }
        
        // Auto-complete round for testing
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Auto-completing round for testing!");
            CompleteRound();
        }
    }

    // New method for smooth orbital movement
    void RotateAroundKaaba(float degrees)
    {
        if (kaabaTransform != null)
        {
            // Get current distance to Kaaba
            float currentDistance = Vector3.Distance(Camera.main.transform.position, kaabaTransform.position);
            
            // If too far, move closer first
            if (currentDistance > maximumDistance)
            {
                Vector3 directionToKaaba = (kaabaTransform.position - Camera.main.transform.position).normalized;
                Camera.main.transform.position += directionToKaaba * 2f * Time.deltaTime;
            }
            
            // Rotate around Kaaba
            Camera.main.transform.RotateAround(kaabaTransform.position, Vector3.up, degrees);
            Camera.main.transform.LookAt(kaabaTransform.position);
            
            // Ensure we maintain proper distance (4m is ideal)
            float targetDistance = 4f;
            Vector3 toKaaba = kaabaTransform.position - Camera.main.transform.position;
            float actualDistance = toKaaba.magnitude;
            
            if (Mathf.Abs(actualDistance - targetDistance) > 0.5f)
            {
                Vector3 newPosition = kaabaTransform.position - (toKaaba.normalized * targetDistance);
                Camera.main.transform.position = newPosition;
            }
        }
    }
    
    void OnGUI()
    {
        // Simple UI for testing - this will show in browser
        GUI.Label(new Rect(10, 10, 300, 20), $"Round: {currentRound}/{totalRounds}");
        GUI.Label(new Rect(10, 30, 300, 20), $"Distance: {Vector3.Distance(playerPosition, kaabaTransform.position):F1}m");
        GUI.Label(new Rect(10, 50, 300, 20), $"Valid Range: {isValidDistance}");
        GUI.Label(new Rect(10, 70, 300, 20), $"Angle Change: {totalAngleChange:F1}°");
        GUI.Label(new Rect(10, 90, 300, 20), $"Last Angle: {lastAngle:F1}°");
        
        // Show instructions
        GUI.Label(new Rect(10, 120, 400, 40), "Press Q to rotate left, E to rotate right, C to test round completion");
    }
}