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
    public float prayerCooldown = 3f; // Minimum time between any prayers to prevent overlapping
    
    [Header("Prayer States")]
    public bool isPrayerPlaying = false;
    public int lastCorner = -1;
    public float lastPrayerTime = 0f;
    
    private TawafController tawafController;
    private PrayerUI prayerUI;
    private int currentCorner = 0;
    private float[] cornerAngles = { 90f, 180f, 270f, 0f }; // Four corners - Hajar al-Aswad at 90° (right side)
    private string[] cornerNames = { "Hajar al-Aswad", "Yemeni Corner", "Shami Corner", "Iraqi Corner" };
    private string currentPrayerText = "";
    private string currentPrayerType = "";
    
    void Start()
    {
        SetupAudio();
        FindTawafController();
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
    
    void FindTawafController()
    {
        tawafController = FindFirstObjectByType<TawafController>();
        if (tawafController == null)
        {
            Debug.LogWarning("PrayerRecitation: TawafController not found!");
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
        if (!enablePrayers || tawafController == null) return;
        
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
        if (tawafController == null)
        {
            Debug.LogWarning("PrayerRecitation: TawafController is null!");
            return;
        }
        
        if (!tawafController.IsKaabaCreated())
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
        Vector3 toKaaba = tawafController.GetKaaba().transform.position - Camera.main.transform.position;
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
        // Check if prayer is already playing or in cooldown
        if (isPrayerPlaying || Time.time - lastPrayerTime < prayerCooldown) 
        {
            Debug.Log($"Prayer blocked - Playing: {isPrayerPlaying}, Cooldown: {Time.time - lastPrayerTime:F1}s");
            return;
        }
        
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
        // Check if prayer is already playing or in cooldown
        if (isPrayerPlaying || Time.time - lastPrayerTime < prayerCooldown) 
        {
            return;
        }
        
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
                return "بِسْمِ اللَّهِ اللَّهُ أَكْبَر\nBismillahi Allahu Akbar\nIn the name of Allah, Allah is the Greatest";
            case 1: // Yemeni Corner
                return "رَبَّنَا آتِنَا فِي الدُّنْيَا حَسَنَةً وَفِي الْآخِرَةِ حَسَنَةً وَقِنَا عَذَابَ النَّارِ\nRabbana atina fid-dunya hasanatan wa fil-akhirati hasanatan wa qina 'adhaban-nar\nOur Lord, grant us good in this world and good in the Hereafter and protect us from the punishment of the Fire";
            case 2: // Shami Corner
                return "اللَّهُمَّ إِنِّي أَسْأَلُكَ الْجَنَّةَ وَأَعُوذُ بِكَ مِنَ النَّارِ\nAllahumma inni as'aluka al-jannah wa a'udhu bika minan-nar\nO Allah, I ask You for Paradise and I seek refuge with You from the Fire";
            case 3: // Iraqi Corner
                return "اللَّهُمَّ إِنِّي أَسْأَلُكَ الْعَفْوَ وَالْعَافِيَةَ فِي الدُّنْيَا وَالْآخِرَةِ\nAllahumma inni as'aluka al-'afwa wal-'afiyah fid-dunya wal-akhirati\nO Allah, I ask You for forgiveness and well-being in this world and the Hereafter";
            default:
                return "سُبْحَانَ اللَّهِ\nSubhan Allah\nGlory be to Allah";
        }
    }
    
    string GetGeneralPrayer()
    {
        string[] generalPrayers = {
            "سُبْحَانَ اللَّهِ وَالْحَمْدُ لِلَّهِ وَلَا إِلَهَ إِلَّا اللَّهُ وَاللَّهُ أَكْبَر\nSubhan Allah wal-hamdu lillahi wa la ilaha illa Allahu wa Allahu Akbar\nGlory be to Allah, and praise be to Allah, and there is no god but Allah, and Allah is the Greatest",
            
            "اللَّهُمَّ صَلِّ عَلَى مُحَمَّدٍ وَعَلَى آلِ مُحَمَّدٍ\nAllahumma salli 'ala Muhammadin wa 'ala ali Muhammad\nO Allah, send prayers upon Muhammad and the family of Muhammad",
            
            "رَبَّنَا تَقَبَّلْ مِنَّا إِنَّكَ أَنْتَ السَّمِيعُ الْعَلِيمُ\nRabbana taqabbal minna innaka antas-Sami'ul-'Alim\nOur Lord, accept this from us, for You are the All-Hearing, the All-Knowing",
            
            "اللَّهُمَّ إِنِّي أَسْأَلُكَ رِضَاكَ وَالْجَنَّةَ وَأَعُوذُ بِكَ مِنْ سَخَطِكَ وَالنَّارِ\nAllahumma inni as'aluka ridaka wal-jannah wa a'udhu bika min sakhatika wan-nar\nO Allah, I ask You for Your pleasure and Paradise, and I seek refuge with You from Your wrath and the Fire",
            
            "لَا إِلَهَ إِلَّا اللَّهُ وَحْدَهُ لَا شَرِيكَ لَهُ لَهُ الْمُلْكُ وَلَهُ الْحَمْدُ وَهُوَ عَلَى كُلِّ شَيْءٍ قَدِيرٌ\nLa ilaha illa Allahu wahdahu la shareeka lahu, lahul-mulku wa lahul-hamdu wa huwa 'ala kulli shay'in qadeer\nThere is no god but Allah alone, with no partner. His is the dominion and His is the praise, and He is able to do all things",
            
            "سُبْحَانَ اللَّهِ وَبِحَمْدِهِ سُبْحَانَ اللَّهِ الْعَظِيمِ\nSubhan Allahi wa bihamdihi, subhan Allahil-'adheem\nGlory be to Allah and His is the praise, glory be to Allah the Most Great"
        };
        
        return generalPrayers[Random.Range(0, generalPrayers.Length)];
    }
    
    void DisplayPrayerText(string prayerText, string prayerType)
    {
        // Store current prayer for display
        currentPrayerText = prayerText;
        currentPrayerType = prayerType;
        
        // Don't call PrayerUI to avoid double display
        // if (prayerUI != null)
        // {
        //     prayerUI.DisplayPrayer(prayerText, prayerType);
        // }
        
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
        // Check if prayer is already playing or in cooldown
        if (isPrayerPlaying || Time.time - lastPrayerTime < prayerCooldown) 
        {
            return;
        }
        
        string prayerText = $"الْحَمْدُ لِلَّهِ\nAlhamdulillah!\nRound {roundNumber} completed. Praise be to Allah!";
        Debug.Log($"Round completion prayer: {prayerText}");
        
        DisplayPrayerText(prayerText, "Round Completion");
        StartCoroutine(PrayerDuration(3f));
    }
    
    public void PlayTawafCompletionPrayer()
    {
        // Check if prayer is already playing or in cooldown
        if (isPrayerPlaying || Time.time - lastPrayerTime < prayerCooldown) 
        {
            return;
        }
        
        string prayerText = "اللَّهُ أَكْبَرُ\nAllahu Akbar!\nTawaf completed! May Allah accept your Tawaf and grant you His mercy.";
        Debug.Log($"Tawaf completion prayer: {prayerText}");
        
        DisplayPrayerText(prayerText, "Tawaf Completion");
        StartCoroutine(PrayerDuration(5f));
    }
    
    // Istilam prayer when touching/kissing Hajar al-Aswad
    public void PlayIstilamPrayer()
    {
        // Check if prayer is already playing or in cooldown
        if (isPrayerPlaying || Time.time - lastPrayerTime < prayerCooldown) 
        {
            return;
        }
        
        string prayerText = "بِسْمِ اللَّهِ وَاللَّهُ أَكْبَرُ\nBismillahi wa Allahu Akbar\nIn the name of Allah, and Allah is the Greatest";
        Debug.Log($"Istilam prayer: {prayerText}");
        
        DisplayPrayerText(prayerText, "Istilam");
        StartCoroutine(PrayerDuration(2f));
    }
    
    // Additional Tawaf prayers
    public void PlayTawafStartPrayer()
    {
        // Check if prayer is already playing or in cooldown
        if (isPrayerPlaying || Time.time - lastPrayerTime < prayerCooldown) 
        {
            return;
        }
        
        string prayerText = "اللَّهُمَّ إِنِّي أُرِيدُ الطَّوَافَ بِبَيْتِكَ الْحَرَامِ فَيَسِّرْهُ لِي وَتَقَبَّلْهُ مِنِّي\nAllahumma inni ureedu tawafa bi baytikal-haram, fayassirhu lee wa taqabbalhu minnee\nO Allah, I intend to perform Tawaf around Your Sacred House, so make it easy for me and accept it from me";
        Debug.Log($"Tawaf start prayer: {prayerText}");
        
        DisplayPrayerText(prayerText, "Tawaf Start");
        StartCoroutine(PrayerDuration(4f));
    }
    
    void OnGUI()
    {
        if (!enablePrayers) return;
        
        // Enhanced prayer display with better formatting and no fixed height
        if (isPrayerPlaying)
        {
            // Prayer overlay with larger, more readable text
            GUI.color = new Color(0, 0, 0, 0.85f);
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
            
            // Calculate dynamic height based on text content
            string[] lines = currentPrayerText.Split('\n');
            int lineCount = lines.Length;
            int dynamicHeight = Mathf.Max(400, lineCount * 35 + 100); // Minimum 400px, 35px per line + padding
            
            // Prayer card background with dynamic height
            GUI.color = new Color(0.1f, 0.1f, 0.1f, 0.98f);
            GUI.Box(new Rect(Screen.width/2 - 350, Screen.height/2 - dynamicHeight/2, 700, dynamicHeight), "");
            
            // Inner card with border
            GUI.color = new Color(0.15f, 0.15f, 0.15f, 0.95f);
            GUI.Box(new Rect(Screen.width/2 - 340, Screen.height/2 - dynamicHeight/2 + 10, 680, dynamicHeight - 20), "");
            
            // Prayer title with corner name
            GUI.color = new Color(1f, 0.8f, 0.2f); // Gold color for title
            GUI.skin.label.fontSize = 28;
            GUI.skin.label.fontStyle = FontStyle.Bold;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(Screen.width/2 - 340, Screen.height/2 - dynamicHeight/2 + 20, 680, 40), $"🕌 {currentPrayerType}");
            
            // Prayer text with better formatting
            GUI.color = Color.white;
            GUI.skin.label.fontSize = 22;
            GUI.skin.label.fontStyle = FontStyle.Normal;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            
            // Split prayer text into sections for better formatting
            string[] sections = currentPrayerText.Split('\n');
            int yOffset = 80;
            int lineHeight = 35;
            
            foreach (string section in sections)
            {
                if (!string.IsNullOrEmpty(section.Trim()))
                {
                    // Arabic text (usually first line) - larger and centered
                    if (section.Contains("ال") || section.Contains("بِسْمِ") || section.Contains("سُبْحَانَ") || 
                        section.Contains("اللَّهُمَّ") || section.Contains("رَبَّنَا") || section.Contains("لَا إِلَهَ"))
                    {
                        GUI.color = new Color(0.2f, 0.8f, 1f); // Light blue for Arabic
                        GUI.skin.label.fontSize = 26;
                        GUI.Label(new Rect(Screen.width/2 - 340, Screen.height/2 - dynamicHeight/2 + yOffset, 680, lineHeight), section);
                        yOffset += lineHeight + 10;
                    }
                    // Transliteration - medium size
                    else if (section.Contains("Allah") || section.Contains("Bismillah") || section.Contains("Subhan") ||
                             section.Contains("Rabbana") || section.Contains("Allahumma") || section.Contains("La ilaha"))
                    {
                        GUI.color = new Color(0.9f, 0.9f, 0.9f); // Light gray for transliteration
                        GUI.skin.label.fontSize = 20;
                        GUI.Label(new Rect(Screen.width/2 - 340, Screen.height/2 - dynamicHeight/2 + yOffset, 680, lineHeight), section);
                        yOffset += lineHeight + 5;
                    }
                    // English translation - smaller size
                    else
                    {
                        GUI.color = new Color(0.7f, 0.7f, 0.7f); // Gray for translation
                        GUI.skin.label.fontSize = 18;
                        GUI.Label(new Rect(Screen.width/2 - 340, Screen.height/2 - dynamicHeight/2 + yOffset, 680, lineHeight), section);
                        yOffset += lineHeight + 5;
                    }
                }
            }
            
            // Reset font settings
            GUI.skin.label.fontSize = 12;
            GUI.skin.label.fontStyle = FontStyle.Normal;
            GUI.skin.label.alignment = TextAnchor.UpperLeft;
        }
        
        // Show prayer status (smaller and less intrusive)
        GUI.color = isPrayerPlaying ? Color.green : Color.white;
        GUI.skin.label.fontSize = 14;
        GUI.Label(new Rect(20, Screen.height - 80, 300, 25), $"Prayer: {(isPrayerPlaying ? currentPrayerType : "Ready")}");
        
        if (lastCorner >= 0)
        {
            GUI.color = Color.yellow;
            GUI.Label(new Rect(20, Screen.height - 55, 300, 25), $"Last: {cornerNames[lastCorner]}");
        }
        
        // Show current corner detection
        if (tawafController != null && tawafController.IsKaabaCreated())
        {
            Vector3 toKaaba = tawafController.GetKaaba().transform.position - Camera.main.transform.position;
            Vector2 flatDirection = new Vector2(toKaaba.x, toKaaba.z);
            float currentAngle = Mathf.Atan2(flatDirection.y, flatDirection.x) * Mathf.Rad2Deg;
            if (currentAngle < 0) currentAngle += 360f;
            
            int nearestCorner = GetNearestCorner(currentAngle);
            GUI.color = Color.cyan;
            GUI.Label(new Rect(20, Screen.height - 30, 300, 25), $"Near: {cornerNames[nearestCorner]} ({currentAngle:F1}°)");
        }
    }
} 