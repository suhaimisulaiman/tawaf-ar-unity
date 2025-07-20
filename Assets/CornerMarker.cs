using UnityEngine;

public class CornerMarker : MonoBehaviour
{
    [Header("Marker Settings")]
    public bool showCornerMarkers = true;
    public float markerDisplayDistance = 2f; // Distance from corner to show marker
    public float markerFadeTime = 1f; // Time for marker to fade in/out
    
    [Header("Corner Information")]
    public string[] cornerNames = { "Hajar al-Aswad", "Yemeni Corner", "Shami Corner", "Iraqi Corner" };
    public float[] cornerAngles = { 0f, 90f, 180f, 270f };
    public Color[] cornerColors = { Color.red, Color.green, Color.blue, Color.yellow };
    
    [Header("UI Elements")]
    public string currentCornerName = "";
    public bool isMarkerVisible = false;
    public float markerAlpha = 0f;
    public float markerStartTime = 0f;
    
    private KaabaTest kaabaTest;
    private int currentCornerIndex = -1;
    private int lastCornerIndex = -1;
    
    void Start()
    {
        FindKaabaTest();
    }
    
    void FindKaabaTest()
    {
        kaabaTest = FindFirstObjectByType<KaabaTest>();
        if (kaabaTest == null)
        {
            Debug.LogWarning("CornerMarker: KaabaTest not found!");
        }
    }
    
    void Update()
    {
        if (!showCornerMarkers || kaabaTest == null || !kaabaTest.IsKaabaCreated())
            return;
        
        CheckCornerProximity();
        UpdateMarkerVisibility();
    }
    
    void CheckCornerProximity()
    {
        // Calculate current angle around Kaaba
        Vector3 toKaaba = kaabaTest.GetKaaba().transform.position - Camera.main.transform.position;
        Vector2 flatDirection = new Vector2(toKaaba.x, toKaaba.z);
        float currentAngle = Mathf.Atan2(flatDirection.y, flatDirection.x) * Mathf.Rad2Deg;
        
        // Normalize angle to 0-360
        if (currentAngle < 0) currentAngle += 360f;
        
        // Find nearest corner
        int nearestCorner = GetNearestCorner(currentAngle);
        float distanceToCorner = Mathf.Abs(Mathf.DeltaAngle(currentAngle, cornerAngles[nearestCorner]));
        
        // Check if we're close enough to show marker
        if (distanceToCorner < markerDisplayDistance)
        {
            if (nearestCorner != currentCornerIndex)
            {
                ShowCornerMarker(nearestCorner);
            }
            currentCornerIndex = nearestCorner;
        }
        else
        {
            if (currentCornerIndex != -1)
            {
                HideCornerMarker();
            }
            currentCornerIndex = -1;
        }
        
        // Debug logging
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"Corner Marker - Angle: {currentAngle:F1}°, Nearest: {cornerNames[nearestCorner]}, Distance: {distanceToCorner:F1}°, Showing: {isMarkerVisible}");
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
        currentCornerName = cornerNames[cornerIndex];
        currentCornerIndex = cornerIndex;
        isMarkerVisible = true;
        markerStartTime = Time.time;
        markerAlpha = 0f;
        
        Debug.Log($"Showing corner marker: {currentCornerName}");
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
            markerAlpha = Mathf.Clamp01(elapsed / markerFadeTime);
        }
        else
        {
            // Fade out
            float elapsed = Time.time - markerStartTime;
            markerAlpha = Mathf.Clamp01(1f - (elapsed / markerFadeTime));
        }
    }
    
    void OnGUI()
    {
        if (!showCornerMarkers || markerAlpha <= 0f) return;
        
        // Calculate screen position for corner marker
        Vector3 cornerWorldPos = GetCornerWorldPosition();
        Vector3 screenPos = Camera.main.WorldToScreenPoint(cornerWorldPos);
        
        // Only show if corner is in front of camera
        if (screenPos.z > 0)
        {
            // Background box
            GUI.color = new Color(0, 0, 0, markerAlpha * 0.8f);
            GUI.Box(new Rect(screenPos.x - 100, Screen.height - screenPos.y - 50, 200, 40), "");
            
            // Corner name
            GUI.color = new Color(1, 1, 1, markerAlpha);
            GUI.skin.label.fontSize = 16;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(screenPos.x - 100, Screen.height - screenPos.y - 50, 200, 40), currentCornerName);
            
            // Corner indicator
            if (currentCornerIndex >= 0 && currentCornerIndex < cornerColors.Length)
            {
                GUI.color = new Color(cornerColors[currentCornerIndex].r, cornerColors[currentCornerIndex].g, cornerColors[currentCornerIndex].b, markerAlpha);
                GUI.Box(new Rect(screenPos.x - 5, Screen.height - screenPos.y - 5, 10, 10), "");
            }
        }
    }
    
    Vector3 GetCornerWorldPosition()
    {
        if (kaabaTest == null || !kaabaTest.IsKaabaCreated()) return Vector3.zero;
        
        // Calculate corner position around Kaaba
        Vector3 kaabaPosition = kaabaTest.GetKaaba().transform.position;
        float cornerAngle = cornerAngles[currentCornerIndex] * Mathf.Deg2Rad;
        float distance = 1.5f; // Distance from Kaaba center
        
        Vector3 cornerPos = kaabaPosition + new Vector3(
            Mathf.Cos(cornerAngle) * distance,
            2f, // Height above ground
            Mathf.Sin(cornerAngle) * distance
        );
        
        return cornerPos;
    }
    
    // Public methods for integration
    public string GetCurrentCornerName()
    {
        return currentCornerName;
    }
    
    public bool IsNearCorner()
    {
        return isMarkerVisible;
    }
    
    public int GetCurrentCornerIndex()
    {
        return currentCornerIndex;
    }
} 