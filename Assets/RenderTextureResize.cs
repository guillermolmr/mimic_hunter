using UnityEngine;

public class RenderTextureResize : MonoBehaviour
{
    Camera cam;
    [SerializeField]
    RenderTexture renderTexture;
    [SerializeField]
    float aspect;

    [SerializeField]
    bool realtime = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GetComponent<Camera>();
        aspect = (float)Screen.width / (float)Screen.height;
        cam.aspect = aspect;
    }

    // Update is called once per frame
    void Update()
    {
        if (realtime)
        {
            aspect = (float)Screen.width / (float)Screen.height;
            cam.aspect = aspect;
        }
        else
        {
            cam.aspect = aspect;
        }
        
    }
}
