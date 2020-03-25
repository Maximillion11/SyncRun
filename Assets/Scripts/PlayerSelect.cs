using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelect : MonoBehaviour
{
    private Button playerButton;
    private RecordingManager rm;

    private void Awake()
    {
        playerButton = GetComponent<Button>();
        rm = GameObject.Find("GameManager").GetComponent<RecordingManager>();

        playerButton.onClick.AddListener(() => rm.ActivatePlayer(int.Parse(transform.name.Substring(10))));
    }
}
