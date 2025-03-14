using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;


public class cameraControl : NetworkBehaviour
{

  public GameObject cameraHolder;
  public Vector3 offset;

  public void Update(){
    if (SceneManager.GetActiveScene().name == "Game") {
      cameraHolder.transform.position = transform.position + offset;
    }

  }

}
