using System.Collections;
using TMPro;

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

    [SerializeField]
    TextMeshProUGUI sensibilityValue;
    float alpha = 0f;
    private void Awake()
    {
        instance = this;
        controller = GetComponent<CharacterController>();
        StartCoroutine(FixStartCamera());
        if (DificultyManager.Instance.mouseSensitivity != 0f)
        {
            mouseSensitivity = DificultyManager.Instance.mouseSensitivity;
        }
        
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
        
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if(scroll>0.01f) // forward
        {
            alpha = 1f;
            mouseSensitivity += 0.025f;
            DificultyManager.Instance.mouseSensitivity = mouseSensitivity;

        }
        if (scroll<-0.01) // backwards
        {
            alpha = 1f;
            mouseSensitivity -= 0.025f;
            DificultyManager.Instance.mouseSensitivity=mouseSensitivity ;
        }
        mouseSensitivity = Mathf.Clamp(mouseSensitivity, 0.01f, 2f);
        sensibilityValue.text = Mathf.RoundToInt(mouseSensitivity*100f).ToString();
        if (alpha > 0f)
        {
            alpha -= Time.deltaTime;
        }
        Color color = sensibilityValue.color;
        color.a = alpha;
        sensibilityValue.color = color;
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
