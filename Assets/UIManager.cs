using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public Canvas mainCanvas;
    public Text roundText;
    public Text distanceText;
    public Text instructionText;
    public Slider progressSlider;
    public GameObject celebrationPanel;
    
    private MovementTracker movementTracker;
    
    void Start()
    {
        CreateUI();
        movementTracker = FindFirstObjectByType<MovementTracker>();
        
        // DEBUG: Check if MovementTracker was found
        if (movementTracker != null)
        {
            Debug.Log("UIManager found MovementTracker!");
        }
        else
        {
            Debug.Log("ERROR: UIManager could NOT find MovementTracker!");
        }
    }
    
    void CreateUI()
    {
        // Create main canvas
        GameObject canvasObj = new GameObject("TawafUI");
        mainCanvas = canvasObj.AddComponent<Canvas>();
        mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Create round counter (top center)
        CreateRoundCounter();
        
        // Create progress bar (bottom)
        CreateProgressBar();
        
        // Create instructions (center)
        CreateInstructions();
        
        // Create celebration panel (hidden initially)
        CreateCelebrationPanel();
    }
    
    void CreateRoundCounter()
    {
        GameObject roundObj = new GameObject("RoundText");
        roundObj.transform.SetParent(mainCanvas.transform, false);
        
        roundText = roundObj.AddComponent<Text>();
        roundText.text = "Round 1 of 7";
        roundText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        roundText.fontSize = 40;
        roundText.color = Color.white;
        roundText.alignment = TextAnchor.MiddleCenter;
        
        // Position at top center
        RectTransform rectTransform = roundText.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 1f);
        rectTransform.anchorMax = new Vector2(0.5f, 1f);
        rectTransform.anchoredPosition = new Vector2(0, -60);
        rectTransform.sizeDelta = new Vector2(300, 60);
    }
    
    void CreateProgressBar()
    {
        GameObject sliderObj = new GameObject("ProgressSlider");
        sliderObj.transform.SetParent(mainCanvas.transform, false);
        
        progressSlider = sliderObj.AddComponent<Slider>();
        progressSlider.minValue = 0;
        progressSlider.maxValue = 7;
        progressSlider.value = 0;
        
        // Style the slider background
        Image background = sliderObj.AddComponent<Image>();
        background.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
        
        // Create fill area for the progress bar
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform, false);
        
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = Vector2.zero;
        fillAreaRect.offsetMax = Vector2.zero;
        
        // Create fill image
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.2f, 0.8f, 0.2f, 1f); // Green color for progress
        
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        
        // Assign the fill to the slider
        progressSlider.fillRect = fillRect;
        
        // Position at bottom
        RectTransform rectTransform = progressSlider.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.1f, 0f);
        rectTransform.anchorMax = new Vector2(0.9f, 0f);
        rectTransform.anchoredPosition = new Vector2(0, 50);
        rectTransform.sizeDelta = new Vector2(0, 20);
        
        Debug.Log("Progress bar created with fill area");
    }
    
    void CreateInstructions()
    {
        GameObject instructionObj = new GameObject("InstructionText");
        instructionObj.transform.SetParent(mainCanvas.transform, false);
        
        instructionText = instructionObj.AddComponent<Text>();
        instructionText.text = "Use Q/E to circle the Kaaba\nPress C to test round completion";
        instructionText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        instructionText.fontSize = 24;
        instructionText.color = new Color(1f, 1f, 1f, 0.8f);
        instructionText.alignment = TextAnchor.MiddleCenter;
        
        // Position at center
        RectTransform rectTransform = instructionText.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = new Vector2(0, -100);
        rectTransform.sizeDelta = new Vector2(400, 100);
    }
    
    void CreateCelebrationPanel()
    {
        GameObject celebrationObj = new GameObject("CelebrationPanel");
        celebrationObj.transform.SetParent(mainCanvas.transform, false);
        
        // Background
        Image background = celebrationObj.AddComponent<Image>();
        background.color = new Color(0f, 0.8f, 0f, 0.9f);
        
        // Text
        GameObject textObj = new GameObject("CelebrationText");
        textObj.transform.SetParent(celebrationObj.transform, false);
        
        Text celebrationText = textObj.AddComponent<Text>();
        celebrationText.text = "🎉 TAWAF COMPLETED! 🎉\nAll 7 rounds finished!";
        celebrationText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        celebrationText.fontSize = 32;
        celebrationText.color = Color.white;
        celebrationText.alignment = TextAnchor.MiddleCenter;
        
        // Position celebration panel (full screen, hidden)
        RectTransform rectTransform = celebrationObj.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        
        RectTransform textRect = celebrationText.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        celebrationPanel = celebrationObj;
        celebrationPanel.SetActive(false);
    }
    
    void Update()
    {
        if (movementTracker != null)
        {
            UpdateUI();
        }
    }
    
    void UpdateUI()
    {
        // DEBUG: Show current values
        Debug.Log($"Updating UI - Current Round: {movementTracker.currentRound}, Progress Bar Value: {progressSlider.value}");
        
        // Update round text
        roundText.text = $"Round {movementTracker.currentRound} of {movementTracker.totalRounds}";
        
        // Update progress bar
        progressSlider.value = movementTracker.currentRound;
        
        // DEBUG: Log after update
        Debug.Log($"After update - Progress Bar Value: {progressSlider.value}");
        
        // Show celebration if completed
        if (movementTracker.currentRound >= movementTracker.totalRounds)
        {
            celebrationPanel.SetActive(true);
        }
    }
}