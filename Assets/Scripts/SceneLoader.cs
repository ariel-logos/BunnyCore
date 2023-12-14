using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] Button playButton;
    [SerializeField] Button quitButton;


    private void Awake()
    {

        if (playButton!= null) playButton.onClick.AddListener(() =>
        {
            SceneData.nextScene = "GameScene";
            SceneManager.LoadScene("LoadingScene");
        });

        if (quitButton != null) quitButton.onClick.AddListener(() => { Application.Quit(); });
    }

    private void Start()
    {
        if (SceneData.nextScene == "GameScene")
        {
            Time.timeScale = 1;
            SceneData.nextScene="";
            SceneManager.LoadScene("GameScene");
        }
        if (SceneData.nextScene == "HomeScene")
        {
            Time.timeScale = 1;
            SceneData.nextScene = "";
            SceneManager.LoadScene("HomeScene");

        }
    }
}
