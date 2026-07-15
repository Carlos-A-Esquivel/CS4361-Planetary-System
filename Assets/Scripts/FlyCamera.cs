using UnityEngine;

public class FlyCamera : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float sprintMultiplier = 3f;
    [SerializeField] private float lookSensitivity = 2f;

    private float pitch = 0f;
    private float yaw = 0f;

    void Start()
    {
        // Initialize rotation from the camera's current orientation
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        // Lock and hide the cursor for mouse-look
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleLook();
        HandleMovement();

        // Press Escape to free the cursor (so you can click away)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        yaw += mouseX;
        pitch -= mouseY; // subtract so moving mouse up looks up
        pitch = Mathf.Clamp(pitch, -89f, 89f); // stop over-rotation

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    void HandleMovement()
    {
        float speed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
            speed *= sprintMultiplier;

        float x = Input.GetAxis("Horizontal"); // A/D
        float z = Input.GetAxis("Vertical");   // W/S

        // Vertical movement with E (up) and Q (down)
        float y = 0f;
        if (Input.GetKey(KeyCode.E)) y += 1f;
        if (Input.GetKey(KeyCode.Q)) y -= 1f;

        Vector3 move = new Vector3(x, y, z) * speed * Time.deltaTime;
        transform.Translate(move, Space.Self);
    }
}