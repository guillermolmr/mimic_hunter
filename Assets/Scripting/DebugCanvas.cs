using TMPro;
using UnityEngine;

public class DebugCanvas : MonoBehaviour
{
    public static DebugCanvas instance;
    public TextMeshProUGUI text;

    


    private void Awake()
    {
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        string str = "";
        foreach(Room room in HouseDecorator.instance.rooms)
        {
            str += room.name + ": \t\thasPlayer(" + room.hasPlayer.ToString() + ")\t playerAdyacent(" + room.isPlayerAdyacentRoom + ") playerSeen("+room.hasPlayerSeen+")\n";
        }

        text.text = str;
    }
}
