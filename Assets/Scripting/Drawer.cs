using System.Collections;
using UnityEngine;

public class Drawer : MonoBehaviour
{
    [SerializeField]
    float timeOpenClose;
    
    public bool isOpen { get; private set; }

    [SerializeField]
    Transform openPosition;
    [SerializeField]
    AudioClip OpenAudio;
    [SerializeField]
    AudioClip CloseAudio;
    AudioSource audioSource;

    bool isAnimating;
    Vector3 startPosition;

    Transform t;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        t = transform;

    }
    private void Start()
    {
        startPosition=transform.position;
    }

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
        if (isAnimating)
            return;
        isAnimating = true;
        audioSource.resource = isOpen ? CloseAudio:OpenAudio;
        audioSource.Play();
        StartCoroutine(AnimationSequence());

    }

    IEnumerator AnimationSequence()
    {
        float itime = 1f / timeOpenClose;
        float cd = 0f;

        Vector3 origin = !isOpen ? startPosition : openPosition.position;
        Vector3 target = isOpen ? startPosition : openPosition.position;
        while (cd < 1f)
        {
            cd += itime * Time.deltaTime;


            t.position = Vector3.Lerp(origin, target, cd);
            
            yield return null;


        }
        
        t.position = target;

        isOpen = !isOpen;
        isAnimating = false;
    }


}
