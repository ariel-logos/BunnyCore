using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{

    [HideInInspector] public GameObject heldCell;

    public GameObject dropPoint;

    // Start is called before the first frame update
    void Start()
    {
        PlayerInput.Instance.OnInteractPerformed += Instance_OnInteractPerformed;
    }

    private void Instance_OnInteractPerformed(object sender, System.EventArgs e)
    {
        if(PlayerInput.Instance.heldCell != null && heldCell == null && Vector3.Distance(this.transform.position, PlayerInput.Instance.heldCell.transform.position) < 1f)
        {
            heldCell = PlayerInput.Instance.heldCell;
            heldCell.GetComponent<EnergyCell>().parent = this.gameObject;
            heldCell.transform.parent = dropPoint.transform;
            heldCell.transform.localPosition = Vector3.zero;
            PlayerInput.Instance.heldCell = null;
        }
        else if (PlayerInput.Instance.heldCell == null && heldCell != null && Vector3.Distance(this.transform.position, PlayerInput.Instance.transform.position) < 1.5f)
        {
            PlayerInput.Instance.heldCell = heldCell;
            heldCell.GetComponent<EnergyCell>().parent = PlayerInput.Instance.gameObject;
            heldCell.transform.parent = PlayerInput.Instance.holdPoint.transform;
            heldCell.transform.localPosition = Vector3.zero;
            heldCell = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        PlayerInput.Instance.OnInteractPerformed -= Instance_OnInteractPerformed;
    }
}
