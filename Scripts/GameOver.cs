using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameOver : MonoBehaviour
{

    private void OnTriggerEnter(Collider col) {

      Debug.Log("Collisison occured with player tag" + col.tag);
      if(!col.CompareTag("Player")) return;

      // if(!NetworkManager.Singleton.IsServer) return;
      Debug.Log("Tag of Object is   " + col.tag);

      var parentObject = col.transform.parent.gameObject;

      if(parentObject.TryGetComponent(out PlayerNetwork playerNetwork)) {
        Debug.Log("Found object controller");
        ulong clientId = playerNetwork.OwnerClientId;
        playerNetwork.loserServerRpc(clientId);

      }
    }
}
