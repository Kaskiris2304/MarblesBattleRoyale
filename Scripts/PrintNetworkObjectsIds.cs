using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PrintNetworkObjectsIds : MonoBehaviour
{
    void Start()
    {
        // Get all player objects in the game
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        // Loop through each player object and print its NetworkObjectId
        foreach (GameObject player in players)
        {
            NetworkObject netObj = player.GetComponent<NetworkObject>();
            if (netObj != null)
            {
                Debug.Log("Player object " + player.name + " has NetworkObjectId " + netObj.NetworkObjectId);
            }
        }
    }
}
