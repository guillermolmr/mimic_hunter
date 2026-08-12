using UnityEngine;

public class DificultyManager
{
    private static DificultyManager _instance;

    public static DificultyManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DificultyManager();
            }
            return _instance;
        }
    }

    public int level;
    public float mouseSensitivity;


}
