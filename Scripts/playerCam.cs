using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerCam : MonoBehaviour
{
  public GameObject cameraPrefab; // Drag your camera prefab into this field in the inspector

  private void Start()
  {
      // Instantiate camera prefab and parent it to this player object
      GameObject cameraObj = Instantiate(cameraPrefab, transform.position, Quaternion.identity);
      cameraObj.transform.parent = transform;

      // Set camera position and rotation relative to the player object
      cameraObj.transform.localPosition = new Vector3(0, 1, -3); // Adjust the position as needed
      cameraObj.transform.localRotation = Quaternion.identity; // Use the default rotation
  }
}
