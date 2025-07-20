using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic; // Added for List

public class TawafController : MonoBehaviour
{
    [Header("Testing Mode")]
    public bool enableTesting = true;
    public bool showDebugUI = true;
    public bool debugMode = false; // Clean experience
    public bool simpleTestMode = true; // Simple test mode for debugging
    
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
    
    [Header("Realistic Kaaba")]
    public bool useRealisticKaaba = false; // Set to false to use simple Kaaba
    public RealisticKaabaBuilder kaabaBuilder;
    public float kaabaScale = 0.1f; // Scale for AR viewing
    
    [Header("Movement Settings")]
    public float minimumDistance = 0.5f; // Smaller for limited space
    public float maximumDistance = 3f;   // Smaller for limited space
    public bool isValidDistance = false;
    
    [Header("Round Counting")]
    public float hajarAswadThreshold = 5f; // Threshold for detecting Hajar al-Aswad pass
    
    [Header("Istilam (Waving) Gesture")]
    public bool enableIstilam = true;
    public float istilamGestureThreshold = 0.2f; // Reduced threshold for easier detection
    public float istilamTimeWindow = 5f; // Extended time window for more comfortable gesture
    public float istilamShakeThreshold = 0.1f; // Threshold for shake detection
    public int requiredShakes = 3; // Number of shakes required to complete Istilam
    
    // Floor arrow system removed
    
    private Vector3 lastPosition;
    private float lastAngle = 0f;
    private bool hasPassedHajarAswad = false;
    private int hajarAswadPasses = 0;
    
    // Round tracking variables
    private bool roundInProgress = false;
    private float roundStartAngle = 0f;
    private float totalRotationThisRound = 0f;
    private float roundStartTime = 0f;
    private float roundTimeout = 300f; // 5 minutes timeout for a round
    
    // Istilam gesture tracking
    private bool isNearHajarAswad = false;
    private bool istilamPerformed = false;
    private float istilamTimer = 0f;
    private Vector3 gestureStartPosition;
    private bool gestureStarted = false;
    private float gestureStartTime = 0f;
    
    // Shake gesture tracking
    private int shakeCount = 0;
    private Vector3 lastShakePosition;
    private bool shakeDirectionChanged = false;
    private float lastShakeTime = 0f;
    
    // Long press tracking removed
    
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
            if (useRealisticKaaba)
            {
                CreateRealisticKaaba();
            }
            else
            {
                CreateSimpleKaaba();
            }
            
            // Play Tawaf start prayer when Kaaba is successfully created
            if (kaabaCreated && enablePrayers && prayerRecitation != null)
            {
                prayerRecitation.PlayTawafStartPrayer();
            }
        }
        catch (System.Exception e)
        {
            kaabaCreated = false;
        }
    }
    
    void CreateRealisticKaaba()
    {
        // Create Kaaba container
        GameObject kaabaContainer = new GameObject("Kaaba");
        kaabaContainer.transform.position = Camera.main.transform.position + (Camera.main.transform.forward * 3f);
        kaabaContainer.transform.position = new Vector3(kaabaContainer.transform.position.x, 0.5f, kaabaContainer.transform.position.z);
        
        // Add RealisticKaabaBuilder component
        kaabaBuilder = kaabaContainer.AddComponent<RealisticKaabaBuilder>();
        
        // Configure realistic Kaaba settings
        kaabaBuilder.kaabaHeight = 1.5f; // Scaled down for AR
        kaabaBuilder.kaabaWidth = 1.2f;
        kaabaBuilder.kaabaDepth = 1.2f;
        
        // Create the realistic Kaaba
        kaabaBuilder.CreateRealisticKaaba();
        
        // Scale for AR viewing
        kaabaBuilder.ScaleForAR(kaabaScale);
        
        // Add lighting for better visibility
        kaabaBuilder.AddLighting();
        
        // Set the main Kaaba instance
        kaabaInstance = kaabaContainer;
        kaabaCreated = true;
        lastPosition = Camera.main.transform.position;
        
        Debug.Log("Realistic Kaaba created successfully!");
    }
    
    void CreateSimpleKaaba()
    {
        // Create a very small, simple black cube for Kaaba
        kaabaInstance = GameObject.CreatePrimitive(PrimitiveType.Cube);
        kaabaInstance.name = "Kaaba";
        kaabaInstance.transform.localScale = new Vector3(0.5f, 0.75f, 0.5f); // Actual working size from KaabaTest.cs
        
        // Position the Kaaba properly in front of the user
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 cameraForward = Camera.main.transform.forward;
        
        // Place Kaaba 2 meters in front of the user (original working distance)
        Vector3 kaabaPosition = cameraPosition + (cameraForward * 2f);
        
        // Keep it at the same height as the user's camera (eye level)
        kaabaPosition.y = cameraPosition.y;
        
        kaabaInstance.transform.position = kaabaPosition;
        
        // Simple dark material for the Kaaba
        Renderer renderer = kaabaInstance.GetComponent<Renderer>();
        if (renderer != null)
        {
            // Use the default material and just change the color
            Material kaabaMaterial = renderer.material;
            kaabaMaterial.color = new Color(0.2f, 0.2f, 0.2f, 1f); // Dark gray, visible
        }
        
        kaabaCreated = true;
        lastPosition = Camera.main.transform.position;
    }
    
    void Update()
    {
        if (enableTesting && kaabaCreated && kaabaInstance != null)
        {
            TrackMovement();
        }
        
        // Simple test mode - simulate rotation with keyboard
        if (simpleTestMode && roundInProgress)
        {
            // Use arrow keys to simulate rotation
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                totalRotationThisRound += 45f; // Simulate 45° rotation
                Debug.Log($"🔧 Simple test - Rotation: {totalRotationThisRound:F1}°/360°");
                
                if (totalRotationThisRound >= 355f)
                {
                    Debug.Log($"🎯 Simple test - Round completed!");
                    CompleteRound();
                    roundInProgress = false;
                    totalRotationThisRound = 0f;
                }
            }
        }
        
        // Ultra simple test - just increment round with spacebar
        if (simpleTestMode && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"🔧 ULTRA SIMPLE TEST - Incrementing round from {currentRound} to {currentRound + 1}");
            currentRound++;
            Debug.Log($"🔧 ULTRA SIMPLE TEST - Round is now: {currentRound}");
        }
        
        // Arrow test removed
        
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
            
            // Long press tracking removed
        }
        
        // Long press detection removed
        
        // Debug: Manual round increment for testing (double tap)
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (Input.GetTouch(0).tapCount == 2) // Double tap
            {
                Debug.Log($"🔧 Manual round increment test - Current: {currentRound}");
                CompleteRound();
            }
            
            // Triple tap to reset round tracking
            if (Input.GetTouch(0).tapCount == 3) // Triple tap
            {
                Debug.Log($"🔧 Manual round reset - Current: {currentRound}, roundInProgress: {roundInProgress}");
                roundInProgress = false;
                totalRotationThisRound = 0f;
                hasPassedHajarAswad = false;
                roundStartTime = 0f;
                Debug.Log($"🔧 Round tracking reset - ready to start new round");
            }
            
            // Single tap to start round (for simple test mode)
            if (simpleTestMode && Input.GetTouch(0).tapCount == 1)
            {
                Debug.Log($"🔧 Simple test mode - Starting round manually");
                roundInProgress = true;
                roundStartAngle = 0f;
                totalRotationThisRound = 0f;
                roundStartTime = Time.time;
                hasPassedHajarAswad = true;
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
        
        // Check if user has moved significantly (prevent false counting when stationary)
        float positionChange = Vector3.Distance(currentPosition, lastPosition);
        bool hasMoved = positionChange > 0.01f; // Minimum 1cm movement
        
        // Calculate angle around Kaaba (X-Z plane only)
        Vector3 toKaaba = kaabaInstance.transform.position - currentPosition;
        Vector2 flatDirection = new Vector2(toKaaba.x, toKaaba.z);
        float currentAngle = Mathf.Atan2(flatDirection.y, flatDirection.x) * Mathf.Rad2Deg;
        
        // Normalize angle to 0-360
        if (currentAngle < 0) currentAngle += 360f;
        
        // Debug logging every 60 frames
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"TrackMovement - Angle: {currentAngle:F1}°, Distance: {distanceToKaaba:F1}m, Valid: {isValidDistance}, Round: {currentRound}");
        }
        
        if (isValidDistance)
        {
            // Calculate angle change (handle wrapping around 360 degrees)
            float angleDifference = Mathf.DeltaAngle(lastAngle, currentAngle);
            
            // Only count significant movements (filter out noise)
            if (Mathf.Abs(angleDifference) > 1f) // Minimum 1 degree change
            {
                totalAngleChange += angleDifference;
            }
            
            lastAngle = currentAngle;
        }
        else
        {
            // Reset angle tracking if out of range
            lastAngle = 0f;
        }
        
        // Check if passed Hajar al-Aswad (90 degrees)
        CheckHajarAswadPass();
        
        // Track rotation progress for display purposes only
        if (roundInProgress && isValidDistance && hasMoved)
        {
            // Check for round timeout
            if (Time.time - roundStartTime > roundTimeout)
            {
                Debug.Log($"⏰ Round timeout after {roundTimeout}s - resetting round tracking");
                roundInProgress = false;
                totalRotationThisRound = 0f;
                return;
            }
            
            // Calculate rotation since round start (handle wrapping around 360°)
            float angleDifference = currentAngle - roundStartAngle;
            
            // Handle wrapping around 360 degrees
            if (angleDifference < 0)
            {
                angleDifference += 360f;
            }
            
            // Only update rotation if there's significant movement (prevent false counting)
            float movementThreshold = 2f; // Minimum 2 degrees of movement
            if (Mathf.Abs(angleDifference - totalRotationThisRound) >= movementThreshold)
            {
                totalRotationThisRound = angleDifference;
                
                // Debug logging every 60 frames
                if (Time.frameCount % 60 == 0)
                {
                    Debug.Log($"Round Progress - Start: {roundStartAngle:F1}°, Current: {currentAngle:F1}°, Rotation: {totalRotationThisRound:F1}°/360°");
                }
            }
        }
        
        // Arrow visibility update removed
        else if (roundInProgress && !isValidDistance)
        {
            // Pause round tracking if out of distance
            if (Time.frameCount % 120 == 0) // Log every 2 seconds
            {
                Debug.Log($"⚠️ Round tracking paused - Distance: {distanceToKaaba:F1}m (need 0.5-3m)");
            }
        }
        else if (roundInProgress && !hasMoved)
        {
            // Pause round tracking if no movement
            if (Time.frameCount % 180 == 0) // Log every 3 seconds
            {
                Debug.Log($"⏸️ Round tracking paused - No movement detected");
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
        
        // Hajar al-Aswad is at 90 degrees (on the right when facing Kaaba)
        float hajarAswadAngle = 90f;
        
        // Check if we're near Hajar al-Aswad
        bool nearHajarAswad = Mathf.Abs(Mathf.DeltaAngle(currentAngle, hajarAswadAngle)) < hajarAswadThreshold;
        
        // Update near Hajar al-Aswad status
        isNearHajarAswad = nearHajarAswad;
        
        // Debug logging for Hajar al-Aswad detection
        if (Time.frameCount % 30 == 0)
        {
            Debug.Log($"HajarAswad - Angle: {currentAngle:F1}°, Near: {nearHajarAswad}, Passed: {hasPassedHajarAswad}, Passes: {hajarAswadPasses}");
        }
        
        if (nearHajarAswad)
        {
            // If we haven't passed Hajar al-Aswad yet, mark it as passed
            if (!hasPassedHajarAswad)
            {
                hasPassedHajarAswad = true;
                hajarAswadPasses++;
                Debug.Log($"Passed Hajar al-Aswad! Current angle: {currentAngle:F1}°, Passes: {hajarAswadPasses}");
                
                // Complete previous round if one was in progress
                if (roundInProgress)
                {
                    Debug.Log($"🎯 Completing round {currentRound} - returned to Hajar al-Aswad");
                    CompleteRound();
                }
                
                // Start a new round (except for the very first pass which just starts tracking)
                if (hajarAswadPasses > 1) // Start new round after first pass
                {
                    roundInProgress = true;
                    roundStartAngle = currentAngle;
                    totalRotationThisRound = 0f;
                    roundStartTime = Time.time;
                    Debug.Log($"🔄 Starting new round {currentRound + 1} from Hajar al-Aswad at angle: {currentAngle:F1}°");
                }
                else
                {
                    // First pass - just start tracking
                    roundInProgress = true;
                    roundStartAngle = currentAngle;
                    totalRotationThisRound = 0f;
                    roundStartTime = Time.time;
                    Debug.Log($"🔄 First pass - starting round tracking at angle: {currentAngle:F1}°");
                }
                
                // Start Istilam timer if enabled
                if (enableIstilam)
                {
                    istilamTimer = istilamTimeWindow;
                    istilamPerformed = false;
                    gestureStarted = false;
                    Debug.Log($"🎯 ISTILAM OPPORTUNITY! Timer started: {istilamTimer}s");
                }
            }
            
            // Check for Istilam gesture if enabled
            if (enableIstilam && !istilamPerformed && istilamTimer > 0)
            {
                CheckIstilamGesture();
            }
            
            // Count down Istilam timer
            if (istilamTimer > 0)
            {
                istilamTimer -= Time.deltaTime;
            }
        }
        // If we've moved away from Hajar al-Aswad, reset the flags
        else if (!nearHajarAswad && hasPassedHajarAswad)
        {
            hasPassedHajarAswad = false;
            istilamPerformed = false;
            gestureStarted = false;
        }
    }
    
    void CheckIstilamGesture()
    {
        // Get current device position (using camera position as proxy for device position)
        Vector3 currentPosition = Camera.main.transform.position;
        
        // Initialize shake tracking if not started
        if (!gestureStarted)
        {
            gestureStartPosition = currentPosition;
            lastShakePosition = currentPosition;
            gestureStarted = true;
            gestureStartTime = Time.time;
            shakeCount = 0;
            shakeDirectionChanged = false;
            lastShakeTime = Time.time;
            Debug.Log("🎯 Starting Istilam shake detection...");
        }
        
        // Calculate movement from last position
        float movementDistance = Vector3.Distance(lastShakePosition, currentPosition);
        float timeSinceLastShake = Time.time - lastShakeTime;
        
        // Detect shake movement (any direction)
        if (movementDistance > istilamShakeThreshold && timeSinceLastShake > 0.2f)
        {
            // Count this as a shake
            shakeCount++;
            lastShakeTime = Time.time;
            lastShakePosition = currentPosition;
            
            Debug.Log($"📱 Shake {shakeCount}/{requiredShakes} detected! Movement: {movementDistance:F2}m");
            
            // Check if we have enough shakes
            if (shakeCount >= requiredShakes)
            {
                istilamPerformed = true;
                Debug.Log($"✅ ISTILAM COMPLETED! {shakeCount} shakes detected");
                PlayIstilamFeedback();
            }
        }
        
        // Reset if taking too long
        float gestureDuration = Time.time - gestureStartTime;
        if (gestureDuration > istilamTimeWindow)
        {
            Debug.Log($"⏰ Istilam time expired. Shakes completed: {shakeCount}/{requiredShakes}");
            gestureStarted = false;
            shakeCount = 0;
        }
    }
    
    void PlayIstilamFeedback()
    {
        // Play Istilam completion sound or haptic feedback
        // This could be integrated with the prayer system
        if (enablePrayers && prayerRecitation != null)
        {
            // Play the specific Istilam prayer
            prayerRecitation.PlayIstilamPrayer();
        }
    }
    
    // Removed complex CheckRoundCompletion method - using simple Hajar al-Aswad pass counting instead
    
    // All arrow-related methods removed
    
    void CompleteRound()
    {
        Debug.Log($"🎯 CompleteRound() called! Previous round: {currentRound}");
        currentRound++;
        Debug.Log($"🎉 ROUND COMPLETED! Total rotation: {totalRotationThisRound:F1}° (360° required)");
        Debug.Log($"🎉 Current Round: {currentRound}/7");
        
        // Reset round tracking for next round
        roundInProgress = false;
        totalRotationThisRound = 0f;
        
        // Play round completion prayer
        if (enablePrayers && prayerRecitation != null)
        {
            prayerRecitation.PlayRoundCompletionPrayer(currentRound);
        }
        
        if (currentRound >= 7)
        {
            Debug.Log("🎊 TAWAF COMPLETED! All 7 rounds finished!");
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
        
        // Enhanced Modern AR App UI Design
        int y = 40;
        int lineHeight = 30;
        int padding = 20;
        
        // Add subtle background blur effect
        GUI.color = new Color(0, 0, 0, 0.3f);
        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
        
        // Main title with enhanced styling
        GUI.skin.label.fontSize = 32;
        GUI.skin.label.fontStyle = FontStyle.Bold;
        
        // Title background with enhanced gradient and shadow
        GUI.color = new Color(0.05f, 0.05f, 0.05f, 0.95f);
        GUI.Box(new Rect(padding - 15, y - 15, 420, 60), "");
        
        // Title glow effect
        GUI.color = new Color(0.3f, 0.9f, 1f, 0.8f); // Brighter cyan
        GUI.Label(new Rect(padding + 2, y + 2, 400, lineHeight), "🕌 Tawaf AR Trainer");
        
        // Main title text
        GUI.color = new Color(0.2f, 0.8f, 1f); // Cyan blue
        GUI.Label(new Rect(padding, y, 400, lineHeight), "🕌 Tawaf AR Trainer");
        y += lineHeight + 25;
        
        if (kaabaCreated)
        {
            // Enhanced round counter with modern card design
            GUI.skin.label.fontSize = 42;
            GUI.skin.label.fontStyle = FontStyle.Bold;
            
            // Card background with enhanced shadow and border
            GUI.color = new Color(0.08f, 0.08f, 0.08f, 0.98f);
            GUI.Box(new Rect(padding - 20, y - 20, 380, 100), "");
            
            // Inner card with gradient
            GUI.color = new Color(0.12f, 0.12f, 0.12f, 0.95f);
            GUI.Box(new Rect(padding - 15, y - 15, 370, 90), "");
            
            // Round counter text with glow
            GUI.color = new Color(1f, 1f, 1f, 0.8f);
            GUI.Label(new Rect(padding + 2, y + 2, 350, 50), $"Round {currentRound}/7");
            
            GUI.color = Color.white;
            GUI.Label(new Rect(padding, y, 350, 50), $"Round {currentRound}/7");
            
            // Simple test display
            if (simpleTestMode)
            {
                GUI.color = Color.yellow;
                GUI.skin.label.fontSize = 16;
                GUI.Label(new Rect(padding, y + 60, 350, 30), $"TEST: Press SPACE to increment, RIGHT ARROW for rotation");
                GUI.Label(new Rect(padding, y + 90, 350, 30), $"Hajar Aswad Passes: {hajarAswadPasses}, Round Active: {roundInProgress}");
                y += 120;
            }
            else
            {
                y += 60;
            }
            
            // Enhanced progress bar with animation
            GUI.skin.label.fontSize = 16;
            
            // Progress bar container with border
            GUI.color = new Color(0.15f, 0.15f, 0.15f, 0.9f);
            GUI.Box(new Rect(padding - 2, y - 2, 340, 24), "");
            
            // Progress bar background
            GUI.color = new Color(0.25f, 0.25f, 0.25f, 0.8f);
            GUI.Box(new Rect(padding, y, 336, 20), "");
            
            // Round progress display
            if (roundInProgress)
            {
                float progressPercentage = totalRotationThisRound / 360f;
                GUI.color = new Color(0.2f, 0.8f, 0.2f, 0.9f);
                GUI.Box(new Rect(padding, y, 336 * progressPercentage, 20), "");
                
                // Progress text
                GUI.color = Color.white;
                GUI.skin.label.fontSize = 14;
                GUI.Label(new Rect(padding, y, 336, 20), $"Rotation: {totalRotationThisRound:F1}°/360° ({progressPercentage * 100:F0}%) | Hajar Aswad Passes: {hajarAswadPasses}");
            }
            
            // Progress bar fill with enhanced gradient and animation
            float progress = (float)currentRound / 7f;
            Color progressColor = Color.Lerp(new Color(0.2f, 0.8f, 0.2f), new Color(0.1f, 0.9f, 0.3f), progress);
            
            // Add pulsing effect for active progress
            float progressPulse = Mathf.Sin(Time.time * 3f) * 0.1f + 0.9f;
            progressColor = new Color(progressColor.r * progressPulse, progressColor.g * progressPulse, progressColor.b * progressPulse, progressColor.a);
            
            GUI.color = progressColor;
            GUI.Box(new Rect(padding, y, 336 * progress, 20), "");
            
            // Progress percentage with enhanced styling
            GUI.color = Color.white;
            GUI.skin.label.fontSize = 14;
            GUI.skin.label.fontStyle = FontStyle.Bold;
            GUI.Label(new Rect(padding + 340, y, 60, 20), $"{(progress * 100):F0}%");
            y += 40;
            
            // Enhanced status cards with animations
            GUI.skin.label.fontSize = 18;
            
            // Distance card with enhanced design
            GUI.color = new Color(0.08f, 0.08f, 0.08f, 0.95f);
            GUI.Box(new Rect(padding - 12, y - 12, 384, 54), "");
            
            GUI.color = new Color(0.12f, 0.12f, 0.12f, 0.9f);
            GUI.Box(new Rect(padding - 10, y - 10, 380, 50), "");
            
            // Distance icon with color coding
            string distanceIcon = isValidDistance ? "✅" : "⚠️";
            Color distanceColor = isValidDistance ? new Color(0.2f, 0.9f, 0.2f) : new Color(0.9f, 0.3f, 0.3f);
            
            // Add pulsing effect for distance warning
            if (!isValidDistance)
            {
                float distancePulse = Mathf.Sin(Time.time * 4f) * 0.2f + 0.8f;
                distanceColor = new Color(distanceColor.r * distancePulse, distanceColor.g * distancePulse, distanceColor.b * distancePulse, distanceColor.a);
            }
            
            GUI.color = distanceColor;
            GUI.Label(new Rect(padding, y, 380, 30), $"{distanceIcon} Distance: {distanceToKaaba:F1}m");
            
            GUI.color = new Color(0.7f, 0.7f, 0.7f);
            GUI.skin.label.fontSize = 14;
            GUI.Label(new Rect(padding, y + 25, 380, 20), isValidDistance ? "🎯 Optimal range" : "📏 Move closer or further");
            y += 60;
            
            // Enhanced corner indicator with animations
            if (enableCornerMarkers && cornerMarker != null && cornerMarker.IsNearCorner())
            {
                // Corner card with enhanced design
                GUI.color = new Color(0.08f, 0.08f, 0.08f, 0.95f);
                GUI.Box(new Rect(padding - 12, y - 12, 384, 54), "");
                
                GUI.color = new Color(0.12f, 0.12f, 0.12f, 0.9f);
                GUI.Box(new Rect(padding - 10, y - 10, 380, 50), "");
                
                // Corner name with pulsing gold effect
                float goldPulse = Mathf.Sin(Time.time * 2f) * 0.1f + 0.9f;
                Color goldColor = new Color(1f * goldPulse, 0.8f * goldPulse, 0.2f * goldPulse);
                
                GUI.color = goldColor;
                GUI.skin.label.fontSize = 18;
                GUI.Label(new Rect(padding, y, 380, 30), $"📍 {cornerMarker.GetCurrentCornerName()}");
                
                GUI.color = new Color(0.7f, 0.7f, 0.7f);
                GUI.skin.label.fontSize = 14;
                GUI.Label(new Rect(padding, y + 25, 380, 20), "🎯 Approaching corner");
                y += 60;
            }
            
            // Hajar al-Aswad indicator
            Vector3 toKaaba = kaabaInstance.transform.position - Camera.main.transform.position;
            Vector2 flatDirection = new Vector2(toKaaba.x, toKaaba.z);
            float currentAngle = Mathf.Atan2(flatDirection.y, flatDirection.x) * Mathf.Rad2Deg;
            if (currentAngle < 0) currentAngle += 360f;
            bool nearHajarAswad = Mathf.Abs(Mathf.DeltaAngle(currentAngle, 90f)) < hajarAswadThreshold;
            
            if (nearHajarAswad)
            {
                // Enhanced Hajar al-Aswad card with dramatic styling
                GUI.color = new Color(0.1f, 0.02f, 0.02f, 0.98f);
                GUI.Box(new Rect(padding - 15, y - 15, 390, 100), "");
                
                GUI.color = new Color(0.15f, 0.05f, 0.05f, 0.95f);
                GUI.Box(new Rect(padding - 10, y - 10, 380, 90), "");
                
                // Hajar al-Aswad title with pulsing red effect
                float redPulse = Mathf.Sin(Time.time * 3f) * 0.15f + 0.85f;
                Color redColor = new Color(0.9f * redPulse, 0.2f * redPulse, 0.2f * redPulse);
                
                GUI.color = redColor;
                GUI.skin.label.fontSize = 20;
                GUI.Label(new Rect(padding, y, 380, 30), "🖤 Hajar al-Aswad");
                
                if (enableIstilam && !istilamPerformed && istilamTimer > 0)
                {
                    // Istilam instruction with urgent pulsing
                    float urgentPulse = Mathf.Sin(Time.time * 6f) * 0.2f + 0.8f;
                    Color goldColor = new Color(1f * urgentPulse, 0.8f * urgentPulse, 0.2f * urgentPulse);
                    
                    GUI.color = goldColor;
                    GUI.skin.label.fontSize = 18;
                    GUI.Label(new Rect(padding, y + 25, 380, 25), "🤲 SHAKE YOUR PHONE!");
                    
                    // Shake progress
                    GUI.color = new Color(0.2f, 0.8f, 0.2f);
                    GUI.skin.label.fontSize = 16;
                    GUI.Label(new Rect(padding, y + 45, 380, 20), $"📱 Shakes: {shakeCount}/{requiredShakes}");
                    
                    GUI.color = new Color(0.7f, 0.7f, 0.7f);
                    GUI.skin.label.fontSize = 14;
                    GUI.Label(new Rect(padding, y + 65, 380, 20), $"⏰ Time remaining: {istilamTimer:F1}s");
                    
                    // Add more visible instruction
                    GUI.color = new Color(1f, 0.5f, 0f); // Orange
                    GUI.skin.label.fontSize = 16;
                    GUI.Label(new Rect(padding, y + 85, 380, 20), "🔄 Shake in any direction to remove card!");
                }
                else if (istilamPerformed)
                {
                    // Completion with celebration effect
                    float celebrationPulse = Mathf.Sin(Time.time * 4f) * 0.1f + 0.9f;
                    Color greenColor = new Color(0.2f * celebrationPulse, 0.9f * celebrationPulse, 0.2f * celebrationPulse);
                    
                    GUI.color = greenColor;
                    GUI.skin.label.fontSize = 16;
                    GUI.Label(new Rect(padding, y + 25, 380, 20), "✅ Istilam completed!");
                    
                    GUI.color = new Color(0.8f, 0.8f, 0.8f);
                    GUI.skin.label.fontSize = 14;
                    GUI.Label(new Rect(padding, y + 45, 380, 20), "🚶 Continue your Tawaf");
                }
                else
                {
                    GUI.color = new Color(0.8f, 0.8f, 0.8f);
                    GUI.skin.label.fontSize = 16;
                    GUI.Label(new Rect(padding, y + 25, 380, 20), "🖤 Black Stone - Complete your round");
                }
                y += 100;
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
        
        // Enhanced bottom instructions with modern styling
        y = Screen.height - 160;
        
        // Dedicated Istilam status display (always visible when active)
        if (enableIstilam && isNearHajarAswad && !istilamPerformed && istilamTimer > 0)
        {
            // Large Istilam instruction overlay
            GUI.color = new Color(0, 0, 0, 0.7f);
            GUI.Box(new Rect(0, Screen.height/2 - 100, Screen.width, 200), "");
            
            // Istilam card
            GUI.color = new Color(0.1f, 0.05f, 0.02f, 0.95f);
            GUI.Box(new Rect(Screen.width/2 - 200, Screen.height/2 - 80, 400, 160), "");
            
            // Border
            GUI.color = new Color(1f, 0.8f, 0.2f, 0.8f);
            GUI.Box(new Rect(Screen.width/2 - 195, Screen.height/2 - 75, 390, 150), "");
            
            // Main instruction with pulsing
            float pulse = Mathf.Sin(Time.time * 8f) * 0.2f + 0.8f;
            GUI.color = new Color(1f * pulse, 0.8f * pulse, 0.2f * pulse);
            GUI.skin.label.fontSize = 28;
            GUI.skin.label.fontStyle = FontStyle.Bold;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(Screen.width/2 - 200, Screen.height/2 - 60, 400, 40), "🤲 ISTILAM TIME!");
            
            // Gesture instruction
            GUI.color = Color.white;
            GUI.skin.label.fontSize = 20;
            GUI.Label(new Rect(Screen.width/2 - 200, Screen.height/2 - 20, 400, 30), "SHAKE your phone to perform Istilam");
            
            // Shake progress
            GUI.color = new Color(0.2f, 0.8f, 0.2f);
            GUI.skin.label.fontSize = 18;
            GUI.Label(new Rect(Screen.width/2 - 200, Screen.height/2 + 10, 400, 25), $"📱 Shakes: {shakeCount}/{requiredShakes}");
            
            // Timer
            GUI.color = new Color(1f, 0.5f, 0f);
            GUI.skin.label.fontSize = 16;
            GUI.Label(new Rect(Screen.width/2 - 200, Screen.height/2 + 35, 400, 25), $"⏰ {istilamTimer:F1} seconds remaining");
            
            // Quick instruction
            GUI.color = new Color(0.8f, 0.8f, 0.8f);
            GUI.skin.label.fontSize = 14;
            GUI.Label(new Rect(Screen.width/2 - 200, Screen.height/2 + 60, 400, 25), "🔄 Shake in any direction to remove card");
            
            // Reset font settings
            GUI.skin.label.fontSize = 12;
            GUI.skin.label.fontStyle = FontStyle.Normal;
            GUI.skin.label.alignment = TextAnchor.UpperLeft;
        }
        
        // Background with enhanced gradient
        GUI.color = new Color(0.05f, 0.05f, 0.05f, 0.9f);
        GUI.Box(new Rect(0, y - 25, Screen.width, 140), "");
        
        // Inner background
        GUI.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        GUI.Box(new Rect(0, y - 20, Screen.width, 130), "");
        
        GUI.color = new Color(0.9f, 0.9f, 0.9f);
        GUI.skin.label.fontSize = 18;
        GUI.skin.label.fontStyle = FontStyle.Bold;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUI.Label(new Rect(0, y, Screen.width, 30), "🔄 Walk around the Kaaba in circles");
        
        GUI.color = new Color(0.8f, 0.8f, 0.8f);
        GUI.skin.label.fontSize = 16;
        GUI.Label(new Rect(0, y + 30, Screen.width, 25), "📏 Keep 0.5-3m distance for best tracking");
        GUI.Label(new Rect(0, y + 55, Screen.width, 25), "🤲 Shake phone when passing Hajar al-Aswad");
        GUI.Label(new Rect(0, y + 80, Screen.width, 25), "🎯 Complete 7 rounds to finish Tawaf");
        
        // Floor arrow UI removed
        
        // Tawaf completion celebration with modern overlay
        if (currentRound >= 7)
        {
            // Semi-transparent overlay with blur effect
            GUI.color = new Color(0, 0, 0, 0.85f);
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
            
            // Celebration card - increased height to accommodate all text
            GUI.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
            GUI.Box(new Rect(Screen.width/2 - 200, Screen.height/2 - 180, 400, 360), "");
            
            // Celebration text with modern typography
            GUI.color = new Color(1f, 0.8f, 0.2f); // Gold
            GUI.skin.label.fontSize = 36;
            GUI.skin.label.fontStyle = FontStyle.Bold;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(Screen.width/2 - 200, Screen.height/2 - 150, 400, 50), "🎉 TAWAF COMPLETED! 🎉");
            
            GUI.color = Color.white;
            GUI.skin.label.fontSize = 20;
            GUI.Label(new Rect(Screen.width/2 - 200, Screen.height/2 - 100, 400, 30), "All 7 rounds finished successfully");
            
            GUI.color = new Color(0.8f, 0.8f, 0.8f);
            GUI.skin.label.fontSize = 16;
            GUI.Label(new Rect(Screen.width/2 - 200, Screen.height/2 - 60, 400, 25), "May Allah accept your Tawaf");
            
            // Completion stats
            GUI.color = new Color(0.2f, 0.8f, 0.2f);
            GUI.Label(new Rect(Screen.width/2 - 200, Screen.height/2 - 20, 400, 25), "✅ Perfect completion achieved");
            
            GUI.color = new Color(0.7f, 0.7f, 0.7f);
            GUI.Label(new Rect(Screen.width/2 - 200, Screen.height/2 + 10, 400, 25), "Tap anywhere to continue");
            
            // Additional completion message
            GUI.color = new Color(0.9f, 0.9f, 0.9f);
            GUI.Label(new Rect(Screen.width/2 - 200, Screen.height/2 + 40, 400, 25), "Your Tawaf journey is complete");
            
            // Reset font settings
            GUI.skin.label.fontSize = 12;
            GUI.skin.label.fontStyle = FontStyle.Normal;
            GUI.skin.label.alignment = TextAnchor.UpperLeft;
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
} 