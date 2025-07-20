using UnityEngine;

public class RealisticKaabaBuilder : MonoBehaviour
{
    [Header("Kaaba Dimensions")]
    public float kaabaHeight = 15f; // 15 meters in real life
    public float kaabaWidth = 12f;  // 12 meters in real life
    public float kaabaDepth = 12f;  // 12 meters in real life
    
    [Header("Materials")]
    public Material blackStoneMaterial;
    public Material goldMaterial;
    public Material whiteMaterial;
    public Material grayMaterial;
    
    [Header("Components")]
    public bool includeKiswah = true; // Black cloth covering
    public bool includeGoldBelt = true; // Gold belt around Kaaba
    public bool includeDoor = true; // Golden door
    public bool includeHajarAswad = true; // Black stone
    public bool includeYemeniCorner = true; // Yemeni corner stone
    public bool includeRainGutter = true; // Mizab (rain gutter)
    
    private GameObject kaabaMain;
    private GameObject kiswahCover;
    private GameObject goldBelt;
    private GameObject door;
    private GameObject hajarAswad;
    private GameObject yemeniCorner;
    private GameObject rainGutter;
    
    void Start()
    {
        CreateRealisticKaaba();
    }
    
    public void CreateRealisticKaaba()
    {
        // Create main Kaaba structure
        CreateMainStructure();
        
        // Add details
        if (includeKiswah) CreateKiswah();
        if (includeGoldBelt) CreateGoldBelt();
        if (includeDoor) CreateDoor();
        if (includeHajarAswad) CreateHajarAswad();
        if (includeYemeniCorner) CreateYemeniCorner();
        if (includeRainGutter) CreateRainGutter();
        
        // Position everything correctly
        PositionComponents();
        
        Debug.Log("Realistic Kaaba created successfully!");
    }
    
    void CreateMainStructure()
    {
        kaabaMain = GameObject.CreatePrimitive(PrimitiveType.Cube);
        kaabaMain.name = "Kaaba_Main";
        kaabaMain.transform.localScale = new Vector3(kaabaWidth, kaabaHeight, kaabaDepth);
        
        // Apply gray stone material
        Renderer renderer = kaabaMain.GetComponent<Renderer>();
        if (grayMaterial != null)
        {
            renderer.material = grayMaterial;
        }
        else
        {
            // Create default gray material
            Material defaultGray = new Material(Shader.Find("Standard"));
            defaultGray.color = new Color(0.4f, 0.4f, 0.4f);
            renderer.material = defaultGray;
        }
        
        kaabaMain.transform.SetParent(transform);
    }
    
    void CreateKiswah()
    {
        kiswahCover = GameObject.CreatePrimitive(PrimitiveType.Cube);
        kiswahCover.name = "Kaaba_Kiswah";
        kiswahCover.transform.localScale = new Vector3(kaabaWidth + 0.1f, kaabaHeight + 0.1f, kaabaDepth + 0.1f);
        
        // Apply black material for Kiswah
        Renderer renderer = kiswahCover.GetComponent<Renderer>();
        if (blackStoneMaterial != null)
        {
            renderer.material = blackStoneMaterial;
        }
        else
        {
            // Create default black material
            Material defaultBlack = new Material(Shader.Find("Standard"));
            defaultBlack.color = new Color(0.05f, 0.05f, 0.05f);
            renderer.material = defaultBlack;
        }
        
        kiswahCover.transform.SetParent(transform);
    }
    
    void CreateGoldBelt()
    {
        goldBelt = GameObject.CreatePrimitive(PrimitiveType.Cube);
        goldBelt.name = "Kaaba_GoldBelt";
        goldBelt.transform.localScale = new Vector3(kaabaWidth + 0.2f, 1f, kaabaDepth + 0.2f);
        
        // Apply gold material
        Renderer renderer = goldBelt.GetComponent<Renderer>();
        if (goldMaterial != null)
        {
            renderer.material = goldMaterial;
        }
        else
        {
            // Create default gold material
            Material defaultGold = new Material(Shader.Find("Standard"));
            defaultGold.color = new Color(1f, 0.8f, 0.2f);
            defaultGold.SetFloat("_Metallic", 1f);
            defaultGold.SetFloat("_Smoothness", 0.8f);
            renderer.material = defaultGold;
        }
        
        goldBelt.transform.SetParent(transform);
    }
    
    void CreateDoor()
    {
        door = GameObject.CreatePrimitive(PrimitiveType.Cube);
        door.name = "Kaaba_Door";
        door.transform.localScale = new Vector3(2f, 3f, 0.1f);
        
        // Apply gold material
        Renderer renderer = door.GetComponent<Renderer>();
        if (goldMaterial != null)
        {
            renderer.material = goldMaterial;
        }
        else
        {
            // Create default gold material
            Material defaultGold = new Material(Shader.Find("Standard"));
            defaultGold.color = new Color(1f, 0.8f, 0.2f);
            defaultGold.SetFloat("_Metallic", 1f);
            defaultGold.SetFloat("_Smoothness", 0.8f);
            renderer.material = defaultGold;
        }
        
        door.transform.SetParent(transform);
    }
    
    void CreateHajarAswad()
    {
        hajarAswad = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        hajarAswad.name = "Kaaba_HajarAswad";
        hajarAswad.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        
        // Apply black stone material
        Renderer renderer = hajarAswad.GetComponent<Renderer>();
        if (blackStoneMaterial != null)
        {
            renderer.material = blackStoneMaterial;
        }
        else
        {
            // Create default black stone material
            Material defaultBlackStone = new Material(Shader.Find("Standard"));
            defaultBlackStone.color = new Color(0.02f, 0.02f, 0.02f);
            defaultBlackStone.SetFloat("_Metallic", 0.1f);
            defaultBlackStone.SetFloat("_Smoothness", 0.3f);
            renderer.material = defaultBlackStone;
        }
        
        hajarAswad.transform.SetParent(transform);
    }
    
    void CreateYemeniCorner()
    {
        yemeniCorner = GameObject.CreatePrimitive(PrimitiveType.Cube);
        yemeniCorner.name = "Kaaba_YemeniCorner";
        yemeniCorner.transform.localScale = new Vector3(0.5f, kaabaHeight, 0.5f);
        
        // Apply white material
        Renderer renderer = yemeniCorner.GetComponent<Renderer>();
        if (whiteMaterial != null)
        {
            renderer.material = whiteMaterial;
        }
        else
        {
            // Create default white material
            Material defaultWhite = new Material(Shader.Find("Standard"));
            defaultWhite.color = new Color(0.9f, 0.9f, 0.9f);
            renderer.material = defaultWhite;
        }
        
        yemeniCorner.transform.SetParent(transform);
    }
    
    void CreateRainGutter()
    {
        rainGutter = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rainGutter.name = "Kaaba_RainGutter";
        rainGutter.transform.localScale = new Vector3(kaabaWidth + 0.3f, 0.2f, 0.3f);
        
        // Apply gold material
        Renderer renderer = rainGutter.GetComponent<Renderer>();
        if (goldMaterial != null)
        {
            renderer.material = goldMaterial;
        }
        else
        {
            // Create default gold material
            Material defaultGold = new Material(Shader.Find("Standard"));
            defaultGold.color = new Color(1f, 0.8f, 0.2f);
            defaultGold.SetFloat("_Metallic", 1f);
            defaultGold.SetFloat("_Smoothness", 0.8f);
            renderer.material = defaultGold;
        }
        
        rainGutter.transform.SetParent(transform);
    }
    
    void PositionComponents()
    {
        // Position main structure
        kaabaMain.transform.position = transform.position;
        
        // Position Kiswah (slightly larger, covering main structure)
        if (kiswahCover != null)
        {
            kiswahCover.transform.position = transform.position;
        }
        
        // Position gold belt (around middle height)
        if (goldBelt != null)
        {
            goldBelt.transform.position = transform.position + Vector3.up * (kaabaHeight / 2f);
        }
        
        // Position door (front face, slightly elevated)
        if (door != null)
        {
            door.transform.position = transform.position + 
                Vector3.forward * (kaabaDepth / 2f + 0.05f) + 
                Vector3.up * (kaabaHeight / 2f - 1.5f);
        }
        
        // Position Hajar al-Aswad (front face, top left corner)
        if (hajarAswad != null)
        {
            hajarAswad.transform.position = transform.position + 
                Vector3.forward * (kaabaDepth / 2f + 0.15f) + 
                Vector3.left * (kaabaWidth / 2f - 0.5f) + 
                Vector3.up * (kaabaHeight - 0.5f);
        }
        
        // Position Yemeni corner (right side)
        if (yemeniCorner != null)
        {
            yemeniCorner.transform.position = transform.position + 
                Vector3.right * (kaabaWidth / 2f + 0.25f);
        }
        
        // Position rain gutter (top, front edge)
        if (rainGutter != null)
        {
            rainGutter.transform.position = transform.position + 
                Vector3.up * kaabaHeight + 
                Vector3.forward * (kaabaDepth / 2f + 0.15f);
        }
    }
    
    // Public method to get the main Kaaba object
    public GameObject GetKaabaObject()
    {
        return kaabaMain;
    }
    
    // Method to scale Kaaba for AR (smaller size for mobile viewing)
    public void ScaleForAR(float scaleFactor = 0.1f)
    {
        transform.localScale = Vector3.one * scaleFactor;
    }
    
    // Method to add lighting effects
    public void AddLighting()
    {
        // Add ambient lighting to highlight the Kaaba
        GameObject light = new GameObject("Kaaba_Light");
        Light pointLight = light.AddComponent<Light>();
        pointLight.type = LightType.Point;
        pointLight.intensity = 2f;
        pointLight.range = 20f;
        pointLight.color = new Color(1f, 0.95f, 0.8f); // Warm light
        light.transform.position = transform.position + Vector3.up * 10f;
        light.transform.SetParent(transform);
    }
} 