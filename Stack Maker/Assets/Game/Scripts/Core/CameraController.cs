using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

//change beetwen camera and Virtual camera
public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cmInGame, cmMiniGame;

    private void OnEnable()
    {
        GameManager.ActionMiniGame += SetMiniGameCamera;
    }

    private void SetMiniGameCamera()
    {
        cmInGame.enabled = false;
        cmMiniGame.enabled = true;
    }

    private void OnDisable()
    {
        GameManager.ActionMiniGame -= SetMiniGameCamera;
    }
}
