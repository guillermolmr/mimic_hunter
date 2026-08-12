using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager :MonoBehaviour
{
    public static GameFlowManager instance;
    public int maxStrikes;
    public int strikes;
    public int mimics;

    List<Mimic> listMimics = new List<Mimic>();


    [Header("DeathSequence")]
    public float timeLockMimic;
    public float timeWaitAnimation;
    public float timeToReachPlayer;

    [Header("References")]
    public GameObject BlackScreen;
    public GameObject DeathScreen;
    [SerializeField]
    HouseDecorator houseDecorator;
    [SerializeField]
    GameObject victoryScreen;
    [SerializeField]
    GameObject gameCompletedScreen
        ;
    [SerializeField]
    GameObject gameOverScreen;

    [SerializeField]
    TextMeshProUGUI mimicsCounter;
    [SerializeField]
    List<GameObject> strikesGO = new List<GameObject>();
    [SerializeField]
    GameObject RulesCreen;

    [Header("Prefabs")]
    public GameObject MimicPrefab;

    public float fillRoom = 0.33f;

    private void Awake()
    {
        instance = this;
        int level = DificultyManager.Instance.level;
        int numMimics = 1+level; 
        if (level>=3)
        {
            fillRoom = 0.5f;

        }
        houseDecorator.Init(fillRoom);
        SpawnMimics(numMimics);
        mimicsCounter.text = mimics.ToString();

    }


    void SpawnMimics(int num = 1)
    {

        mimics = 0;
        for (int i=0; i < num; i++)
        {
            GameObject mp = Instantiate(MimicPrefab);
            Mimic mimic = mp.GetComponent<Mimic>();
            mimic.InitMimic(num > 1);
            
            listMimics.Add(mimic);
            mimics++;
        }
        
    }
    public void ReportMimicDeath(Mimic mimic)
    {
        listMimics.Remove(mimic);
        mimics--;
        mimicsCounter.text = mimics.ToString() ;
        if (mimics <= 0)
        {
            LevelCompleted();
        }
        
        

    }

    public void ReportFailShot()
    {
        
        if (strikes < strikesGO.Count)
        {
            strikesGO[strikes].SetActive(true);

        }
        strikes++;
        if (strikes>=maxStrikes)
        {
            GameOver();
        }
        Mimic killerMimic = GetMimicInRoom();
        if (killerMimic != null)
        {
            MimicKillSequence(killerMimic);
        }
    }
    void MimicKillSequence(Mimic mimic)
    {
        PlayerController.instance.canMove = false;
        PlayerController.instance.canShoot = false;
        StartCoroutine(PlayerDeathSequence(mimic));
    }

    IEnumerator PlayerDeathSequence(Mimic mimic)
    {
        float cd = 0f;
        Transform cam = PlayerController.instance.CameraTransform;
        Transform mimicT = mimic.transform;
        Quaternion target = Quaternion.LookRotation(mimicT.position - cam.position);
        float itime = 1f/timeLockMimic;
        while (cd < 1f)
        {
            cd += Time.deltaTime*itime;

            cam.rotation = Quaternion.Lerp(cam.rotation,target,cd);

            yield return null;
        }
        cam.rotation = target;
        yield return null;

        //mimic Animation
        yield return mimic.Activate();

        /*
        cd = 0f;
        while (cd < timeWaitAnimation)
        {
            cd += Time.deltaTime ;
            yield return null;
        }
        yield return null;
        */
        yield return new WaitForSeconds(timeWaitAnimation);

        //End wait, now latch!
        cd = 0f;
        itime = 1f / timeToReachPlayer;
        Vector3 origin = mimicT.position;
        while (cd <1f)
        {
            yield return null;
            cd += itime * Time.deltaTime;
            mimicT.position = Vector3.Lerp(origin, cam.position, cd);
            
        }

        Debug.Log("DEAD");
        BlackScreen.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        DeathScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;


    }

    Mimic GetMimicInRoom()
    {
        foreach(Mimic mimic in listMimics)
        {
            if (mimic.room.hasPlayer)
                return mimic;
        }
        return null;
    }

    public void LevelCompleted()
    {
        Invoke(nameof(ShowVictoryScreen), 2f);
    }

    void ShowVictoryScreen()
    {
        PlayerController.instance.canMove = false;
        PlayerController.instance.canShoot = false;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (DificultyManager.Instance.level == 4)
        {
            gameCompletedScreen.SetActive(true);
            //show game completed screen
        }
        else
        {
            victoryScreen.SetActive(true);
            DificultyManager.Instance.level++;
        }
            
    }

    public void GameOver()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PlayerController.instance.canMove = false;
        PlayerController.instance.canShoot = false;

        gameOverScreen.SetActive(true);

    }

    public void RetryLevel()
    {
        SceneManager.LoadScene("HouseScene");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RetryLevel();
        }
        RulesCreen.SetActive(Input.GetKey(KeyCode.Tab));
    }

}
