using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordMovement : MonoBehaviour
{
    public int PlayerNumber = 0;
    public float RecordInterval = 0f;
    public string SaveLocation = "";

    private float timer = 0;

    private List<float> positionsX = new List<float>();
    private List<float> positionsY = new List<float>();

    RecordingManager rm;

    private void Start()
    {
        rm = GameObject.Find("GameManager").GetComponent<RecordingManager>();
    }

    private void Update()
    {
        if (timer >= RecordInterval)
        {
            RecordFrame();

            timer = 0;
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    private void RecordFrame()
    {
        positionsX.Add(transform.position.x);
        positionsY.Add(transform.position.y);
    }

    public void SendData()
    {
        rm.SaveFile(new FrameData(positionsX, positionsY), PlayerNumber);
    }
}
