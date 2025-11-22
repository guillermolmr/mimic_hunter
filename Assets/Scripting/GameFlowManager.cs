using System.Collections;
using System.Collections.Generic;
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


    [Header("Prefabs")]
    public GameObject MimicPrefab;

    private void Awake()
    {
        instance = this;
        houseDecorator.Init();
        SpawnMimics();
    }


    void SpawnMimics(int num = 1)
    {
        for(int i=0; i < num; i++)
        {
            GameObject mp = Instantiate(MimicPrefab);
            Mimic mimic = mp.GetComponent<Mimic>();
            mimic.InitMimic();
            listMimics.Add(mimic);
        }
        
    }
    public void ReportMimicDeath(Mimic mimic)
    {

        listMimics.Remove(mimic);
        mimics--;
        if (mimics <= 0)
        {
            LevelCompleted();
        }
        
        

    }

    public void ReportFailShot()
    {
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
        mimic.Activate();

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
        victoryScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }

    public void GameOver()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }

    public void RetryLevel()
    {
        SceneManager.LoadScene("HouseScene");
    }

}
