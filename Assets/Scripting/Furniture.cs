using System;
using UnityEngine;

public class Furniture : MonoBehaviour
{

    public event Action OnDestruction;

    [SerializeField]
    int layerTarget = 10;
    int layerDefault = 0;

    GameObject[] children;

    Spot spot;
    Room room;
    public bool isRealFurniture { get; private set; }

    public void Init(Room room, Spot spot,bool isRealFurniture)
    {
        this.room = room;
        this.spot = spot;
        this.isRealFurniture = isRealFurniture;
    }

    private void Start()
    {
        layerDefault = gameObject.layer;
        Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
        children = new GameObject[colliders.Length];
        for(int i = 0;i<colliders.Length;i++)
        {
            Collider col = colliders[i];
            children[i] = col.gameObject;
            col.tag = tag;
            FurnitureChildren fc=children[i].AddComponent<FurnitureChildren>();
            fc.parent = this;
        }
        

    }

    public void HighLight()
    {
        foreach(GameObject child in children)
        {
            child.layer = layerTarget;
        }
    }
    public void UnhighLight()
    {
        foreach (GameObject child in children)
        {
            child.layer = layerDefault;
        }
    }
    public void DestroyFurniture(bool destroy=true)
    {
        if (isRealFurniture)
        {
            room.FreeSpot(spot);
        }

        OnDestruction?.Invoke();
        if (destroy)
            Destroy(gameObject);
    }
}
