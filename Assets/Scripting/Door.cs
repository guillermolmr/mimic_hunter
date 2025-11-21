using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{

    public bool isOpen { get; private set; }
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

    public void DoAnimation()
    {

        if (!canAnimate)
            return;
        canAnimate = false;
        if (isOpen)
        {
            StartCoroutine(AnimateDoor(angle, closedAngle));
            isOpen = false;
        }
        else
        {
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
    }

    
}
