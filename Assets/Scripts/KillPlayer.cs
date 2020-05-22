using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayer : MonoBehaviour
{
    RecordingManager rm;

    private void Awake()
    {
        rm = GameObject.Find("GameManager").GetComponent<RecordingManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            rm.StopPlayer();
        }
    }
}
