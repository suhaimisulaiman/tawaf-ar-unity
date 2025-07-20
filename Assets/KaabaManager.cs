using UnityEngine;

public class KaabaManager : MonoBehaviour
{
    [Header("Kaaba Settings")]
    public GameObject kaabaPrefab;
    public Vector3 kaabaSize = new Vector3(2f, 3f, 2f); // width, height, depth
    
    private GameObject kaabaInstance;
    private bool kaabaPlaced = false;
    
    void Start()
    {
        CreateKaaba();
    }
    
    void CreateKaaba()
    {
        // Create a simple cube for now (we'll replace with 3D model later)
        kaabaInstance = GameObject.CreatePrimitive(PrimitiveType.Cube);
        kaabaInstance.name = "Kaaba";
        
        // Set size
        kaabaInstance.transform.localScale = kaabaSize;
        
        // Position it in front of user
        kaabaInstance.transform.position = new Vector3(0, 1.5f, 5f);
        
        // Make it black (traditional Kaaba color)
        Renderer renderer = kaabaInstance.GetComponent<Renderer>();
        renderer.material.color = Color.black;
        
        kaabaPlaced = true;
        
        // NEW: Tell MovementTracker about the Kaaba
        MovementTracker tracker = GetComponent<MovementTracker>();
        if (tracker != null)
        {
            tracker.kaabaTransform = kaabaInstance.transform;
            Debug.Log("Kaaba connected to MovementTracker!");
        }
        
        Debug.Log("Kaaba created successfully!");
    }
    
    public Vector3 GetKaabaPosition()
    {
        if (kaabaPlaced && kaabaInstance != null)
        {
            return kaabaInstance.transform.position;
        }
        return Vector3.zero;
    }
    
    public bool IsKaabaPlaced()
    {
        return kaabaPlaced;
    }
}