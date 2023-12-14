using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disposer : MonoBehaviour
{
    [SerializeField] Animator animator;
    [HideInInspector] public GameObject heldCell;

    public GameObject dropPoint;

    private bool disposing;
    private float disposeTime = 2f;

    // Start is called before the first frame update
    void Start()
    {
        PlayerInput.Instance.OnInteractPerformed += Instance_OnInteractPerformed;
    }

    private void Instance_OnInteractPerformed(object sender, System.EventArgs e)
    {
        if (!disposing &&
            PlayerInput.Instance.heldCell != null &&
            heldCell == null &&
            Vector3.Distance(this.transform.position, PlayerInput.Instance.heldCell.transform.position) < 1f)
        {
            heldCell = PlayerInput.Instance.heldCell;
            heldCell.GetComponent<EnergyCell>().parent = this.gameObject;
            heldCell.transform.parent = dropPoint.transform;
            heldCell.transform.localPosition = Vector3.zero;
            PlayerInput.Instance.heldCell = null;
            disposing = true;
            animator.SetBool("disposeCell", true);
        }
       

    }

    private void Update()
    {
        if(disposing)
        {
            disposeTime -= Time.deltaTime;
            if (disposeTime < 0f)
            {
                disposing = false;
                animator.SetBool("disposeCell", false);
                disposeTime = 2f;
            }
            else if (disposeTime < 1f)
            {
                Destroy(heldCell);
                heldCell = null;
                
            }
        }
    }

    private void OnDisable()
    {
        PlayerInput.Instance.OnInteractPerformed -= Instance_OnInteractPerformed;
    }
}
