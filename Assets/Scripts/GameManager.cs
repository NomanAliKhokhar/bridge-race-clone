using Cinemachine;
using Lean.Pool;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance => instance;
    private static GameManager instance;


    public Color NaturalColor => naturalColor;
    public Color PlayerColor => Colors[0];
    public Color RandomColor => Colors[Random.Range(0, randomContestantCount)];
    public Color RandomColorAIColors => Colors[Random.Range(1, randomContestantCount)];





    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private CanvasGroup GameOverUI;
    [SerializeField] private Player player;
    //[SerializeField] private BridgeController[] bridges;
    //[SerializeField] private BrickSpawner[] brickSpawners;
    [SerializeField] private int currentFloor;
    [SerializeField] private Floor[] floors;
    [SerializeField] private Color[] Colors;
    [SerializeField] private Color naturalColor;


    private List<ContestantAI> contestantList;
    private int randomContestantCount;
    private void Awake()
    {

        Debug.unityLogger.logEnabled = Debug.isDebugBuild;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
        //Screen.SetResolution(1080, 1920, false, 30);
        Input.simulateMouseWithTouches = false;


        contestantList = new List<ContestantAI>();
        instance = this;
        Debug.Log("Test");


    }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {

        Clear();



        vcam.Follow = player.transform;
        vcam.LookAt = player.transform;
        randomContestantCount = Random.Range(2, Colors.Length);

        player.Init(PlayerColor);

        UnlockNextFloor();


        for (int i = randomContestantCount - 1; i >= 1; i--)
        {
            ContestantAI contestant = LeanPool.Spawn(GameAssets.Instance.Contestant, Vector3.right * i, Quaternion.identity);
            contestant.Init(Colors[i]);
            contestantList.Add(contestant);
        }

    }

    private void Clear()
    {
        foreach (ContestantAI contestant in contestantList)
        {
            contestant.Despawn();
        }
        contestantList.Clear();
    }

    public void Restart()
    {
        Initialize();

        GameOverUI.alpha = 0;
        GameOverUI.blocksRaycasts = false;
        GameOverUI.interactable = false;
    }

    public void GameOver(Character winner)
    {
        foreach (ContestantAI contestant in contestantList)
        {
            contestant.Stop();
        }

        GameOverUI.alpha = 1.0f;
        GameOverUI.blocksRaycasts = true;
        GameOverUI.interactable = true;

        vcam.Follow = winner.transform;
        vcam.LookAt = winner.transform;

        player.Stop();


    }

    public void UnlockNextFloor()
    {
        if (currentFloor < floors.Length)
        {
            floors[currentFloor].Init();
            currentFloor++;
        }
    }


    public BridgeController FindBestBridge(Color characterColor, int CurrentFloor)
    {
        BridgeController bestBridge = floors[CurrentFloor].FindBestBridge(characterColor);
        return bestBridge;

    }
}