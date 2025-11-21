#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "Prop", menuName = "Scriptable Objects/Prop")]
public class Prop : ScriptableObject
{
    public GameObject prop;

    [EnumMask]
    public RoomType roomType;
    [EnumMask]
    public SpotType spotType;





}
