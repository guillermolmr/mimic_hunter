
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spot : MonoBehaviour
{
    public SpotType spotType;

    public UnityEvent mimicSwapOut;
    public UnityEvent mimicSwapIn;

    public List<Transform> siblingSpots=new List<Transform>();

    public bool isSiblingOf(Spot spot)
    {
        Transform ts = spot.transform;
        foreach (Transform sibling in siblingSpots)
        {
            if (ts == sibling)
                return true;
        }
        return false;
    }
}
