using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class KaabaTest : MonoBehaviour
{
    [Header("Testing Mode")]
    public bool enableTesting = true;
    public bool showDebugUI = true;
    public bool debugMode = false; // Clean experience
    
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
            CreateKaaba();
        }
        
        // Find prayer recitation system
        if (enablePrayers)
        {
            prayerRecitation = FindFirstObjectByType<PrayerRecitation>();
            if (prayerRecitation == null)
            {
                GameObject prayerObj = new GameObject("PrayerRecitation");
                prayerRecitation = prayerObj.AddComponent<PrayerRecitation>();
            }
        }
        
        // Find corner marker system
        if (enableCornerMarkers)
        {
            cornerMarker = FindFirstObjectByType<CornerMarker>();
            if (cornerMarker == null)
            {
                GameObject markerObj = new GameObject("CornerMarker");
                cornerMarker = markerObj.AddComponent<CornerMarker>();
            }
        }
        
        // Try to create Kaaba automatically if in testing mode
        if (enableTesting && !kaabaCreated)
        {
            CreateKaaba();
            if (!kaabaCreated)
            {
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
            return;
        }
        
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
        
        kaabaCreated = true;
        lastPosition = Camera.main.transform.position;
        }
        catch (System.Exception e)
        {
            kaabaCreated = false;
        }
    }
    
    void Update()
    {
        if (enableTesting && kaabaCreated && kaabaInstance != null)
        {
            TrackMovement();
        }
        
        // Manual Kaaba creation with touch (only for initial setup)
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (!kaabaCreated)
            {
                CreateKaaba();
                // If main creation failed, try fallback
                if (!kaabaCreated)
                {
                    CreateSimpleKaaba();
                }
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
                

            }
            
                    // Check if passed Hajar al-Aswad (0 degrees)
        CheckHajarAswadPass();
        
        // Simple round counting: each Hajar al-Aswad pass = 1 round
        if (hajarAswadPasses > 0 && !hasPassedHajarAswad)
        {
            // Complete round when we pass Hajar al-Aswad
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
        }
        // If we've moved away from Hajar al-Aswad, reset the flag
        else if (!nearHajarAswad && hasPassedHajarAswad)
        {
            hasPassedHajarAswad = false;
        }
        

    }
    
    // Removed complex CheckRoundCompletion method - using simple Hajar al-Aswad pass counting instead
    
    void CompleteRound()
    {
        currentRound++;
        
        // Play round completion prayer
        if (enablePrayers && prayerRecitation != null)
        {
            prayerRecitation.PlayRoundCompletionPrayer(currentRound);
        }
        
        if (currentRound >= 7)
        {
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
        
        // Modern AR App UI Design
        int y = 40;
        int lineHeight = 30;
        int padding = 20;
        int cornerRadius = 15;
        
        // Main title with modern styling
        GUI.skin.label.fontSize = 28;
        GUI.skin.label.fontStyle = FontStyle.Bold;
        
        // Title background with gradient effect
        GUI.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
        GUI.Box(new Rect(padding - 10, y - 10, 400, 50), "");
        
        // Title text with glow effect
        GUI.color = new Color(0.2f, 0.8f, 1f); // Cyan blue
        GUI.Label(new Rect(padding, y, 400, lineHeight), "🕌 Tawaf AR Trainer");
        y += lineHeight + 20;
        
        if (kaabaCreated)
        {
            // Round counter with modern card design
            GUI.skin.label.fontSize = 36;
            GUI.skin.label.fontStyle = FontStyle.Bold;
            
            // Card background with shadow
            GUI.color = new Color(0.15f, 0.15f, 0.15f, 0.95f);
            GUI.Box(new Rect(padding - 15, y - 15, 350, 80), "");
            
            // Round counter text
            GUI.color = Color.white;
            GUI.Label(new Rect(padding, y, 350, 40), $"Round {currentRound}/7");
            y += 50;
            
            // Modern progress bar
            GUI.skin.label.fontSize = 16;
            
            // Progress bar container
            GUI.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            GUI.Box(new Rect(padding, y, 320, 20), "");
            
            // Progress bar fill with gradient
            float progress = (float)currentRound / 7f;
            Color progressColor = Color.Lerp(new Color(0.2f, 0.8f, 0.2f), new Color(0.1f, 0.9f, 0.3f), progress);
            GUI.color = progressColor;
            GUI.Box(new Rect(padding, y, 320 * progress, 20), "");
            
            // Progress percentage
            GUI.color = Color.white;
            GUI.Label(new Rect(padding + 330, y, 50, 20), $"{(progress * 100):F0}%");
            y += 40;
            
            // Status cards
            GUI.skin.label.fontSize = 18;
            
            // Distance card
            GUI.color = new Color(0.12f, 0.12f, 0.12f, 0.9f);
            GUI.Box(new Rect(padding - 10, y - 10, 380, 50), "");
            
            GUI.color = isValidDistance ? new Color(0.2f, 0.9f, 0.2f) : new Color(0.9f, 0.3f, 0.3f);
            GUI.Label(new Rect(padding, y, 380, 30), $"📏 Distance: {distanceToKaaba:F1}m");
            
            GUI.color = new Color(0.7f, 0.7f, 0.7f);
            GUI.skin.label.fontSize = 14;
            GUI.Label(new Rect(padding, y + 25, 380, 20), isValidDistance ? "✅ Optimal range" : "⚠️ Move closer or further");
            y += 60;
            
            // Current corner indicator
            if (enableCornerMarkers && cornerMarker != null && cornerMarker.IsNearCorner())
            {
                GUI.color = new Color(0.12f, 0.12f, 0.12f, 0.9f);
                GUI.Box(new Rect(padding - 10, y - 10, 380, 50), "");
                
                GUI.color = new Color(1f, 0.8f, 0.2f); // Gold
                GUI.skin.label.fontSize = 18;
                GUI.Label(new Rect(padding, y, 380, 30), $"📍 {cornerMarker.GetCurrentCornerName()}");
                
                GUI.color = new Color(0.7f, 0.7f, 0.7f);
                GUI.skin.label.fontSize = 14;
                GUI.Label(new Rect(padding, y + 25, 380, 20), "Approaching corner");
                y += 60;
            }
            
            // Hajar al-Aswad indicator
            Vector3 toKaaba = kaabaInstance.transform.position - Camera.main.transform.position;
            Vector2 flatDirection = new Vector2(toKaaba.x, toKaaba.z);
            float currentAngle = Mathf.Atan2(flatDirection.y, flatDirection.x) * Mathf.Rad2Deg;
            if (currentAngle < 0) currentAngle += 360f;
            bool nearHajarAswad = Mathf.Abs(Mathf.DeltaAngle(currentAngle, 0f)) < hajarAswadThreshold;
            
            if (nearHajarAswad)
            {
                GUI.color = new Color(0.15f, 0.05f, 0.05f, 0.95f);
                GUI.Box(new Rect(padding - 10, y - 10, 380, 50), "");
                
                GUI.color = new Color(0.9f, 0.2f, 0.2f); // Red for Hajar al-Aswad
                GUI.skin.label.fontSize = 18;
                GUI.Label(new Rect(padding, y, 380, 30), "🖤 Hajar al-Aswad");
                
                GUI.color = new Color(0.8f, 0.8f, 0.8f);
                GUI.skin.label.fontSize = 14;
                GUI.Label(new Rect(padding, y + 25, 380, 20), "Black Stone - Complete your round");
                y += 60;
            }
        }
        else
        {
            // Setup instructions with modern design
            GUI.color = new Color(0.12f, 0.12f, 0.12f, 0.9f);
            GUI.Box(new Rect(padding - 10, y - 10, 380, 80), "");
            
            GUI.color = Color.white;
            GUI.skin.label.fontSize = 20;
            GUI.Label(new Rect(padding, y, 380, 30), "🎯 Setup Required");
            
            GUI.color = new Color(0.8f, 0.8f, 0.8f);
            GUI.skin.label.fontSize = 16;
            GUI.Label(new Rect(padding, y + 35, 380, 25), "Tap screen to create Kaaba");
            GUI.Label(new Rect(padding, y + 55, 380, 25), "Point camera at floor, then tap");
        }
        
        // Bottom instructions with modern styling
        y = Screen.height - 120;
        GUI.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        GUI.Box(new Rect(0, y - 20, Screen.width, 100), "");
        
        GUI.color = new Color(0.8f, 0.8f, 0.8f);
        GUI.skin.label.fontSize = 16;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUI.Label(new Rect(0, y, Screen.width, 25), "🔄 Walk around the Kaaba in circles");
        GUI.Label(new Rect(0, y + 25, Screen.width, 25), "📏 Keep 0.5-3m distance for best tracking");
        GUI.Label(new Rect(0, y + 50, Screen.width, 25), "🎯 Complete 7 rounds to finish Tawaf");
        
        // Tawaf completion celebration with modern overlay
        if (currentRound >= 7)
        {
            // Semi-transparent overlay with blur effect
            GUI.color = new Color(0, 0, 0, 0.85f);
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
            
            // Celebration card
            GUI.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
            GUI.Box(new Rect(Screen.width/2 - 200, Screen.height/2 - 150, 400, 300), "");
            
            // Celebration text with modern typography
            GUI.color = new Color(1f, 0.8f, 0.2f); // Gold
            GUI.skin.label.fontSize = 36;
            GUI.skin.label.fontStyle = FontStyle.Bold;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(Screen.width/2 - 200, Screen.height/2 - 120, 400, 50), "🎉 TAWAF COMPLETED! 🎉");
            
            GUI.color = Color.white;
            GUI.skin.label.fontSize = 20;
            GUI.Label(new Rect(Screen.width/2 - 200, Screen.height/2 - 70, 400, 30), "All 7 rounds finished successfully");
            
            GUI.color = new Color(0.8f, 0.8f, 0.8f);
            GUI.skin.label.fontSize = 16;
            GUI.Label(new Rect(Screen.width/2 - 200, Screen.height/2 - 30, 400, 25), "May Allah accept your Tawaf");
            
            // Completion stats
            GUI.color = new Color(0.2f, 0.8f, 0.2f);
            GUI.Label(new Rect(Screen.width/2 - 200, Screen.height/2 + 10, 400, 25), "✅ Perfect completion achieved");
            
            GUI.color = new Color(0.7f, 0.7f, 0.7f);
            GUI.Label(new Rect(Screen.width/2 - 200, Screen.height/2 + 40, 400, 25), "Tap anywhere to continue");
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
        }
        catch (System.Exception e)
        {
            kaabaCreated = false;
        }
    }
} 