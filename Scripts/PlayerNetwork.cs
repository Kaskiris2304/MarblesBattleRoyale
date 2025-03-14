using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;


public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] public Transform cameraPrefab;

    private Rigidbody rb;
    private Transform camera;
    public CinemachineFreeLook freelookcamera;
    public NetworkVariable<int> lives = new(3, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> move = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public bool flag = true;

    public Transform target;

    public static event System.Action<int> ChangedLivesEvent;
    public static event System.Action GameOverEvent;
    public static event System.Action WinnerEvent;




    void Start()
    {

        if (IsOwner)
        {
          Debug.Log("my id is: " + OwnerClientId);
          Cursor.visible = false;
          rb = GetComponent<Rigidbody>();
          freelookcamera.Priority = 10;
          freelookcamera.Follow = target.transform;
          freelookcamera.LookAt = target.transform;

          camera = Instantiate(cameraPrefab);

        }

        if(IsOwner == false) {
           freelookcamera.Priority = 0;
        }
    }



    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision 1");

        // Debug.Log("collision 2");
        Debug.Log(collision.gameObject.tag);
        if (!collision.gameObject.CompareTag("Player")) return;

        Debug.Log("collision 2");
        if(!IsOwner) return;
        Debug.Log("collision 3");

        // Debug.Log("player 2 object"+ otherPlayer.OwnerClientId);
        if (collision.gameObject.TryGetComponent(out PlayerNetwork playerNetwork))
        {
            Debug.Log("collision 4" + OwnerClientId + transform.position);
            Debug.Log("collision 4" + playerNetwork.OwnerClientId + playerNetwork.transform.position);

            var player1 = new PlayerData() {
              Id = OwnerClientId,
              Direction = -(playerNetwork.transform.position - transform.position).normalized
            };

            var player2 = new PlayerData() {
              Id = playerNetwork.OwnerClientId,
              Direction = (playerNetwork.transform.position - transform.position).normalized
            };

            ApplyForceServerRpc(player1, player2);

        }
    }



    struct PlayerData : INetworkSerializable {
      public ulong Id;
      public Vector3 Direction;

      public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref Id);
        serializer.SerializeValue(ref Direction);
      }
    }



    [ServerRpc(RequireOwnership = false)]
    private void ApplyForceServerRpc(PlayerData player1, PlayerData player2)
    {
        Debug.Log("Server log");

        Debug.Log("Server player1 id  "+ player1.Id);
        Debug.Log("Server player2 id  "+ player2.Id);


        // Player 2 is the host, apply force to its Rigidbody
        Debug.Log("Applying force from playerId: " + player1.Id + " to playerId: " + player2.Id);
        var player2Object = NetworkManager.Singleton.ConnectedClients[player2.Id].PlayerObject;
        var player2Rigidbody = player2Object.GetComponentInChildren<Rigidbody>();
        player2Rigidbody.AddForce(player2.Direction * 500f);

        Debug.Log("Applying force from playerId: " + player2.Id + " to playerId: " + player1.Id);
        var player1Object = NetworkManager.Singleton.ConnectedClients[player1.Id].PlayerObject;
        var player1Rigidbody = player1Object.GetComponentInChildren<Rigidbody>();
        player1Rigidbody.AddForce(player1.Direction * 500f);

    }



    [ServerRpc]
    public void loserServerRpc(ulong clientsId) {
      Debug.Log("loser");

      var client = NetworkManager.ConnectedClients[clientsId].PlayerObject;
      var client2 = client.GetComponentInChildren<PlayerNetwork>();
      Debug.Log("details after falling: ClientId" + clientsId + "Lives: " + client2.lives.Value);
      if(client2.lives.Value == 1) {
        client2.lives.Value -=1;
      }
      else if(client2.lives.Value == 0) {
        return;
      }
      else {
        client2.lives.Value -=1;
        StartCoroutine(MovePlayerDelayed(client2));
      }
    }

    private IEnumerator MovePlayerDelayed(PlayerNetwork client2) {
      yield return new WaitForSeconds(1.5f);
      client2.transform.position = new Vector3(7.6f, 1.0f, -1.32f);
    }



    [ServerRpc]
    public void shutPlayerServerRpc(ulong identity) {
      Debug.Log("shutting down player: " + identity);
      var client5 = NetworkManager.ConnectedClients[identity].PlayerObject;
      var client6 = client5.GetComponentInChildren<PlayerNetwork>();
      client6.move.Value = 1;
    }



    [ServerRpc]
    public void checkWinnerServerRpc()
    {

      // Check how many players are remaining
      Debug.Log("Checking for possible winner");
      int numberOfPlayersRemaining = 0;
      foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
      {
        var playerObject = client.PlayerObject;
        if (playerObject != null)
        {
            var playerNetwork = playerObject.GetComponentInChildren<PlayerNetwork>();
            if (playerNetwork != null && playerNetwork.move.Value != 1)
            {
                numberOfPlayersRemaining++;
            }
        }
      }

      // If only one player is remaining, declare them as the winner
      if (numberOfPlayersRemaining == 1)
      {
          foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
          {
            var playerObject = client.PlayerObject;
            if (playerObject != null)
            {
                var playerNetwork = playerObject.GetComponentInChildren<PlayerNetwork>();
                if (playerNetwork != null && playerNetwork.move.Value != 1)
                {
                    Debug.Log("Player " + client.ClientId + " is the winner!");
                    playerNetwork.move.Value = 2;
                }
            }
          }
      }
    }





    void Update() {
      if (!IsOwner && !IsHost) return;
      // Debug.Log("client with id: " + OwnerClientId + "move number is: " + move);

      if (move.Value == 0) {
        MovePlayerServer();
      }
      else if (move.Value == 1) {
        return;
      }
      else if (move.Value == 2) {
        Debug.Log("Winner is client: " + OwnerClientId);
        WinnerEvent?.Invoke();
        return;
      }


      if(lives.Value == 2) {
        ChangedLivesEvent?.Invoke(lives.Value);
      }
      else if(lives.Value == 1) {
        ChangedLivesEvent?.Invoke(lives.Value);
      }
      else if (lives.Value == 0) {
        ChangedLivesEvent?.Invoke(lives.Value);
        shutPlayerServerRpc(OwnerClientId);
        GameOverEvent?.Invoke();
        checkWinnerServerRpc();
      }

    }

    private void MovePlayerServer() {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        movement = Quaternion.AngleAxis(camera.rotation.eulerAngles.y, Vector3.up) * movement;
        MovePlayerServerRpc(movement, speed);
    }



    [ServerRpc]
    private void MovePlayerServerRpc(Vector3 movement, float speed, ServerRpcParams serverRpcParams = default) {


      var clientId = serverRpcParams.Receive.SenderClientId;
      Debug.Log("Movement of player 2: " + clientId);
      if (NetworkManager.ConnectedClients.ContainsKey(clientId))
      {
        var client = NetworkManager.ConnectedClients[clientId].PlayerObject;
        var client2 = client.GetComponentInChildren<Rigidbody>();
        client2.AddForce(movement * speed);
        Debug.Log("Applied force to client: " + clientId);
      }

    }

}
