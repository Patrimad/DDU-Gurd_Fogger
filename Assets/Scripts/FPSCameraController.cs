using Photon.Pun;
using UnityEngine;

public class FPSCameraController : MonoBehaviour
{
    public float mouseSensivity = 5f;
    public Transform playerBody;
    public Transform headBone;
    public float minRotationAngle = -60f;
    public float maxRotationAngle = 60f;
    
    float xRotation = 0f;

    PhotonView PV;

    void Awake()
    {
        PV = GetComponentInParent<PhotonView>();
    }

    void Start()
    {
        if (!PV.IsMine)
        {
            enabled = false; // sluk hele scriptet på remote players
            return;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        
        float mouseX = Input.GetAxis("Mouse X") * mouseSensivity * 100 *  Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensivity * 100 * Time.deltaTime;
        float mouseScrollWheel = Input.GetAxis("Mouse ScrollWheel");
        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minRotationAngle, maxRotationAngle);
        
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); 
        
        playerBody.Rotate(Vector3.up * mouseX);
        
        transform.position = headBone.position;
    }
}