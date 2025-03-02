using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float zoomSpeed = 15f;
    public float minZoom = 5f;
    public float maxZoom = 40f;

    private float fixedHeight;
    [SerializeField] private float lowAngle;
    [SerializeField] private float highAngle;

    void Start()
    {
        fixedHeight = transform.position.y;
    }

    void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveX, 0, moveZ) * (moveSpeed * Time.deltaTime);
        transform.position += movement;
        transform.position = new Vector3(transform.position.x, fixedHeight, transform.position.z);
    }

    void HandleZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            fixedHeight -= scrollInput * zoomSpeed;
            fixedHeight = Mathf.Clamp(fixedHeight, minZoom, maxZoom);
            transform.position = new Vector3(transform.position.x, fixedHeight, transform.position.z);
            if (fixedHeight < maxZoom && fixedHeight > minZoom)
            {
                
                Vector3 forwardMovement = transform.forward * (scrollInput * zoomSpeed);
                forwardMovement.y = 0;
                transform.position += forwardMovement;
            }
        }

        AdjustCameraAngle();
    }

    void AdjustCameraAngle()
    {
        float angle = Mathf.Lerp(lowAngle, highAngle, (fixedHeight - minZoom) / (maxZoom - minZoom));
        transform.rotation = Quaternion.Euler(angle, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }
}