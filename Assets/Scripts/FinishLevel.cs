using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLevel : MonoBehaviour
{
    RecordingManager rm;

    private void Awake()
    {
        rm = GameObject.Find("GameManager").GetComponent<RecordingManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            rm.FinishLevel();
        }
    }
}
