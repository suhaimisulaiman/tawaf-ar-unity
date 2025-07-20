using UnityEngine;

public class SimpleCornerTest : MonoBehaviour
{
    [Header("Test Settings")]
    public bool enableTest = true;
    public bool showDebugUI = true;
    
    [Header("Corner Information")]
    public string[] cornerNames = { "Hajar al-Aswad", "Yemeni Corner", "Shami Corner", "Iraqi Corner" };
    public float[] cornerAngles = { 0f, 90f, 180f, 270f };
    public Color[] cornerColors = { Color.red, Color.green, Color.blue, Color.yellow };
    
    [Header("Test Data")]
    public float testAngle = 0f;
    public int currentCornerIndex = -1;
    public bool isNearCorner = false;
    public string currentCornerName = "";
    
    private float markerAlpha = 0f;
    private float markerStartTime = 0f;
    private bool isMarkerVisible = false;
    
    void Start()
    {
        Debug.Log("SimpleCornerTest: Starting corner marker test...");
    }
    
    void Update()
    {
        if (!enableTest) return;
        
        // Simulate movement around corners
        testAngle += Time.deltaTime * 30f; // 30 degrees per second
        if (testAngle >= 360f) testAngle -= 360f;
        
        CheckCornerProximity();
        UpdateMarkerVisibility();
        
        // Manual test with touch
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Debug.Log($"Touch detected - Current angle: {testAngle:F1}°, Corner: {currentCornerName}");
            ShowCornerMarker(currentCornerIndex);
        }
    }
    
    void CheckCornerProximity()
    {
        // Find nearest corner
        int nearestCorner = GetNearestCorner(testAngle);
        float distanceToCorner = Mathf.Abs(Mathf.DeltaAngle(testAngle, cornerAngles[nearestCorner]));
        
        // Check if we're close enough to show marker (within 15 degrees)
        if (distanceToCorner < 15f)
        {
            if (nearestCorner != currentCornerIndex)
            {
                ShowCornerMarker(nearestCorner);
            }
            currentCornerIndex = nearestCorner;
            isNearCorner = true;
            currentCornerName = cornerNames[nearestCorner];
        }
        else
        {
            if (currentCornerIndex != -1)
            {
                HideCornerMarker();
            }
            currentCornerIndex = -1;
            isNearCorner = false;
            currentCornerName = "";
        }
        
        // Debug logging
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"Corner Test - Angle: {testAngle:F1}°, Nearest: {cornerNames[nearestCorner]}, Distance: {distanceToCorner:F1}°, Showing: {isMarkerVisible}");
        }
    }
    
    int GetNearestCorner(float angle)
    {
        float minDistance = 360f;
        int nearestCorner = 0;
        
        for (int i = 0; i < cornerAngles.Length; i++)
        {
            float distance = Mathf.Abs(Mathf.DeltaAngle(angle, cornerAngles[i]));
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestCorner = i;
            }
        }
        
        return nearestCorner;
    }
    
    void ShowCornerMarker(int cornerIndex)
    {
        if (cornerIndex >= 0 && cornerIndex < cornerNames.Length)
        {
            currentCornerName = cornerNames[cornerIndex];
            currentCornerIndex = cornerIndex;
            isMarkerVisible = true;
            markerStartTime = Time.time;
            markerAlpha = 0f;
            
            Debug.Log($"Showing corner marker: {currentCornerName}");
        }
    }
    
    void HideCornerMarker()
    {
        isMarkerVisible = false;
        markerStartTime = Time.time;
        Debug.Log($"Hiding corner marker: {currentCornerName}");
    }
    
    void UpdateMarkerVisibility()
    {
        if (isMarkerVisible)
        {
            // Fade in
            float elapsed = Time.time - markerStartTime;
            markerAlpha = Mathf.Clamp01(elapsed / 1f); // 1 second fade
        }
        else
        {
            // Fade out
            float elapsed = Time.time - markerStartTime;
            markerAlpha = Mathf.Clamp01(1f - (elapsed / 1f));
        }
    }
    
    void OnGUI()
    {
        if (!showDebugUI) return;
        
        int y = 20;
        int lineHeight = 25;
        
        // Test controls
        GUI.color = Color.white;
        GUI.Label(new Rect(20, y, 400, lineHeight), $"Simple Corner Test - Enabled: {enableTest}");
        y += lineHeight;
        
        // Current angle
        GUI.color = Color.yellow;
        GUI.Label(new Rect(20, y, 400, lineHeight), $"Test Angle: {testAngle:F1}°");
        y += lineHeight;
        
        // Corner status
        GUI.color = isNearCorner ? Color.green : Color.white;
        GUI.Label(new Rect(20, y, 400, lineHeight), $"Near Corner: {isNearCorner}");
        y += lineHeight;
        
        GUI.color = Color.cyan;
        GUI.Label(new Rect(20, y, 400, lineHeight), $"Current Corner: {currentCornerName}");
        y += lineHeight;
        
        // Marker status
        GUI.color = isMarkerVisible ? Color.green : Color.white;
        GUI.Label(new Rect(20, y, 400, lineHeight), $"Marker Visible: {isMarkerVisible} (Alpha: {markerAlpha:F2})");
        y += lineHeight;
        
        // Instructions
        GUI.color = Color.white;
        GUI.Label(new Rect(20, y, 400, lineHeight), "Tap screen to test corner marker");
        y += lineHeight;
        
        // Corner marker display (if visible)
        if (isMarkerVisible && markerAlpha > 0f)
        {
            // Background box
            GUI.color = new Color(0, 0, 0, markerAlpha * 0.8f);
            GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 25, 200, 50), "");
            
            // Corner name
            GUI.color = new Color(1, 1, 1, markerAlpha);
            GUI.skin.label.fontSize = 18;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 25, 200, 50), currentCornerName);
            
            // Corner indicator
            if (currentCornerIndex >= 0 && currentCornerIndex < cornerColors.Length)
            {
                GUI.color = new Color(cornerColors[currentCornerIndex].r, cornerColors[currentCornerIndex].g, cornerColors[currentCornerIndex].b, markerAlpha);
                GUI.Box(new Rect(Screen.width / 2 - 5, Screen.height / 2 - 5, 10, 10), "");
            }
        }
    }
} 