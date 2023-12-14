using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private GameObject player;
    [SerializeField] private CinemachineDollyCart cameraDolly;
    [SerializeField] private float baseFOV;
    [SerializeField] private float FOVMulti;

    private float lengthScale;
    private float radius;

    // Start is called before the first frame update
    void Start()
    {
        
        lengthScale = cameraDolly.m_Path.PathLength;
        radius = new Vector3(virtualCamera.transform.position.x, 0, virtualCamera.transform.position.z).magnitude;
    }

    // Update is called once per frame
    void LateUpdate()
    {   
        Vector2 player2DPos = new Vector2(player.transform.position.x, player.transform.position.z);
        float player2DPosMag = player2DPos.magnitude;
        
        player2DPos = new Vector2(player2DPos.x / player2DPosMag, player2DPos.y / player2DPosMag);
        float posLength = Vector2.SignedAngle(player2DPos,new Vector2(1,1));

        float FOV = baseFOV+(player.transform.position.magnitude*FOVMulti);

        if (posLength>0) SetCameraPos(posLength/360, FOV);
        else SetCameraPos((posLength+360) / 360, FOV);
       
    }

    private void SetCameraPos(float pos, float scaledFOV)
    {
        virtualCamera.m_Lens.FieldOfView = scaledFOV;
        cameraDolly.m_Position = pos*lengthScale;
    }
}
