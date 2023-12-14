using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Reactor : MonoBehaviour
{
    public static Reactor Instance { get; private set; }

    [SerializeField] private bool gameStarted = false;
    private bool gameOver = false;
    private bool paused = false;
    [SerializeField] private float totalTime;
    [SerializeField] private float bonusTime;
    [SerializeField] private Image progressBar;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private GameObject[] dropPoints;
    private EnergyCellSO.CellType[] acceptedCellTypes;
    private GameObject heldCell;

    [SerializeField] private Color blueBar;
    [SerializeField][Range(0, 100)] private int yellowThreshold;
    [SerializeField]private Color yellowBar;
    [SerializeField][Range(0, 100)] private int orangeThreshold;
    [SerializeField] private Color orangeBar;
    [SerializeField][Range(0, 100)] private int redThreshold;
    [SerializeField] private Color redBar;
    [SerializeField] private TextMeshProUGUI countdown;
    [SerializeField] private TextMeshProUGUI timeScore;
    [SerializeField] private TextMeshProUGUI timeFinalScore;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private GameObject instructions;

    public event EventHandler OnDoorsActivated;
    public event EventHandler OnConsumedCell;
    public event EventHandler OnGameOver;

    private float gameTime = 0;
    private float currentTime;
    private bool consuming = false;
    private bool consumed = false;
    private float consumingTime = 2f;
    private bool crossedIn;

    private void Awake()
    {
        Instance = this;
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(() =>
        {
            SceneData.nextScene = "HomeScene";
            SceneManager.LoadScene("LoadingScene");
        });
    }
    // Start is called before the first frame update
    void Start()
    {
        PauseGame();
        pauseMenu.GetComponentInChildren<TextMeshProUGUI>().enabled = false;

        acceptedCellTypes = new EnergyCellSO.CellType[4];
        currentTime = totalTime;

        SetDropPoint(0);
        SetDropPoint(1);
        SetDropPoint(2);
        SetDropPoint(3);

        PlayerInput.Instance.OnInteractPerformed += Instance_OnInteractPerformed;
        PlayerInput.Instance.OnPausePerformed += Instance_OnPausePerformed;

        gameStarted = true;
    }

    private void Instance_OnPausePerformed(object sender, System.EventArgs e)
    {
        PauseGame();
    }

    private void Instance_OnInteractPerformed(object sender, System.EventArgs e)
    {
        for (int i = 0; i < dropPoints.Length; i++)
        {
            if (PlayerInput.Instance.heldCell != null &&
                heldCell == null &&
                Vector3.Distance(PlayerInput.Instance.heldCell.transform.position, dropPoints[i].transform.position) < 0.65f &&
                PlayerInput.Instance.heldCell.GetComponent<EnergyCell>().energyCellSO.label == acceptedCellTypes[i] &&
                !consuming
                )
            {
                //Accepted Cell
                PlayerInput.Instance.heldCell.GetComponent<EnergyCell>().parent = dropPoints[i];
                PlayerInput.Instance.heldCell.transform.SetParent(dropPoints[i].transform);
                PlayerInput.Instance.heldCell.transform.localPosition = Vector3.zero;
                heldCell = PlayerInput.Instance.heldCell;
                PlayerInput.Instance.heldCell = null;
                consuming = true;
                doorAnimator.SetBool("doorsOpen", false);
                OnDoorsActivated?.Invoke(this, EventArgs.Empty);
                SetDropPoint(i);
            }
        }
    }



    // Update is called once per frame
    void Update()
    {
        if (gameStarted)
        {
            UpdateTimer();
            ManageDoors();
        }

        if(gameOver)
        {
            Time.timeScale = 0;
            timeFinalScore.text = "Final Time\n" + ((int)(gameTime / 60f)).ToString() + "'  " + (gameTime % 60).ToString("00") + "\"  " + gameTime.ToString("0.00").Split('.')[1];
            if(gameStarted)
            {
                OnGameOver?.Invoke(this, EventArgs.Empty);
            }
            gameStarted = false;
            gameOverMenu.SetActive(true);
            mainMenuButton.gameObject.SetActive(true);

        }
    }

    private void UpdateTimer()
    {
        gameTime += Time.deltaTime;
        currentTime -= Time.deltaTime;
        if (currentTime < 0 )
        {
            currentTime = 0;
            gameOver = true;
        }
        if (currentTime > totalTime)
        {
            currentTime = totalTime;
        }

        //update UI
        timeScore.text = "Total  Time:  "+((int)(gameTime/60f)).ToString()+"'  "+(gameTime%60).ToString("00")+"\"  "+gameTime.ToString("0.00").Split('.')[1];
        progressBar.fillAmount =  currentTime/totalTime;
        countdown.enabled = false;
        if (progressBar.fillAmount > yellowThreshold / 100f)
        {
            progressBar.color = blueBar;
        }
        else if (progressBar.fillAmount > orangeThreshold / 100f)
        {
            progressBar.color = yellowBar;
        }
        else if (progressBar.fillAmount > redThreshold / 100f)
        {
            progressBar.color = orangeBar;
        }
        else
        {
            progressBar.color = redBar;
            countdown.enabled = true;
            countdown.text = currentTime.ToString("00.0")+"s";
        }

        if(consuming)
        {

            consumingTime-= Time.deltaTime;
            
            if (consumingTime < 0 )
            {
                doorAnimator.SetBool("doorsOpen", true);
                OnDoorsActivated?.Invoke(this, EventArgs.Empty);
                consumingTime = 2f;
                consumed = false;
                consuming = false;
            }
            else if (consumingTime < 1 && !consumed)
            {
                consumed = true;
                Destroy(heldCell);
                OnConsumedCell?.Invoke(this, EventArgs.Empty);
                heldCell = null;
                currentTime += bonusTime;
            }
        }
    }

    private void SetDropPoint(int id)
    {
        int rndTypeID = UnityEngine.Random.Range(0, 4);
        switch(rndTypeID)
        {
            case 0:
                acceptedCellTypes[id] = EnergyCellSO.CellType.Red;
                dropPoints[id].GetComponentInChildren<SpriteRenderer>().color = Color.red;
                break;
            case 1:
                acceptedCellTypes[id] = EnergyCellSO.CellType.Yellow;
                dropPoints[id].GetComponentInChildren<SpriteRenderer>().color = Color.yellow;
                break;
            case 2:
                acceptedCellTypes[id] = EnergyCellSO.CellType.Green;
                dropPoints[id].GetComponentInChildren<SpriteRenderer>().color = Color.green;
                break;
            case 3:
                acceptedCellTypes[id] = EnergyCellSO.CellType.Blue;
                dropPoints[id].GetComponentInChildren<SpriteRenderer>().color = Color.blue;
                break;
            default:
                break;
        }
    }

    private void ManageDoors()
    {
        if (!consuming)
        {

            if (Vector3.Distance(PlayerInput.Instance.transform.position, this.transform.position) < 2.8f)
            {
                doorAnimator.SetBool("doorsOpen", true);
                if (!crossedIn) OnDoorsActivated?.Invoke(this, EventArgs.Empty);
                crossedIn = true;
            }
            else
            {
                doorAnimator.SetBool("doorsOpen", false);
                if (crossedIn) OnDoorsActivated?.Invoke(this, EventArgs.Empty);
                crossedIn = false;
            }
        }
    }

    private void PauseGame()

    {
        paused = !paused;
        if (paused)
        {

            pauseMenu.SetActive(true);
            pauseMenu.GetComponentInChildren<TextMeshProUGUI>().enabled = true;
            mainMenuButton.gameObject.SetActive(true);
            gameStarted = false;
            Time.timeScale = 0f;
        }
        else
        {
            instructions.SetActive(false);
            pauseMenu.SetActive(false);
            mainMenuButton.gameObject.SetActive(false);
            gameStarted = true;
            Time.timeScale = 1f;
        }
    }

    private void OnDisable()
    {
        PlayerInput.Instance.OnInteractPerformed -= Instance_OnInteractPerformed;
        PlayerInput.Instance.OnPausePerformed -= Instance_OnPausePerformed;
    }
}
