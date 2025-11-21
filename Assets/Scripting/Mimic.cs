

using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

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

    Room room;
    Spot spot;

    AudioSource audioSource;

    [Header("Death")]
    
    public AudioClip deathSound;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void InitMimic()
    {
        List<Room> rooms = HouseDecorator.instance.rooms;
        room = rooms[Random.Range(0, rooms.Count)];

        spot = room.freeSpots[Random.Range(0, room.freeSpots.Count)];


        GameObject prefab = HouseDecorator.instance.GetRandomProp(room.roomType, spot.spotType);
        if (prefab == null)
        {
            prefab = FastSwap();
        }

        transform.position = spot.transform.position;
        transform.rotation = spot.transform.rotation;

        SpawnPrefab(prefab);
        room.UseSpot(spot);
        initialized = true;

    }

    void SwapInRoom()
    {
        room.FreeSpot(spot);
        spot = room.freeSpots[Random.Range(0, room.freeSpots.Count)];
        Destroy(mimicProp);
        GameObject prefab = HouseDecorator.instance.GetRandomProp(room.roomType, spot.spotType);
        if (prefab == null)
        {
            prefab = FastSwap();
        }
        transform.position = spot.transform.position;
        transform.rotation = spot.transform.rotation;

        SpawnPrefab(prefab);
        room.UseSpot(spot);


    }



    void SwapRoom(Room targetRoom, bool openDoor)
    {
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

        GameObject prefab = HouseDecorator.instance.GetRandomProp(room.roomType, spot.spotType);
        if (prefab == null)
        {
            prefab = FastSwap();
        }

        transform.position = spot.transform.position;
        transform.rotation = spot.transform.rotation;

        SpawnPrefab(prefab);
        room.UseSpot(spot);

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


    public void Activate()
    {
        Vector3 dir = transform.position - CameraTransform.position;
        RaycastHit hit;
        if (Physics.Raycast(CameraTransform.position, dir, out hit))
        {
            eyeMouth.position = hit.point;
            eyeMouth.forward = dir;
            eyeMouth.gameObject.SetActive(true);
        }
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
        
        gameObject.SetActive(false);
        room.FreeSpot(spot);
    }

}
