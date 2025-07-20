using UnityEngine;

public class PrayerUI : MonoBehaviour
{
    [Header("UI Settings")]
    public bool showPrayerText = true;
    public float displayDuration = 5f;
    
    private string currentPrayerText = "";
    private string currentPrayerType = "";
    private float displayStartTime = 0f;
    private bool isDisplaying = false;
    
    void Start()
    {
        // Find PrayerRecitation and connect to it
        PrayerRecitation prayerRecitation = FindFirstObjectByType<PrayerRecitation>();
        if (prayerRecitation != null)
        {
            // We'll connect through the PrayerRecitation script
        }
    }
    
    public void DisplayPrayer(string prayerText, string prayerType)
    {
        currentPrayerText = prayerText;
        currentPrayerType = prayerType;
        displayStartTime = Time.time;
        isDisplaying = true;
        
        Debug.Log($"PrayerUI: Displaying {prayerType} - {prayerText}");
    }
    
    void Update()
    {
        // Check if display time has expired
        if (isDisplaying && Time.time - displayStartTime > displayDuration)
        {
            isDisplaying = false;
        }
    }
    
    void OnGUI()
    {
        if (!showPrayerText || !isDisplaying) return;
        
        // Calculate fade effect
        float timeElapsed = Time.time - displayStartTime;
        float alpha = 1f;
        
        if (timeElapsed > displayDuration - 1f)
        {
            alpha = 1f - ((timeElapsed - (displayDuration - 1f)) / 1f);
        }
        
        // Set color with alpha
        Color textColor = Color.white;
        textColor.a = alpha;
        GUI.color = textColor;
        
        // Display prayer text
        GUI.skin.label.fontSize = 24;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        
        // Background
        GUI.color = new Color(0, 0, 0, alpha * 0.7f);
        GUI.Box(new Rect(Screen.width/2 - 300, Screen.height/2 - 100, 600, 200), "");
        
        // Prayer type
        GUI.color = new Color(1, 1, 0, alpha); // Yellow
        GUI.Label(new Rect(Screen.width/2 - 300, Screen.height/2 - 80, 600, 40), currentPrayerType);
        
        // Prayer text
        GUI.color = new Color(1, 1, 1, alpha); // White
        GUI.Label(new Rect(Screen.width/2 - 300, Screen.height/2 - 40, 600, 80), currentPrayerText);
    }
} 