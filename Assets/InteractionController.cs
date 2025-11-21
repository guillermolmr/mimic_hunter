using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [SerializeField]
    KeyCode interactionKey = KeyCode.E;

    [SerializeField]
    float distance = 1f;
    [SerializeField]
    float radius = 0.2f;
    [SerializeField]
    Transform cam;
    [SerializeField]
    LayerMask InteractableLayerMask;


    RaycastHit hit;

    Interactable selectedInteractable;
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Selector())
        {
            if (Input.GetKeyDown(interactionKey))
            {
                selectedInteractable.DoAction();
            }
        }
    }

    bool Selector()
    {
        
        if(Physics.SphereCast(cam.position,radius,cam.forward,out hit, distance,InteractableLayerMask))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                selectedInteractable = hit.collider.GetComponent<Interactable>();
                selectedInteractable.Select();
                return true;
            }
            

            
        }

        if (selectedInteractable != null)
        {
            selectedInteractable.Unselect();
            selectedInteractable = null;
        }
        return false;
    }

}
