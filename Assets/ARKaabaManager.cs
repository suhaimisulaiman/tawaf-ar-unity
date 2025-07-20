using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ARKaabaManager : MonoBehaviour
{
    [Header("AR References")]
    public ARPlaneManager planeManager;
    public GameObject kaabaPrefab;
    
    [Header("Scripts")]
    public MovementTracker movementTracker;
    public UIManager uiManager;
    
    private GameObject kaabaInstance;
    private bool kaabaPlaced = false;
    
    void Start()
    {
        // Find AR components
        planeManager = FindFirstObjectByType<ARPlaneManager>();
        movementTracker = GetComponent<MovementTracker>();
        uiManager = GetComponent<UIManager>();
    }
    
    void Update()
    {
        // Tap to place Kaaba on detected plane
        if (!kaabaPlaced && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                PlaceKaaba(touch.position);
            }
        }
        
        // For testing in editor - click to place
        if (!kaabaPlaced && Input.GetMouseButtonDown(0))
        {
            PlaceKaaba(Input.mousePosition);
        }
    }
    
    void PlaceKaaba(Vector2 screenPosition)
    {
        // Raycast to find plane
        var hits = new List<ARRaycastHit>();
        var raycastManager = FindFirstObjectByType<ARRaycastManager>();
        
        if (raycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            var hit = hits[0];
            
            // Create Kaaba at hit position
            kaabaInstance = GameObject.CreatePrimitive(PrimitiveType.Cube);
            kaabaInstance.name = "Kaaba";
            kaabaInstance.transform.position = hit.pose.position;
            kaabaInstance.transform.localScale = new Vector3(2f, 3f, 2f);
            
            // Create proper black material for Kaaba
            Renderer renderer = kaabaInstance.GetComponent<Renderer>();
            renderer.material.color = Color.black;
            
            // Add a gold door (small cube on one face)
            GameObject door = GameObject.CreatePrimitive(PrimitiveType.Cube);
            door.name = "KaabaDoor";
            door.transform.SetParent(kaabaInstance.transform);
            door.transform.localPosition = new Vector3(0, 0, 1.01f); // Slightly in front
            door.transform.localScale = new Vector3(0.6f, 1.2f, 0.02f);
            
            // Create gold material for door
            Renderer doorRenderer = door.GetComponent<Renderer>();
            doorRenderer.material.color = new Color(1f, 0.8f, 0.2f); // Gold color
            
            // Connect to movement tracker
            if (movementTracker != null)
            {
                movementTracker.kaabaTransform = kaabaInstance.transform;
            }
            
            kaabaPlaced = true;
            Debug.Log("Kaaba placed in AR!");
        }
    }
}