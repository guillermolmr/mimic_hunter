using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{

    public bool isOpen;
    bool canAnimate = true;
    [SerializeField]
    float openAngle = 90;
    [SerializeField]
    float closedAngle = 0;

    

    [SerializeField]
    Transform hinge;

    [SerializeField]
    float timeAnimation = 0.5f;
    [SerializeField]
    bool CalculateOpenDirection;

    float angle = 0f;

    Transform playerTransform;

    
    public Transform OtherDoor;

    public AudioClip OpenDoorAudio;
    public AudioClip CloseDoorAudio;

    AudioSource audioSource;

    public void Open()
    {
        if (!isOpen)
        {
            DoAnimation();
        }
    }
    public void Close()
    {
        if (isOpen)
        {
            DoAnimation();
        }
    }
    public void DoAnimation()
    {

        if (!canAnimate)
            return;
        canAnimate = false;
        if (isOpen)
        {
            audioSource.resource = CloseDoorAudio;
            StartCoroutine(AnimateDoor(angle, closedAngle));
            isOpen = false;
        }
        else
        {
            audioSource.resource = OpenDoorAudio;
            Vector3 dirPlayer = playerTransform.position - transform.position;
            float dot = Vector3.Dot(hinge.forward, dirPlayer);

            if (CalculateOpenDirection)
            {
                
                StartCoroutine(AnimateDoor(closedAngle, dot > 0f ? openAngle : -openAngle));
            }
            else
            {
                
                StartCoroutine(AnimateDoor(closedAngle, openAngle));
            }
            
            
            isOpen = true;
        }
        audioSource?.Play();

    }

    IEnumerator AnimateDoor(float startAngle, float endAngle)
    {
        
        float cd = 0f;
        float itime = 1f / timeAnimation;
        Vector3 euler = hinge.localEulerAngles;
        while (cd < 1f)
        {
            cd += itime * Time.deltaTime;
            angle = Mathf.Lerp(startAngle, endAngle, cd);
            euler.y = angle;
            hinge.localEulerAngles = euler;
            if(OtherDoor!=null)
                OtherDoor.localEulerAngles = -euler;
            yield return null;
        }
        euler.y = endAngle;
        hinge.localEulerAngles = euler;
        if (OtherDoor != null)
            OtherDoor.localEulerAngles = -euler;
        canAnimate = true;
    }

    void Start()
    {
        playerTransform = PlayerController.instance.transform;
        canAnimate = true;
        audioSource = GetComponent<AudioSource>();
    }

    
}
