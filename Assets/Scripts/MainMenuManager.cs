using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Spaceship")]
    public Transform spaceship;
    public float forwardSpeed = 5f;
    public float verticalRange = 0.5f;
    public float verticalChangeSpeed = 2f;

    private float targetY;
    private float verticalLerpTime = 0f;

    [Header("Camera Follow")]
    public Transform cam;
    public Vector3 camOffset = new Vector3(-10, 2, 0); 
    public float camSmoothTime = 0.2f;
    private Vector3 camVelocity = Vector3.zero;

    [Header("Color Settings")]
    public Material[] spaceshipColors;
    public Renderer spaceshipRenderer;
    public int materialIndex = 0;
    public float colorChangeInterval = 2f;
    private float colorTimer = 0f;

    [Header("UI Stuff")]
    public Button startButton;

    void Start()
    {
        targetY = spaceship.position.y;

        if (cam == null && Camera.main != null)
            cam = Camera.main.transform;

        ChangeColor();

        if (startButton != null)
            startButton.onClick.AddListener(StartGame);
    }

    void Update()
    {
        Vector3 pos = spaceship.position;
        pos.z += forwardSpeed * Time.deltaTime;

        verticalLerpTime += Time.deltaTime * verticalChangeSpeed;
        if (verticalLerpTime >= 1f)
        {
            verticalLerpTime = 0f;
            targetY = spaceship.position.y + Random.Range(-verticalRange, verticalRange);
        }
        pos.y = Mathf.Lerp(pos.y, targetY, Time.deltaTime * verticalChangeSpeed);
        spaceship.position = pos;

        Vector3 targetCamPos = new Vector3(camOffset.x, camOffset.y, spaceship.position.z) + camOffset;
        cam.position = Vector3.SmoothDamp(cam.position, targetCamPos, ref camVelocity, camSmoothTime);

        colorTimer += Time.deltaTime;
        if (colorTimer >= colorChangeInterval)
        {
            colorTimer = 0f;
            ChangeColor();
        }
    }

    void ChangeColor()
    {
        if (spaceshipColors.Length > 0 && spaceshipRenderer != null)
        {
            int newIndex = Random.Range(0, spaceshipColors.Length);
            Material[] mats = spaceshipRenderer.materials;
            if (materialIndex >= 0 && materialIndex < mats.Length)
            {
                mats[materialIndex] = spaceshipColors[newIndex];
                spaceshipRenderer.materials = mats;
            }
        }
    }

    void StartGame()
    {
        SceneManager.LoadScene("MainGame");
    }
}
