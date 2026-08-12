using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HouseDecorator : MonoBehaviour
{

    public static HouseDecorator instance;
    public List<Prop> props = new List<Prop>();

    public List<Room> rooms = new List<Room>();

    
    public List<GameObject>[][] propSourced;

    
    
    
    private void Awake()
    {
        instance = this;
        
    }
    


    public void Init(float fillRoom)
    {
        if (instance == null)
            instance = this;
        int roomTypeCount = (int)RoomType.Count;
        int spotTypeCount = (int)SpotType.Count;
        propSourced = new List<GameObject>[roomTypeCount][];
        for (int i = 0; i < (int)RoomType.Count; i++)
        {
            propSourced[i] = new List<GameObject>[spotTypeCount];
            for (int j = 0; j < spotTypeCount; j++)
            {
                propSourced[i][j] = new List<GameObject>();
                foreach (Prop prop in props)
                {
                    if (
                        ((int)prop.roomType & (1 << i)) > 0 &&
                        ((int)prop.spotType & (1 << j)) > 0
                        )
                    {

                        propSourced[i][j].Add(prop.prop);
                    }
                }
                //Debug.Log( ((RoomType)i).ToString() + ", " + ((SpotType)j).ToString() + ": " + propSourced[i][j].Count);
            }

        }

        DecorateRooms(fillRoom);
    }
    void DecorateRooms(float fillRoom)
    {
        

        foreach(Room room in rooms)
        {
            room.Decorate(fillRoom);
        }

        
    }

    public GameObject GetRandomProp(RoomType roomType, SpotType spotType)
    {
        List<GameObject> list = propSourced[(int)roomType][(int)spotType];
        if (list.Count==0)
        {
            Debug.LogError("There is no prop for spot '" + spotType.ToString() + "' in room '" + roomType.ToString() + "'");
            return null;
        }
        return list[Random.Range(0, list.Count)];



    }
    public void DetectRoomPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found???");
            return;
        }

        Vector3 playerPos = player.transform.position;
        Room roomplayer = null;
        Room oldRoom = null;
        foreach (Room room in rooms)
        {
            if (room.hasPlayer)
            {
                oldRoom = room;
            }
            //room.hasPlayer = false;
            //room.isPlayerAdyacentRoom = false;
            BoxCollider boxCollider = room.GetComponent<BoxCollider>();
            if (boxCollider == null)
            {
                Debug.LogError("Room "+room.name+" has no box collider");
                return;
            }
            if (boxCollider.bounds.Contains(playerPos))
            {
                roomplayer = room;
            }

            
        }
        if (roomplayer == null)
        {
            //Debug.LogError("Player not found in any room");
            return;
        }
        roomplayer.PlayerEnter();
    }
}
