using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class ShootingManager : MonoBehaviour
{

    [SerializeField]
    float speedBullet = 10f;
    [SerializeField]
    float timeToLoadShot=1f;
    [SerializeField]
    float pitchUp = 2;
    [SerializeField]
    float pitchDown = 2;
    
    float cdLoadShot;
    

    [SerializeField]
    Transform muzzle;
    [SerializeField]
    Transform camTransform;
    [SerializeField]
    LineRenderer lineRenderer;
    Transform lineRendererTransform;
    [SerializeField]
    GameObject bullet;



    [SerializeField]
    Image loadingUI;

    AudioSource audioSource;
    [SerializeField]
    AudioSource audioSourceShoot;
    [SerializeField]
    AudioSource audioSourceExplosion;
    [SerializeField]
    ParticleSystem explosionParticles;
    RaycastHit hit;
    
    [SerializeField]
    GameObject ashDecalPrefab;
    [SerializeField]
    GameObject bloodSplash;
    float beamWidth;
    float itime;

    bool isShooting = false;
    bool isHitting = false;

    PlayerController playerController;
    Furniture targetFurniture;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        beamWidth = lineRenderer.startWidth;
        itime = 1f / timeToLoadShot;
        lineRendererTransform = lineRenderer.transform;
        playerController = GetComponent<PlayerController>();
        targetFurniture = null;
    }

    
    void Start()
    {
        
    }

    void TargetHighlight()
    {
        isHitting = Physics.Raycast(camTransform.position, camTransform.forward, out hit, 100f);

        if (isHitting)
        {
            if (hit.collider.CompareTag("Prop"))
            {
                FurnitureChildren furnitureChildren = hit.collider.GetComponent<FurnitureChildren>();
                Furniture f = furnitureChildren.parent;

                
                if (targetFurniture != null)
                {
                    if (f != targetFurniture)
                    {
                        targetFurniture.UnhighLight();
                        targetFurniture = f;
                        targetFurniture.HighLight();
                        DebugCanvas.instance.text.text = targetFurniture.name;

                    }

                }
                else
                {
                    targetFurniture = f;
                    targetFurniture.HighLight();
                    DebugCanvas.instance.text.text = targetFurniture.name;
                }
                

            }
            else if (targetFurniture != null)
            {
                targetFurniture.UnhighLight();
                targetFurniture = null;
            }


        }
        else
        {
            if (targetFurniture != null)
            {
                targetFurniture.UnhighLight();
            }
        }
    }

    void Update()
    {
        if (!playerController.canShoot)
            return;
        TargetHighlight();
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            audioSource.loop = true;
            audioSource.pitch = pitchUp;
            audioSource.Play();
            audioSource.loop = false;
            //lineRenderer.enabled = true;

        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {

            if (timeToLoadShot <= cdLoadShot)
            {

                Shoot(muzzle.position, hit.point);
            }
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if(cdLoadShot < timeToLoadShot)
                cdLoadShot += Time.deltaTime;
            
        }
        else
        {
            audioSource.pitch = -pitchDown;
            if (0f< cdLoadShot && !isShooting)
            {
                cdLoadShot -= (Time.deltaTime + Time.deltaTime);
            }
        }
        if (isShooting)
            loadingUI.fillAmount = 0f;
        else
            loadingUI.fillAmount = itime * cdLoadShot;


        //lineRenderer.SetPosition(0, lineRendererTransform.position);
        //lineRenderer.SetPosition(1, bullet.transform.position);
        

        if (timeToLoadShot > 0f)
        {
            //Physics.Raycast(camTransform.position, camTransform.forward, out hit, 100f);

            
            if (isHitting)
            {
                lineRenderer.SetPosition(0, lineRendererTransform.position);
                lineRenderer.SetPosition(1, hit.point);
            }
            else
            {
                lineRenderer.SetPosition(0, lineRendererTransform.position);
                lineRenderer.SetPosition(1, camTransform.position+ camTransform.forward*100f);
            }

            lineRenderer.startWidth = itime * cdLoadShot * beamWidth;
            
        }
        else
        {
            
            lineRenderer.startWidth = 0f;
        }
        
        
        
    }


    void Shoot(Vector3 origin,Vector3 target)
    {
        audioSource.Stop();
        audioSourceShoot.Play();
        isShooting = true;
        //bullet.transform.parent = null;
        bullet.SetActive(true);
        bullet.transform.position = muzzle.position;
        bullet.transform.rotation = Quaternion.LookRotation(target-origin);

        playerController.canMove = false;

        StartCoroutine(ShootAnimation(origin, target));


    }

    IEnumerator ShootAnimation(Vector3 origin, Vector3 target)
    {
        
        float distance = Vector3.Distance(origin, target);
        float totalTime = distance / speedBullet;
        

        float itime2 = 1f / totalTime;
        Debug.Log("distance: " + distance);
        Debug.Log("speedBullet: " + speedBullet);
        Debug.Log("itime2: " + itime2);
        Debug.Log("totalTime: " + totalTime);
        float cd = 0f;
        cdLoadShot = 1f - cd;
        Transform bulletT = bullet.transform;
        while (cd<1f)
        {
            cd += Time.deltaTime*itime2;
            bulletT.position = Vector3.Lerp(origin, target, cd);
            
            yield return null;
        }

        bulletT.position = target;
        yield return null;

        cdLoadShot = 0f;
        playerController.canMove = true;
        isShooting = false;
        bullet.SetActive(false);
        yield return null;
        audioSourceExplosion.Play();
        explosionParticles.transform.position = target;
        explosionParticles.Play();

        Instantiate(ashDecalPrefab, target, camTransform.rotation);
        if (targetFurniture!=null)
        {
            if (!targetFurniture.isRealFurniture)
            {
                Instantiate(bloodSplash, target, camTransform.rotation);
            }
            else
            {
                GameFlowManager.instance.ReportFailShot();
            }
            targetFurniture.DestroyFurniture();
        }

        

        //bulletT.position = muzzle.position;

    }
}
