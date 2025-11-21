using UnityEngine;


public enum RoomType
{
    
    Living,
    Dining,
    Bathroom,
    Kitchen,
    Storage,
    Bedroom,
    Office,
    Count

}

public enum SpotType
{
    
    Floor,
    Table,
    Shelf,
    Wall,
    Ceiling,
    Sitting,
    TVLike,
    Count
}

public class EnumMaskAttribute : PropertyAttribute { }
