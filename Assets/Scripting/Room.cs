
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Room : MonoBehaviour
{
    public RoomType roomType;
    public List<RoomConection> roomConections = new List<RoomConection>();

    public List<Spot> spots = new List<Spot>();
    BoxCollider boxCollider;
    public List<Spot> usedSpots;
    public List<Spot> freeSpots;

    public bool hasPlayerSeen = false;
    public bool hasPlayer = false;
    
    public bool isPlayerAdyacentRoom = false;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }


    public void Decorate(float fill = 0.5f)
    {
        int N = spots.Count;
        int half = (int)(N* fill); //this can be changed later to increase level dificulty
        Debug.Log("Decorating " + name);
        
        List<Spot> copy = new List<Spot>(spots);

        
        for (int i = 0; i < half; i++)
        {
            int r = Random.Range(i, N);

            Spot temp = copy[i];
            copy[i] = copy[r];
            copy[r] = temp;
        }
        usedSpots = copy.GetRange(0, half);
        freeSpots = copy.GetRange(half,half );

        List<GameObject>[] propsAvalible = HouseDecorator.instance.propSourced[(int)roomType];

        foreach (Spot spot in usedSpots)
        {
            List<GameObject> validSpotTypeProps = propsAvalible[(int)spot.spotType];
            int count = validSpotTypeProps.Count;
            if (count > 0)
            {
                GameObject randomProp = validSpotTypeProps[Random.Range(0, count)];
                GameObject go=Instantiate(randomProp, spot.transform.position, spot.transform.rotation);
                Furniture f = go.GetComponent<Furniture>();
                f.Init(this, spot, true);
                //Debug.Log("Spawn " + randomProp.name);
            }
            else
            {
                Debug.LogError("No props for " + roomType.ToString() + ": " + spot.spotType.ToString());
            }
            
        }

    }

    [Button]
    public void FindSpots()
    {
        boxCollider = GetComponent<BoxCollider>();
        Bounds bounds = boxCollider.bounds;
        if (boxCollider == null)
        {
            Debug.LogError("No BoxCollider found in Room");
            return;
        }
        spots.Clear();
        GameObject[] spotsGO = GameObject.FindGameObjectsWithTag("Spot");
        foreach(GameObject spotGO in spotsGO)
        {
            if(spotGO.name== "SpotPrueba")
            {
                Debug.Log("Entra!");
            }
            if (bounds.Contains(spotGO.transform.position))
            {
                Spot spot = spotGO.GetComponent<Spot>();

                
                if (spot != null)
                {
                    spots.Add(spot);
                    if (spotGO.transform.parent == null)
                    {
                        spotGO.transform.parent = transform;
                    }
                    else if(spotGO.transform.parent.parent == null)
                    {
                        spotGO.transform.parent.parent = transform;
                    }else if (spotGO.transform.parent==transform || spotGO.transform.parent.parent ==transform)
                    {
                        Debug.Log("Nothing?");
                    }
                    else
                    {
                        Debug.LogError("Make sure you unparent "+ spotGO.name);
                    }
                    
                }
                else
                {
                    Debug.LogError("No Spot behaviour on Spot GameObjetc");
                }
            }
        }
        EditorUtility.SetDirty(this);

    }
    public void UseSpot(Spot spot)
    {
        if (freeSpots.Contains(spot))
        {
            freeSpots.Remove(spot);
            usedSpots.Add(spot);
        }
        else
        {
            Debug.LogError("Trying to use a spot already used or of diferent room");
        }
    }
    public void FreeSpot(Spot spot)
    {
        if (usedSpots.Contains(spot))
        {
            usedSpots.Remove(spot);
            freeSpots.Add(spot);
        }
        else
        {
            Debug.LogError("Trying to free a spot already free or of diferent room");
        }
    }

    public Door GetDoor(Room room)
    {
        

        foreach (RoomConection rc in roomConections)
        {
            if (rc.room == room)
            {
                return rc.door;
            }
        }

        return null;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerEnter();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (hasPlayer)
            {
                HouseDecorator.instance.DetectRoomPlayer();
            }
        }
    }

    public void PlayerEnter()
    {
        hasPlayer = true;
        hasPlayerSeen = true;
        isPlayerAdyacentRoom = false;
        NotifyConnectedRoomsOfPlayerEnter();
    }

    void NotifyConnectedRoomsOfPlayerEnter()
    {
        foreach (RoomConection rc in roomConections)
        {
            rc.room.PlayerExited();
            rc.room.isPlayerAdyacentRoom = true;
        }
    }

    void NotifyConnectedRoomsOfPlayerExit()
    {
        foreach (RoomConection rc in roomConections)
        {
            rc.room.isPlayerAdyacentRoom = false;
        }
    }

    

    public void PlayerExited()
    {
        if (hasPlayer)
        {
            hasPlayer = false;
            NotifyConnectedRoomsOfPlayerExit();
        }
        
    }

    public bool IsAnyDoorOpened()
    {
        foreach (RoomConection rc in roomConections)
        {
            if (rc.door.isOpen)
                return true;
        }
        return false;
    }
    public RoomConection getConectedRoomWithoutPlayer(bool canOpen)
    {
        List<RoomConection> validRC = new List<RoomConection>();
        foreach (RoomConection rc in roomConections)
        {
            if (!canOpen)
            {
                if (rc.door.isOpen && !rc.room.hasPlayer)
                    validRC.Add(rc);
            }
            else
            {
                if (!rc.room.hasPlayer)
                    validRC.Add(rc);
            }
            
        }
        if(validRC.Count==0)
            return null;
        if (validRC.Count == 1)
            return validRC[0];
        return validRC[Random.Range(0, validRC.Count)];
    }

}
