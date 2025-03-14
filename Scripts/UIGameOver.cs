using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameOver : MonoBehaviour
{
    private Canvas gameOverCanvas;

    private void Start() {
      gameOverCanvas = GetComponent<Canvas>();
    }

    private void OnEnable() {
      PlayerNetwork.GameOverEvent += GameOver;
    }

    private void OnDisable() {
      PlayerNetwork.GameOverEvent -= GameOver;
    }

    private void GameOver(){
      gameOverCanvas.enabled = true;
    }
}
