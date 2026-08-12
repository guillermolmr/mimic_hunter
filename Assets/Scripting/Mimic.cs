

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Mimic : MonoBehaviour
{

    /*
    Mimic rules:
        if room is closed and player is outside:
            each 7s can change to object in room
        else if door open and player not in any of conected rooms:
            mimic has 50% chance to move to next room when 14s
        else if player in room with mimic:
            if player shoot:
                if player hit mimic -> mimic dies
                else -> player dies
            

     */
    [SerializeField]
    List<AudioClip> sounds = new List<AudioClip>();

    Transform CameraTransform;
    [SerializeField]
    LayerMask EnemyLayer;
    [SerializeField]
    Transform eyeMouth;

    [SerializeField]
    float timeToSwapInRoom = 7f;
    [SerializeField]
    float probabilitySwapRoom = 0.5f;

    [SerializeField]
    float probabilityOpenDoor = 0.2f;
    [SerializeField]
    bool canOpenDoors = false;

    float cdSwapInRoom = 0f;

    bool initialized = false;

    GameObject mimicProp;

    public Room room{get; private set;}
    public Spot spot {get; private set;}

    AudioSource audioSource;

    [Header("Death")]
    
    public AudioClip deathSound;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void InitMimic(bool _canOpenDoors)
    {
        canOpenDoors = _canOpenDoors;
        List<Room> rooms = HouseDecorator.instance.rooms;
        room = rooms[Random.Range(0, rooms.Count)];
        try
        {
            spot = room.freeSpots[Random.Range(0, room.freeSpots.Count)];
        }catch(System.Exception e)
        {
            Debug.LogError(e.ToString());
            return;
        }

        GameObject prefab = HouseDecorator.instance.GetRandomProp(room.roomType, spot.spotType);
        if (prefab == null)
        {
            prefab = FastSwap();
        }
        transform.parent = spot.transform;
        transform.position = spot.transform.position;
        transform.rotation = spot.transform.rotation;

        SpawnPrefab(prefab);
        room.UseSpot(spot);
        initialized = true;

    }

    void SwapInRoom()
    {
        Spot oldSpot = spot;
        room.FreeSpot(spot);
        spot = room.freeSpots[Random.Range(0, room.freeSpots.Count)];
        Destroy(mimicProp);
        GameObject prefab = HouseDecorator.instance.GetRandomProp(room.roomType, spot.spotType);
        if (prefab == null)
        {
            prefab = FastSwap();
        }
        transform.parent = spot.transform;
        transform.position = spot.transform.position;
        transform.rotation = spot.transform.rotation;
        SpawnPrefab(prefab);
        room.UseSpot(spot);
        if (oldSpot != spot && !oldSpot.isSiblingOf(spot))
        {
            oldSpot.mimicSwapOut?.Invoke();
        }

    }



    void SwapRoom(Room targetRoom, bool openDoor)
    {
        Spot oldSpot = spot;
        room.FreeSpot(spot);
        Destroy(mimicProp);
        if (openDoor)
        {
            Door door = room.GetDoor(targetRoom);
            if (door == null)
            {
                Debug.LogError("GetDoor between " + room.name + " and " + targetRoom.name + "returned null");
                return;
            }
            if (!door.isOpen)
            {
                door.DoAnimation();
            }
        }

        room = targetRoom;
        spot = room.freeSpots[Random.Range(0, room.freeSpots.Count)];
        if (oldSpot != spot)
        {
            oldSpot.mimicSwapOut?.Invoke();
        }

        GameObject prefab = HouseDecorator.instance.GetRandomProp(room.roomType, spot.spotType);
        if (prefab == null)
        {
            prefab = FastSwap();
        }
        transform.parent = spot.transform;
        transform.position = spot.transform.position;
        transform.rotation = spot.transform.rotation;

        SpawnPrefab(prefab);
        room.UseSpot(spot);
#if UNITY_EDITOR
        Debug.Log("Mimic swaped to room: " + room.name);
#endif
    }

    GameObject FastSwap()
    {
        Debug.LogWarning("Had to use FastSwap()");

        for (int i = 0; i < room.freeSpots.Count; i++)
        {
            GameObject prefab = HouseDecorator.instance.GetRandomProp(room.roomType, room.freeSpots[i].spotType);
            if (prefab != null)
            {
                spot = room.freeSpots[i];
                return prefab;
            }
        }
        Debug.LogError("Couldn't find a prefab for any spot in room " + room.ToString());
        return null;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CameraTransform = PlayerController.instance.CameraTransform;

        cdSwapInRoom = timeToSwapInRoom;
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F))
        {
            PlayRandomSound();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            SwapInRoom();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            RoomConection rc = room.getConectedRoomWithoutPlayer(true);
            if (rc != null )
            {
                SwapRoom(rc.room, true);
                
            }
        }
#endif
        if (!initialized)
            return;

        if (room.hasPlayer || !room.hasPlayerSeen)
        {
            cdSwapInRoom = timeToSwapInRoom;
            return;
        }
        cdSwapInRoom -= Time.deltaTime;
        if (cdSwapInRoom > 0f)
            return;



        bool swaped = false;
        if (room.IsAnyDoorOpened())
        {


            if ((!room.isPlayerAdyacentRoom) && Random.Range(0, 1f) < probabilitySwapRoom)
            {
                RoomConection rc = room.getConectedRoomWithoutPlayer(false);

                if (rc != null && !rc.room.isPlayerAdyacentRoom)
                {
                    SwapRoom(rc.room, false);
                    swaped = true;
                }
            }




        }

        if (!swaped)
        {

            if (canOpenDoors && Random.Range(0, 1f) < probabilityOpenDoor)
            {
                RoomConection rc = room.getConectedRoomWithoutPlayer(true);
                if (rc != null && !rc.room.isPlayerAdyacentRoom)
                {
                    SwapRoom(rc.room, true);
                    swaped = true;
                }
            }
            else
            {
                RoomConection rc = room.getConectedRoomWithoutPlayer(false);

                if (rc == null || !rc.room.hasPlayer)
                {
                    SwapInRoom();
                    swaped = true;
                }
            }





        }
        if (swaped)
        {
            room.hasPlayerSeen = false;
            PlayRandomSound();
        }
        cdSwapInRoom = timeToSwapInRoom;


    }
    IEnumerator MoveTo(Vector3 v)
    {
        float timeToMoveOut = 0.5f;
        float itime = 1f / timeToMoveOut;
        float cd = 0f;

        Vector3 origin = transform.position;
        while (cd < 1f)
        {
            cd += Time.deltaTime * itime;
            transform.position = Vector3.Lerp(origin,v,cd);
            CameraTransform.rotation = Quaternion.LookRotation(transform.position - CameraTransform.position);
            yield return null;
        }


    }
    IEnumerator CheckRoute()
    {
        
        Vector3 origin = transform.position + new Vector3(0f,0.05f,0f);
        if (!Physics.Raycast(origin, Vector3.up, 0.4f))
        {
            yield return MoveTo(origin + Vector3.up * 0.4f);
        }
        else if (!Physics.Raycast(origin, Vector3.forward,0.4f))
        {
            yield return MoveTo(origin+Vector3.forward*0.4f);
        }
        else if(!Physics.Raycast(origin, -Vector3.forward, 0.4f))
        {
            yield return MoveTo(origin - Vector3.forward * 0.4f);
        }
        else if (!Physics.Raycast(origin, Vector3.right, 0.4f))
        {
            yield return MoveTo(origin + Vector3.right * 0.4f);
        }
        else if (!Physics.Raycast(origin, -Vector3.right, 0.4f))
        {
            yield return MoveTo(origin - Vector3.right * 0.4f);
        }
        
    }
    public IEnumerator Activate()
    {
        

        spot.mimicSwapOut?.Invoke();
        yield return new WaitForSeconds(0.5f);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, CameraTransform.position- transform.position, out hit) && !hit.collider.CompareTag("Player"))
        {
            yield return CheckRoute();
        }
        Vector3 dir = transform.position - CameraTransform.position;
        
        if (Physics.Raycast(CameraTransform.position, dir, out hit))
        {
            eyeMouth.position = hit.point;
            eyeMouth.forward = dir;
            eyeMouth.gameObject.SetActive(true);
        }
        yield return null;
    }
    void SpawnPrefab(GameObject prefab)
    {
        mimicProp = Instantiate(prefab, transform);

        //for testing
        /*
        int layer = gameObject.layer;
        var children = mimicProp.transform.GetComponentsInChildren<Transform>(includeInactive: true);
        foreach (var child in children)
        {
            child.gameObject.layer = layer;
        }
        */
        Furniture furniture = mimicProp.GetComponent<Furniture>();
        furniture.Init(room, spot, false);
        furniture.OnDestruction += Die;

    }

    void PlayRandomSound()
    {
        audioSource.resource = sounds[Random.Range(0, sounds.Count)];
        audioSource.Play();
    }

    void Die()
    {

        GameFlowManager.instance.ReportMimicDeath(this);
        gameObject.SetActive(false);
        room.FreeSpot(spot);
    }

}
