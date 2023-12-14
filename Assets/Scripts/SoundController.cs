using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] private AudioSource mainTrackAudioSource;
    [SerializeField] private AudioSource spawnerAudioSource;
    [SerializeField] private AudioSource walkingAudioSource;
    [SerializeField] private AudioSource doorsAudioSource;
    [SerializeField] private AudioSource consumedCellAudioSource;
    [SerializeField] private AudioSource gameOverAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        Reactor.Instance.OnDoorsActivated += Instance_OnDoorsActivated;
        Reactor.Instance.OnConsumedCell += Instance_OnConsumedCell;
        Reactor.Instance.OnGameOver += Instance_OnGameOver;
    }

    private void Instance_OnGameOver(object sender, System.EventArgs e)
    {
        mainTrackAudioSource.Stop(); 
        gameOverAudioSource.Play();
    }

    private void Instance_OnConsumedCell(object sender, System.EventArgs e)
    {
        consumedCellAudioSource.Play();
    }

    private void Instance_OnDoorsActivated(object sender, System.EventArgs e)
    {
        doorsAudioSource.Play();
    }



    // Update is called once per frame
    void Update()
    {
        if(Spawner.isSpawned)
        {
            spawnerAudioSource.Play();
            Spawner.isSpawned = false;
        }
        if(PlayerInput.Instance.isMoving)
        {
            if(!walkingAudioSource.isPlaying)walkingAudioSource.Play();
        }
        
    }

    private void OnDisable()
    {
        Reactor.Instance.OnDoorsActivated -= Instance_OnDoorsActivated;
        Reactor.Instance.OnConsumedCell -= Instance_OnConsumedCell;
        Reactor.Instance.OnGameOver -= Instance_OnGameOver;
    }
}
