using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float laneWidth = 2f;
    public float moveSpeed = 10f;
    private int currentLane = 1;

    public Material[] colors; // 0=Red, 1=Green, 2=Blue
    private Renderer rend;
    private int currentColorIndex = 0;

    [Header("Camera Follow")]
    public Transform cam;
    public Vector3 camOffset = new Vector3(0, 5, -10);
    public float camSmoothTime = 0.2f;
    private Vector3 camVelocity = Vector3.zero;

    [Header("Platform Check")]
    public LayerMask laneLayer;
    public float checkDistance = 2f;
    public float checkInterval = 0.25f;
    public float platformGracePeriod = 0.5f;

    [Header("Tilt Settings")]
    public Transform modelTiltContainer; 
    public float tiltAngle = 25f;
    public float tiltSmoothTime = 0.1f;
    private float targetTilt = 0f;
    private float currentTilt = 0f;
    private float tiltVelocity = 0f;

    private float checkTimer = 0f;
    private float graceTimer = 0f;
    private int lastPlatformID = -1;

    private float colorChangeGraceTimer = 0f;
    public float colorChangeGracePeriod = 0.2f;

    public GameObject modelChild;
    public int bodyMaterialIndex = 1;
    private Renderer modelRenderer;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (modelChild != null)
            modelRenderer = modelChild.GetComponent<Renderer>();

        if (cam == null && Camera.main != null)
            cam = Camera.main.transform;

        SetColor(0);
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) && currentLane > 0)
        {
            currentLane--;
            targetTilt = tiltAngle;
        }
        if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) && currentLane < 2)
        {
            currentLane++;
            targetTilt = -tiltAngle;
        }

        Vector3 targetPosition = new Vector3((currentLane - 1) * laneWidth, transform.position.y, transform.position.z + moveSpeed * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.2f);

        if (Input.GetKeyDown(KeyCode.Alpha1)) SetColor(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetColor(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetColor(2);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            int nextColor = (currentColorIndex + 1) % colors.Length;
            SetColor(nextColor);
        }
        else if (scroll < 0f)
        {
            int prevColor = (currentColorIndex - 1 + colors.Length) % colors.Length;
            SetColor(prevColor);
        }

        checkTimer += Time.deltaTime;
        if (checkTimer >= checkInterval)
        {
            checkTimer = 0f;
            CheckPlatformColor();
        }

        if (graceTimer > 0f)
        {
            graceTimer -= Time.deltaTime;
        }

        if (colorChangeGraceTimer > 0f)
        {
            colorChangeGraceTimer -= Time.deltaTime;
        }

        targetTilt = Mathf.Lerp(targetTilt, 0f, Time.deltaTime * 5f);
        currentTilt = Mathf.SmoothDamp(currentTilt, targetTilt, ref tiltVelocity, tiltSmoothTime);

        if (modelTiltContainer != null)
        {
            modelTiltContainer.localRotation = Quaternion.Euler(0f, 0f, currentTilt);
        }
    }

    void LateUpdate()
    {
        if (cam != null)
        {
            Vector3 targetCamPos = transform.position + camOffset;
            cam.position = Vector3.SmoothDamp(cam.position, targetCamPos, ref camVelocity, camSmoothTime);
        }
    }

    void SetColor(int index)
    {
        if (currentColorIndex != index)
        {
            currentColorIndex = index;
            colorChangeGraceTimer = colorChangeGracePeriod;

            if (colors.Length > index && modelRenderer != null)
            {
                Material[] mats = modelRenderer.materials;
                if (bodyMaterialIndex >= 0 && bodyMaterialIndex < mats.Length)
                {
                    mats[bodyMaterialIndex] = colors[index];
                    modelRenderer.materials = mats;
                }
            }
        }
    }

    void CheckPlatformColor()
    {
        if (graceTimer > 0f || colorChangeGraceTimer > 0f)
        {
            return; 
        }

        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, Vector3.down);
        Debug.DrawRay(ray.origin, ray.direction * checkDistance, Color.red, 0.2f);

        if (Physics.Raycast(ray, out RaycastHit hit, checkDistance, laneLayer))
        {
            Obstacle platform = hit.collider.GetComponent<Obstacle>();
            if (platform != null)
            {
                int platformID = platform.GetInstanceID();
                if (platformID != lastPlatformID)
                {
                    graceTimer = platformGracePeriod;
                    lastPlatformID = platformID;
                    return;
                }

                if (platform.colorIndex == 3)
                {
                    Debug.Log("Neutral platform. No color check needed.");
                }
                else if (platform.colorIndex != currentColorIndex)
                {
                    Debug.Log("Game Over: Wrong color!");
                    enabled = false;
                }
                else
                {
                    Debug.Log("Correct color: " + platform.colorIndex);
                }
            }
        }
    }
}
