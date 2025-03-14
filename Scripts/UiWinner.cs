using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiWinner : MonoBehaviour
{
  private Canvas winnerCanvas;

  private void Start() {
    winnerCanvas = GetComponent<Canvas>();
  }

  private void OnEnable() {
    PlayerNetwork.WinnerEvent += GameWon;
  }

  private void OnDisable() {
    PlayerNetwork.WinnerEvent -= GameWon;
  }

  private void GameWon(){
    winnerCanvas.enabled = true;
  }
}
