using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

public class TestLobby : MonoBehaviour {
  private async void Start() {
    await UnityServices.InitializeAsync();

    AuthenticationService.Instance.SignedIn += () => {
      Debug.Log("Signed in" + AuthenticationService.Instance.PlayerId);
    };

    await AuthenticationService.Instance.SignInAnonymouslyAsync();
  }


  private async void CreatLobby() {
    try{
      string lobbyName = "MyLobby";
      int maxPlayers = 4;
      Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);

      Debug.Log("Create Lobby" + lobby.Name + " " +lobby.MaxPlayers);
    } catch (LobbyServiceException e) {
      Debug.Log(e);
    }

  }
}
