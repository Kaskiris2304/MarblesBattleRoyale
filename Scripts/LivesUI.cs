using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class LivesUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI livestext;

    private void Start() {
      // gameObject.SetActive(false);
    }

    private void OnEnable() {
      PlayerNetwork.ChangedLivesEvent += ChangeLivesText;
    }

    private void OnDisable() {
      PlayerNetwork.ChangedLivesEvent -= ChangeLivesText;
    }


    private void ChangeLivesText(int lives) {
      livestext.text = lives.ToString();
    }
}
