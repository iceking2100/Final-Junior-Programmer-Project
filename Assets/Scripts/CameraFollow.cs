using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target & Offset")]
    [Tooltip("The Transform the camera should follow e.g., your player).")]
    [SerializeField]
    private Transform target; // the player's transform

    [Tooltip("Offset position relative to the target. Example: (0, 2, -10) for 2D, where Z is the camera distance")]
    [SerializeField]
    private Vector3 offset = new Vector3(6f, 6f, -29f); // default offset for a 2D camera

    [Header("Smoothing")]
    [Tooltip("How smoothly the camera follows the target. Higher values mean faster following")]
    [SerializeField]
    private float smoothSpeed = 0.125f; // default smoothing speed


    // --- Recommended: Specific Y-Axis Clamping ---
    [Header("Vertical Camera Clamp")]
    [Tooltip("Enable or disable vertical (Y-axis) clamping.")]
    [SerializeField]
    private bool enableYClamp = false;

    [Tooltip("The minimum Y-position the camera can go.")]
    [SerializeField]
    private float minYClamp = 6f; // default minimum Y position

    [Tooltip("The maximum Y-position the camera can go.")]
    [SerializeField]
    private float maxYClamp = 6f; // default maximum Y position


    // --- Optional: Full Camera Bounds (for X and Y) if you decide to use it later ---
    // This setup replaces the minCameraBounds/maxCameraBounds Vector2s with clear float values for X and Y bounds.
    // or you can revert to using Vector2s if you want to define boundaries for both axes.
    // I'll keep the current Vector2 format commented out for now, but you can uncomment it if you want to use it later.
    [Header("Camera Bounds (Optional)")]
    [Tooltip("The minimum X and Y bounds for the camera.")]
    [SerializeField] private bool enableFullBounds = false; // Enable or disable full camera bounds
    [SerializeField] private float minCameraBounds; // default minimum camera position
    [SerializeField] private float maxCameraBounds; // default maximum camera position
    [SerializeField] private float minCameraBoundsZ; // default minimum camera position for Z
    [SerializeField] private float maxCameraBoundsZ; // default maximum camera position for Z


    // [SerializeField] private Vector2 minCameraBounds = new Vector2(0, 0);
    // [SerializeField] private Vector2 maxCameraBounds = new Vector2(0,6);


    private Vector3 currentVelocity = Vector3.zero; // Used for SmoothDamp to track the current velocity of the camera

    // LateUpdate is called after all Update methods have been called
    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("Target is not assigned in CameraFollow script. Please assign a target to follow.");
            return;
        }

        // 1. Calculate the desired position based on the target's position and the offset
        Vector3 desiredPosition = target.position + offset;


        // 2. Smoothly move the camera towards the desired position using Lerp or SmoothDamp
        // This calculates an intermediate 'smoothedPosition'
         Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothSpeed);
        // Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        // Using SmoothDamp

        // 3. apply ONLY Vertical (Y-axis) clamping to the smoothed posistion
         if (enableYClamp)
         {
             // Clamp the Y position if vertical clamping is enabled
             smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minYClamp, maxYClamp);
         }

        // --- If you were to use full X and Y bounds it would go here instead of (or in addition to) the Y clamp:
 
        if (enableFullBounds) 
        {
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minCameraBounds, maxCameraBounds);
            smoothedPosition.z = Mathf.Clamp(smoothedPosition.z, minCameraBoundsZ, maxCameraBoundsZ);
        }


        // 4. Update the camera's position
        transform.position = smoothedPosition;

        // Optionally clamp the camera position within bounds
        // desiredPosition.x = Mathf.Clamp(desiredPosition.x, minCameraBounds.x, maxCameraBounds.x);
        //desiredPosition.y = Mathf.Clamp(desiredPosition.y, minCameraBounds.y, maxCameraBounds.y);
        //transform.position = smoothedPosition;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enableFullBounds = true; // Set to false by default, can be enabled in the inspector
        enableYClamp = true; // Set to false by default, can be enabled in the inspector
        if (target != null)
        {
            transform.position = target.position + offset; // Initialize camera position at the start
            // if using full bounds, you can set the initial position within bounds
            if (enableFullBounds && enableYClamp)
            {
                // Clamp the initial position within the defined camera bounds
                // This ensures the camera starts within the defined bounds
                // If you want to use the min and max bounds for both X and Y, you can do it like this:
                Vector3 initialPosition = transform.position;
                initialPosition.x = Mathf.Clamp(initialPosition.x, minCameraBounds, maxCameraBounds);
                initialPosition.z = Mathf.Clamp(initialPosition.z, minCameraBoundsZ, maxCameraBoundsZ);
                initialPosition.y = Mathf.Clamp(initialPosition.y, minYClamp, maxYClamp);
                transform.position = initialPosition;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
