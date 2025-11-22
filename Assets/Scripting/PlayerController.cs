using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public static PlayerController instance;

    //input
    private float moveInputX;
    private float moveInputY;
    private float mouseX;
    private float mouseY;
    //references
    CharacterController controller;
    public Transform cameraHolderTransform;
    public Transform CameraTransform;
    //movement
    public float speed=5f;
    public float mouseSensitivity=1f;
    public float downSpeed = 1f;
    //data
    private float eulerX;
    private float eulerY;

    bool moveCamera;
    public bool canMove=true;
    public bool canShoot=true;
    private void Awake()
    {
        instance = this;
        controller = GetComponent<CharacterController>();
        StartCoroutine(FixStartCamera());
    }

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        moveCamera = false;
    }
    IEnumerator FixStartCamera()
    {
        yield return null;
        yield return null;
        yield return null;

        moveCamera = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (!canMove)
            return;
        InputManager();
        if (moveCamera)
            CameraRotation();
        Movement();
    }



    void InputManager()
    {
        moveInputX = Input.GetAxis("Horizontal");
        moveInputY = Input.GetAxis("Vertical");

        mouseX = Input.mousePositionDelta.x;
        mouseY = Input.mousePositionDelta.y;
        
    }
    void CameraRotation()
    {
        eulerX -= mouseY* mouseSensitivity;

        eulerX = Mathf.Clamp(eulerX, -90f, 90f);

        eulerY += mouseX* mouseSensitivity;

        cameraHolderTransform.localEulerAngles = new Vector3(0f, eulerY, 0f);
        CameraTransform.localEulerAngles = new Vector3(eulerX, 0f , 0f);
    }
    void Movement()
    {

        Vector3 move = cameraHolderTransform.right * moveInputX + cameraHolderTransform.forward * moveInputY;



        move *= speed*Time.deltaTime;
        if (!controller.isGrounded)
        {
            move.y = -downSpeed * Time.deltaTime;
        }
        
        controller.Move(move);
    }
}
