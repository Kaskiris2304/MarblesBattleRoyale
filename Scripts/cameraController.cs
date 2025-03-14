using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class cameraController : NetworkBehaviour
{
  public Transform target; // the object that the camera will follow
  public Vector3 offset; // the offset from the target object's position

  void LateUpdate()
  {
      // set the camera's position to the target object's position plus the offset
      transform.position = target.position + offset;

      // make the camera look at the target object
      transform.LookAt(target);
  }

}
