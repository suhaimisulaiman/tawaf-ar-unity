using UnityEngine;
using System.Collections.Generic;

public class PrayerRecitation : MonoBehaviour
{
    [Header("Audio Components")]
    public AudioSource audioSource;
    
    [Header("Prayer Settings")]
    public bool enablePrayers = true;
    public bool playCornerPrayers = true;
    public bool playGeneralPrayers = true;
    
    [Header("Prayer Timing")]
    public float prayerInterval = 30f; // Seconds between general prayers
    public float cornerThreshold = 90f; // Degrees to trigger corner prayer (more sensitive for testing)
    
    [Header("Prayer States")]
    public bool isPrayerPlaying = false;
    public int lastCorner = -1;
    public float lastPrayerTime = 0f;
    
    private KaabaTest kaabaTest;
    private PrayerUI prayerUI;
    private int currentCorner = 0;
    private float[] cornerAngles = { 0f, 90f, 180f, 270f }; // Four corners
    private string[] cornerNames = { "Hajar al-Aswad", "Yemeni Corner", "Shami Corner", "Iraqi Corner" };
    
    void Start()
    {
        SetupAudio();
        FindKaabaTest();
    }
    
    void SetupAudio()
    {
        // Create AudioSource if it doesn't exist
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.volume = 0.7f;
        }
    }
    
    void FindKaabaTest()
    {
        kaabaTest = FindFirstObjectByType<KaabaTest>();
        if (kaabaTest == null)
        {
            Debug.LogWarning("PrayerRecitation: KaabaTest not found!");
        }
        
        // Find PrayerUI
        prayerUI = FindFirstObjectByType<PrayerUI>();
        if (prayerUI == null)
        {
            Debug.Log("Creating PrayerUI component...");
            GameObject uiObj = new GameObject("PrayerUI");
            prayerUI = uiObj.AddComponent<PrayerUI>();
        }
    }
    
    void Update()
    {
        if (!enablePrayers || kaabaTest == null) return;
        
        // Check for corner prayers
        if (playCornerPrayers)
        {
            CheckCornerPrayers();
        }
        
        // Check for general prayers
        if (playGeneralPrayers)
        {
            CheckGeneralPrayers();
        }
    }
    
    void CheckCornerPrayers()
    {
        if (kaabaTest == null)
        {
            Debug.LogWarning("PrayerRecitation: KaabaTest is null!");
            return;
        }
        
        if (!kaabaTest.IsKaabaCreated())
        {
            Debug.Log("PrayerRecitation: Kaaba not created yet...");
            return;
        }
        
        // Remove distance restriction for corner prayers - they should work regardless of distance
        // if (!kaabaTest.isValidDistance)
        // {
        //     Debug.Log("PrayerRecitation: Not in valid distance range...");
        //     return;
        // }
        
        // Calculate current angle around Kaaba
        Vector3 toKaaba = kaabaTest.GetKaaba().transform.position - Camera.main.transform.position;
        Vector2 flatDirection = new Vector2(toKaaba.x, toKaaba.z);
        float currentAngle = Mathf.Atan2(flatDirection.y, flatDirection.x) * Mathf.Rad2Deg;
        
        // Normalize angle to 0-360
        if (currentAngle < 0) currentAngle += 360f;
        
        // Check which corner we're closest to
        int nearestCorner = GetNearestCorner(currentAngle);
        
        // Debug logging every 30 frames
        if (Time.frameCount % 30 == 0)
        {
            Debug.Log($"Corner Detection - Current Angle: {currentAngle:F1}°, Nearest Corner: {cornerNames[nearestCorner]}, Last Corner: {(lastCorner >= 0 ? cornerNames[lastCorner] : "None")}");
        }
        
        // If we've moved to a new corner and we're close enough
        if (nearestCorner != lastCorner && Mathf.Abs(currentAngle - cornerAngles[nearestCorner]) < cornerThreshold)
        {
            Debug.Log($"Corner prayer triggered! Moving from {(lastCorner >= 0 ? cornerNames[lastCorner] : "None")} to {cornerNames[nearestCorner]}");
            PlayCornerPrayer(nearestCorner);
            lastCorner = nearestCorner;
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
    
    void CheckGeneralPrayers()
    {
        if (Time.time - lastPrayerTime > prayerInterval && !isPrayerPlaying)
        {
            PlayGeneralPrayer();
            lastPrayerTime = Time.time;
        }
    }
    
    void PlayCornerPrayer(int cornerIndex)
    {
        if (isPrayerPlaying) return;
        
        string cornerName = cornerNames[cornerIndex];
        string prayerText = GetCornerPrayer(cornerIndex);
        
        Debug.Log($"Playing corner prayer for {cornerName}: {prayerText}");
        
        // For now, we'll use text-to-speech or display text
        // In a full implementation, you'd load audio files
        DisplayPrayerText(prayerText, cornerName);
        
        // Simulate prayer duration
        StartCoroutine(PrayerDuration(3f));
    }
    
    void PlayGeneralPrayer()
    {
        if (isPrayerPlaying) return;
        
        string prayerText = GetGeneralPrayer();
        
        Debug.Log($"Playing general prayer: {prayerText}");
        
        DisplayPrayerText(prayerText, "Tawaf Prayer");
        
        // Simulate prayer duration
        StartCoroutine(PrayerDuration(5f));
    }
    
    string GetCornerPrayer(int cornerIndex)
    {
        switch (cornerIndex)
        {
            case 0: // Hajar al-Aswad (Black Stone)
                return "Bismillahi Allahu Akbar";
            case 1: // Yemeni Corner
                return "Rabbana atina fid-dunya hasanatan wa fil-akhirati hasanatan wa qina 'adhaban-nar";
            case 2: // Shami Corner
                return "Allahumma inni as'aluka al-jannah wa a'udhu bika minan-nar";
            case 3: // Iraqi Corner
                return "Allahumma inni as'aluka al-'afwa wal-'afiyah fid-dunya wal-akhirati";
            default:
                return "Subhan Allah";
        }
    }
    
    string GetGeneralPrayer()
    {
        string[] generalPrayers = {
            "Subhan Allah wal-hamdu lillahi wa la ilaha illa Allahu wa Allahu Akbar",
            "Allahumma salli 'ala Muhammadin wa 'ala ali Muhammad",
            "Rabbana taqabbal minna innaka antas-Sami'ul-'Alim",
            "Allahumma inni as'aluka ridaka wal-jannah wa a'udhu bika min sakhatika wan-nar"
        };
        
        return generalPrayers[Random.Range(0, generalPrayers.Length)];
    }
    
    void DisplayPrayerText(string prayerText, string prayerType)
    {
        // Display prayer text on screen
        if (prayerUI != null)
        {
            prayerUI.DisplayPrayer(prayerText, prayerType);
        }
        
        Debug.Log($"[{prayerType}] {prayerText}");
    }
    
    System.Collections.IEnumerator PrayerDuration(float duration)
    {
        isPrayerPlaying = true;
        yield return new WaitForSeconds(duration);
        isPrayerPlaying = false;
    }
    
    // Public methods for integration
    public void PlayRoundCompletionPrayer(int roundNumber)
    {
        if (isPrayerPlaying) return;
        
        string prayerText = $"Round {roundNumber} completed. Alhamdulillah!";
        Debug.Log($"Round completion prayer: {prayerText}");
        
        DisplayPrayerText(prayerText, "Round Completion");
        StartCoroutine(PrayerDuration(2f));
    }
    
    public void PlayTawafCompletionPrayer()
    {
        if (isPrayerPlaying) return;
        
        string prayerText = "Tawaf completed! Allahu Akbar! May Allah accept your Tawaf.";
        Debug.Log($"Tawaf completion prayer: {prayerText}");
        
        DisplayPrayerText(prayerText, "Tawaf Completion");
        StartCoroutine(PrayerDuration(4f));
    }
    
    void OnGUI()
    {
        if (!enablePrayers) return;
        
        // Show prayer status
        GUI.color = isPrayerPlaying ? Color.green : Color.white;
        GUI.Label(new Rect(20, Screen.height - 100, 400, 30), $"Prayer Status: {(isPrayerPlaying ? "Playing" : "Ready")}");
        
        if (lastCorner >= 0)
        {
            GUI.color = Color.yellow;
            GUI.Label(new Rect(20, Screen.height - 70, 400, 30), $"Last Corner: {cornerNames[lastCorner]}");
        }
        
        // Show current corner detection
        if (kaabaTest != null && kaabaTest.IsKaabaCreated())
        {
            Vector3 toKaaba = kaabaTest.GetKaaba().transform.position - Camera.main.transform.position;
            Vector2 flatDirection = new Vector2(toKaaba.x, toKaaba.z);
            float currentAngle = Mathf.Atan2(flatDirection.y, flatDirection.x) * Mathf.Rad2Deg;
            if (currentAngle < 0) currentAngle += 360f;
            
            int nearestCorner = GetNearestCorner(currentAngle);
            GUI.color = Color.cyan;
            GUI.Label(new Rect(20, Screen.height - 40, 400, 30), $"Current Corner: {cornerNames[nearestCorner]} ({currentAngle:F1}°)");
        }
    }
} 