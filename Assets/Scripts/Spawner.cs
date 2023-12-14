using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


public class Spawner : MonoBehaviour
{
    public static bool isSpawned = false;
    public enum SpawnerState
    {
        Waiting,
        Spawning,
        Spawned
    }
    private float spawnTimer;
    [SerializeField] private GameObject[] cellPrefabs;
    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private Animator animator;
    [HideInInspector] public GameObject spawnedCell;


    public SpawnerState state;

    // Start is called before the first frame update
    private void Awake()
    {
        
    }

    private void Instance_OnInteractPerformed(object sender, EventArgs e)
    {
        if (state == SpawnerState.Spawned &&
            Vector3.Distance(this.transform.position,PlayerInput.Instance.transform.position)<1.5f &&
            PlayerInput.Instance.heldCell == null)
        {
            spawnedCell.GetComponent<EnergyCell>().parent = PlayerInput.Instance.gameObject;
            spawnedCell.transform.parent = PlayerInput.Instance.holdPoint.transform;
            PlayerInput.Instance.heldCell = spawnedCell;
            spawnedCell.transform.localPosition = Vector3.zero;
            animator.SetBool("isSpawned", false);
            spawnedCell = null;
            spawnTimer = UnityEngine.Random.Range(2f, 10f);
            state = SpawnerState.Waiting;
        }
    }

    void Start()
    {
        PlayerInput.Instance.OnInteractPerformed += Instance_OnInteractPerformed;
        spawnTimer = UnityEngine.Random.Range(2f,10f);
    }

    // Update is called once per frame
    void Update()
    {
        if (state == SpawnerState.Waiting)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer < 0)
            {
                state = SpawnerState.Spawning;
                spawnTimer = 1;
                if (spawnedCell == null)
                {
                    spawnedCell = Instantiate(cellPrefabs[UnityEngine.Random.Range(1, 5)]);
                    spawnedCell.GetComponent<EnergyCell>().parent = spawnPoint;                    
                    spawnedCell.transform.parent = spawnPoint.transform;
                    spawnedCell.transform.localPosition = Vector3.zero;
                    animator.SetBool("isSpawned", true);
                    
                }
            }
        }
        else if (state == SpawnerState.Spawning)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer < 0)
            {
                isSpawned = true;
                state = SpawnerState.Spawned;
            }
        }
    }

    private void OnDisable()
    {
        PlayerInput.Instance.OnInteractPerformed -= Instance_OnInteractPerformed; 
    }
}
