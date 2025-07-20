using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class KaabaTest : MonoBehaviour
{
    [Header("Testing Mode")]
    public bool enableTesting = true;
    public bool showDebugUI = true;
    public bool debugMode = true; // Easier testing
    
    [Header("Prayer Integration")]
    public bool enablePrayers = true;
    private PrayerRecitation prayerRecitation;
    
    [Header("Corner Marker Integration")]
    public bool enableCornerMarkers = true;
    private CornerMarker cornerMarker;
    
    [Header("Kaaba Settings")]
    public GameObject kaabaInstance;
    public bool kaabaCreated = false;
    public float distanceToKaaba = 0f;
    public int currentRound = 0;
    public float totalAngleChange = 0f;
    
    [Header("Movement Settings")]
    public float minimumDistance = 0.5f; // Smaller for limited space
    public float maximumDistance = 3f;   // Smaller for limited space
    public bool isValidDistance = false;
    
    [Header("Round Counting")]
    public float hajarAswadThreshold = 5f; // Threshold for detecting Hajar al-Aswad pass
    
    private Vector3 lastPosition;
    private float lastAngle = 0f;
    private bool hasPassedHajarAswad = false;
    private int hajarAswadPasses = 0;
    
    void Start()
    {
        if (enableTesting)
        {
            Debug.Log("KaabaTest: Starting in testing mode...");
            CreateKaaba();
        }
        
        // Find prayer recitation system
        if (enablePrayers)
        {
            prayerRecitation = FindFirstObjectByType<PrayerRecitation>();
            if (prayerRecitation == null)
            {
                Debug.Log("Creating PrayerRecitation component...");
                GameObject prayerObj = new GameObject("PrayerRecitation");
                prayerRecitation = prayerObj.AddComponent<PrayerRecitation>();
            }
            else
            {
                Debug.Log("Found existing PrayerRecitation component");
            }
        }
        else
        {
            Debug.Log("Prayers disabled in KaabaTest");
        }
        
        // Find corner marker system
        if (enableCornerMarkers)
        {
            cornerMarker = FindFirstObjectByType<CornerMarker>();
            if (cornerMarker == null)
            {
                Debug.Log("Creating CornerMarker component...");
                GameObject markerObj = new GameObject("CornerMarker");
                cornerMarker = markerObj.AddComponent<CornerMarker>();
            }
            else
            {
                Debug.Log("Found existing CornerMarker component");
            }
        }
        else
        {
            Debug.Log("Corner markers disabled in KaabaTest");
        }
        
        // Try to create Kaaba automatically if in testing mode
        if (enableTesting && !kaabaCreated)
        {
            Debug.Log("Testing mode enabled - creating Kaaba automatically...");
            CreateKaaba();
            if (!kaabaCreated)
            {
                Debug.Log("Automatic creation failed, trying fallback...");
                CreateSimpleKaaba();
            }
        }
    }
    
    void CreateKaaba()
    {
        // Check if Kaaba already exists
        GameObject existingKaaba = GameObject.Find("Kaaba");
        if (existingKaaba != null)
        {
            kaabaInstance = existingKaaba;
            kaabaCreated = true;
            Debug.Log("Found existing Kaaba!");
            return;
        }
        
        Debug.Log("Creating new Kaaba...");
        
        try
        {
            // Create a simple black cube for Kaaba (smaller for limited space)
            kaabaInstance = GameObject.CreatePrimitive(PrimitiveType.Cube);
            kaabaInstance.name = "Kaaba";
            kaabaInstance.transform.localScale = new Vector3(0.5f, 0.75f, 0.5f); // Much smaller
        
        // Add a subtle glow effect to make it more visible
        // We'll use a slightly lighter material for better visibility
        
        // Position it closer to camera for smaller space
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 cameraForward = Camera.main.transform.forward;
        kaabaInstance.transform.position = cameraPosition + (cameraForward * 2f); // Closer
        kaabaInstance.transform.position = new Vector3(kaabaInstance.transform.position.x, 0.5f, kaabaInstance.transform.position.z); // Lower
        
        // Make it dark gray with proper material (more visible than pure black)
        Renderer renderer = kaabaInstance.GetComponent<Renderer>();
        if (renderer != null)
        {
            try
            {
                // Try to find Standard shader, fallback to default if not found
                Shader standardShader = Shader.Find("Standard");
                if (standardShader == null)
                {
                    Debug.LogWarning("Standard shader not found, using default shader");
                    standardShader = Shader.Find("Hidden/InternalErrorShader");
                    if (standardShader == null)
                    {
                        // Last resort - use the renderer's existing material
                        Debug.LogWarning("No shader found, using existing material");
                        Material existingMaterial = renderer.material;
                        if (existingMaterial != null)
                        {
                            existingMaterial.color = new Color(0.1f, 0.1f, 0.1f, 1f); // Dark gray
                        }
                    }
                    else
                    {
                        Material kaabaMaterial = new Material(standardShader);
                        kaabaMaterial.color = new Color(0.1f, 0.1f, 0.1f, 1f); // Dark gray
                        renderer.material = kaabaMaterial;
                    }
                }
                else
                {
                    Material kaabaMaterial = new Material(standardShader);
                    kaabaMaterial.color = new Color(0.1f, 0.1f, 0.1f, 1f); // Dark gray, not pure black
                    kaabaMaterial.SetFloat("_Metallic", 0.8f); // Slightly metallic
                    kaabaMaterial.SetFloat("_Smoothness", 0.3f); // Not too shiny
                    renderer.material = kaabaMaterial;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error setting Kaaba material: {e.Message}");
                // Use default material color
                if (renderer.material != null)
                {
                    renderer.material.color = new Color(0.1f, 0.1f, 0.1f, 1f);
                }
            }
        }
        
        Debug.Log("Kaaba created with black material");
        
        kaabaCreated = true;
        lastPosition = Camera.main.transform.position;
        
        Debug.Log("Kaaba created successfully at: " + kaabaInstance.transform.position);
        Debug.Log("kaabaCreated flag set to: " + kaabaCreated);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error creating Kaaba: {e.Message}");
            kaabaCreated = false;
        }
    }
    
    void Update()
    {
        // Debug: Log status every 60 frames
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"KaabaTest Update - kaabaCreated: {kaabaCreated}, kaabaInstance: {(kaabaInstance != null ? "exists" : "null")}, enableTesting: {enableTesting}");
        }
        
        if (enableTesting && kaabaCreated && kaabaInstance != null)
        {
            TrackMovement();
        }
        else
        {
            if (Time.frameCount % 60 == 0)
            {
                Debug.Log($"Not tracking - enableTesting: {enableTesting}, kaabaCreated: {kaabaCreated}, kaabaInstance: {(kaabaInstance != null ? "exists" : "null")}");
            }
        }
        
        // Manual Kaaba creation with touch (only for initial setup)
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Debug.Log("Touch detected - attempting to create Kaaba");
            if (!kaabaCreated)
            {
                CreateKaaba();
                // If main creation failed, try fallback
                if (!kaabaCreated)
                {
                    Debug.Log("Main Kaaba creation failed, trying fallback method...");
                    CreateSimpleKaaba();
                }
            }
            else
            {
                // No manual testing - everything should work automatically
                Debug.Log("Tap detected but no manual actions needed - system works automatically");
            }
        }
    }
    
    void TrackMovement()
    {
        // Get current position
        Vector3 currentPosition = Camera.main.transform.position;
        
        // Calculate distance to Kaaba
        distanceToKaaba = Vector3.Distance(currentPosition, kaabaInstance.transform.position);
        
        // Check if in valid range
        isValidDistance = (distanceToKaaba >= minimumDistance && distanceToKaaba <= maximumDistance);
        
        // Debug: Log distance and validity
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"Distance: {distanceToKaaba:F1}m, Valid: {isValidDistance}, Min: {minimumDistance}, Max: {maximumDistance}");
        }
        
        if (isValidDistance)
        {
            // Calculate angle around Kaaba (X-Z plane only)
            Vector3 toKaaba = kaabaInstance.transform.position - currentPosition;
            Vector2 flatDirection = new Vector2(toKaaba.x, toKaaba.z);
            float currentAngle = Mathf.Atan2(flatDirection.y, flatDirection.x) * Mathf.Rad2Deg;
            
            // Calculate angle change (handle wrapping around 360 degrees)
            float angleDifference = Mathf.DeltaAngle(lastAngle, currentAngle);
            
            // Only count significant movements (filter out noise)
            if (Mathf.Abs(angleDifference) > 1f) // Minimum 1 degree change
            {
                totalAngleChange += angleDifference;
                
                // Debug logging
                if (debugMode && Time.frameCount % 30 == 0) // Log every 30 frames
                {
                    Debug.Log($"Angle: {currentAngle:F1}°, Change: {angleDifference:F1}°, Total: {totalAngleChange:F1}°");
                }
            }
            
                    // Check if passed Hajar al-Aswad (0 degrees)
        CheckHajarAswadPass();
        
        // Simple round counting: each Hajar al-Aswad pass = 1 round
        if (hajarAswadPasses > 0 && !hasPassedHajarAswad)
        {
            // Complete round when we pass Hajar al-Aswad
            Debug.Log($"Round completed! Hajar al-Aswad passes: {hajarAswadPasses}");
            CompleteRound();
            
            // Reset for next round
            hajarAswadPasses = 0;
            hasPassedHajarAswad = false;
        }
            
            lastAngle = currentAngle;
        }
        else
        {
            // Reset angle tracking if out of range
            lastAngle = 0f;
            if (Time.frameCount % 60 == 0)
            {
                Debug.Log("Out of valid range - resetting angle tracking");
            }
        }
        
        lastPosition = currentPosition;
    }
    
    void CheckHajarAswadPass()
    {
        // Calculate current angle around Kaaba
        Vector3 toKaaba = kaabaInstance.transform.position - Camera.main.transform.position;
        Vector2 flatDirection = new Vector2(toKaaba.x, toKaaba.z);
        float currentAngle = Mathf.Atan2(flatDirection.y, flatDirection.x) * Mathf.Rad2Deg;
        
        // Normalize angle to 0-360
        if (currentAngle < 0) currentAngle += 360f;
        
        // Hajar al-Aswad is at 0 degrees (or close to it)
        float hajarAswadAngle = 0f;
        
        // Check if we're near Hajar al-Aswad (much smaller threshold)
        bool nearHajarAswad = Mathf.Abs(Mathf.DeltaAngle(currentAngle, hajarAswadAngle)) < hajarAswadThreshold;
        
        // If we're near Hajar al-Aswad and haven't marked it yet
        if (nearHajarAswad && !hasPassedHajarAswad)
        {
            hasPassedHajarAswad = true;
            hajarAswadPasses++;
            Debug.Log($"Passed Hajar al-Aswad! Current angle: {currentAngle:F1}°, Passes: {hajarAswadPasses}");
        }
        // If we've moved away from Hajar al-Aswad, reset the flag
        else if (!nearHajarAswad && hasPassedHajarAswad)
        {
            hasPassedHajarAswad = false;
        }
        
        // Debug logging
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"Hajar al-Aswad Check - Angle: {currentAngle:F1}°, Near: {nearHajarAswad}, Passes: {hajarAswadPasses}");
        }
    }
    
    // Removed complex CheckRoundCompletion method - using simple Hajar al-Aswad pass counting instead
    
    void CompleteRound()
    {
        currentRound++;
        Debug.Log($"Round {currentRound} completed! Distance: {distanceToKaaba:F1}m");
        
        // Play round completion prayer
        if (enablePrayers && prayerRecitation != null)
        {
            prayerRecitation.PlayRoundCompletionPrayer(currentRound);
        }
        
        if (currentRound >= 7)
        {
            Debug.Log("🎉 TAWAF COMPLETED! All 7 rounds finished!");
            
            // Play Tawaf completion prayer
            if (enablePrayers && prayerRecitation != null)
            {
                prayerRecitation.PlayTawafCompletionPrayer();
            }
        }
    }
    
    private float completionDisplayTime = 0f;
    private bool showCompletion = false;
    
    void OnGUI()
    {
        if (!showDebugUI) return;
        
        // Clean, professional UI
        GUI.skin.label.fontSize = 18;
        
        int y = 20;
        int lineHeight = 25;
        
        // Main title
        GUI.color = new Color(0.2f, 0.6f, 1f); // Blue
        GUI.Label(new Rect(20, y, 400, lineHeight), "🕌 Tawaf AR Trainer");
        y += lineHeight + 10;
        
        if (kaabaCreated)
        {
            // Round counter - main focus
            GUI.color = Color.white;
            GUI.skin.label.fontSize = 24;
            GUI.Label(new Rect(20, y, 400, lineHeight), $"Round {currentRound}/7");
            y += lineHeight + 5;
            
            // Progress bar background
            GUI.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);
            GUI.Box(new Rect(20, y, 300, 15), "");
            
            // Progress bar fill
            float progress = (float)currentRound / 7f;
            GUI.color = new Color(0.2f, 0.8f, 0.2f, 0.9f); // Green
            GUI.Box(new Rect(20, y, 300 * progress, 15), "");
            
            y += lineHeight + 10;
            
            // Status indicators
            GUI.skin.label.fontSize = 16;
            
            // Distance indicator
            GUI.color = isValidDistance ? new Color(0.2f, 0.8f, 0.2f) : new Color(0.8f, 0.2f, 0.2f);
            GUI.Label(new Rect(20, y, 400, lineHeight), $"Distance: {distanceToKaaba:F1}m");
            y += lineHeight;
            
            // Current corner
            if (enableCornerMarkers && cornerMarker != null && cornerMarker.IsNearCorner())
            {
                GUI.color = new Color(1f, 0.8f, 0.2f); // Gold
                GUI.Label(new Rect(20, y, 400, lineHeight), $"📍 {cornerMarker.GetCurrentCornerName()}");
                y += lineHeight;
            }
            
            // Near Hajar al-Aswad indicator
            Vector3 toKaaba = kaabaInstance.transform.position - Camera.main.transform.position;
            Vector2 flatDirection = new Vector2(toKaaba.x, toKaaba.z);
            float currentAngle = Mathf.Atan2(flatDirection.y, flatDirection.x) * Mathf.Rad2Deg;
            if (currentAngle < 0) currentAngle += 360f;
            bool nearHajarAswad = Mathf.Abs(Mathf.DeltaAngle(currentAngle, 0f)) < hajarAswadThreshold;
            
            if (nearHajarAswad)
            {
                GUI.color = new Color(0.8f, 0.2f, 0.2f); // Red for Hajar al-Aswad
                GUI.Label(new Rect(20, y, 400, lineHeight), "🖤 Hajar al-Aswad");
                y += lineHeight;
            }
        }
        else
        {
            // Setup instructions
            GUI.color = Color.white;
            GUI.Label(new Rect(20, y, 400, lineHeight), "Tap to create Kaaba");
            y += lineHeight;
            
            GUI.color = new Color(0.7f, 0.7f, 0.7f);
            GUI.Label(new Rect(20, y, 400, lineHeight), "Point camera at floor, then tap screen");
        }
        
        // Bottom instructions
        y = Screen.height - 80;
        GUI.color = new Color(0.6f, 0.6f, 0.6f, 0.8f);
        GUI.skin.label.fontSize = 14;
        GUI.Label(new Rect(20, y, 400, lineHeight), "Walk around the Kaaba in circles");
        y += lineHeight;
        GUI.Label(new Rect(20, y, 400, lineHeight), "Keep 0.5-3m distance for best tracking");
        
        // Tawaf completion celebration
        if (currentRound >= 7)
        {
            // Semi-transparent overlay
            GUI.color = new Color(0, 0, 0, 0.7f);
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
            
            // Celebration text
            GUI.color = new Color(1f, 0.8f, 0.2f); // Gold
            GUI.skin.label.fontSize = 32;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(0, Screen.height/2 - 100, Screen.width, 50), "🎉 TAWAF COMPLETED! 🎉");
            
            GUI.color = Color.white;
            GUI.skin.label.fontSize = 20;
            GUI.Label(new Rect(0, Screen.height/2 - 50, Screen.width, 30), "All 7 rounds finished successfully");
            
            GUI.color = new Color(0.8f, 0.8f, 0.8f);
            GUI.skin.label.fontSize = 16;
            GUI.Label(new Rect(0, Screen.height/2, Screen.width, 25), "May Allah accept your Tawaf");
        }
    }
    }
    
    // Public methods for integration with existing scripts
    public GameObject GetKaaba()
    {
        return kaabaInstance;
    }
    
    public bool IsKaabaCreated()
    {
        return kaabaCreated;
    }
    
    public int GetCurrentRound()
    {
        return currentRound;
    }
    
    public float GetDistanceToKaaba()
    {
        return distanceToKaaba;
    }
    
    // Fallback method to create a simple Kaaba without complex components
    void CreateSimpleKaaba()
    {
        Debug.Log("Creating simple Kaaba fallback...");
        
        try
        {
            // Create a basic GameObject
            kaabaInstance = new GameObject("Kaaba");
            
            // Add a simple mesh renderer with a cube mesh
            MeshFilter meshFilter = kaabaInstance.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = kaabaInstance.AddComponent<MeshRenderer>();
            
            // Create a simple cube mesh
            Mesh cubeMesh = new Mesh();
            cubeMesh.vertices = new Vector3[]
            {
                new Vector3(-0.25f, -0.375f, -0.25f), // 0
                new Vector3(0.25f, -0.375f, -0.25f),  // 1
                new Vector3(0.25f, 0.375f, -0.25f),   // 2
                new Vector3(-0.25f, 0.375f, -0.25f),  // 3
                new Vector3(-0.25f, -0.375f, 0.25f),  // 4
                new Vector3(0.25f, -0.375f, 0.25f),   // 5
                new Vector3(0.25f, 0.375f, 0.25f),    // 6
                new Vector3(-0.25f, 0.375f, 0.25f)    // 7
            };
            
            cubeMesh.triangles = new int[]
            {
                0, 2, 1, 0, 3, 2, // front
                1, 6, 5, 1, 2, 6, // right
                5, 7, 4, 5, 6, 7, // back
                4, 3, 0, 4, 7, 3, // left
                3, 6, 2, 3, 7, 6, // top
                0, 5, 4, 0, 1, 5  // bottom
            };
            
            cubeMesh.RecalculateNormals();
            meshFilter.mesh = cubeMesh;
            
            // Position it
            Vector3 cameraPosition = Camera.main.transform.position;
            Vector3 cameraForward = Camera.main.transform.forward;
            kaabaInstance.transform.position = cameraPosition + (cameraForward * 2f);
            kaabaInstance.transform.position = new Vector3(kaabaInstance.transform.position.x, 0.5f, kaabaInstance.transform.position.z);
            
            // Set a simple color
            Material simpleMaterial = new Material(Shader.Find("Hidden/InternalErrorShader"));
            if (simpleMaterial != null)
            {
                simpleMaterial.color = new Color(0.1f, 0.1f, 0.1f, 1f);
                meshRenderer.material = simpleMaterial;
            }
            
            kaabaCreated = true;
            lastPosition = Camera.main.transform.position;
            
            Debug.Log("Simple Kaaba created successfully!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error creating simple Kaaba: {e.Message}");
            kaabaCreated = false;
        }
    }
} 