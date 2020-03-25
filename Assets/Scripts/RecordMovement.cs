using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class RecordMovement : MonoBehaviour
{
    public int LevelNumber = 0;
    public int PlayerNumber = 0;
    public float RecordInterval = 0f;
    public string SaveLocation = "";

    private float timer = 0;


    private List<float> positionsX = new List<float>();
    private List<float> positionsY = new List<float>();

    private RecordingManager rm;

    private void Awake()
    {
        rm = GameObject.Find("GameManager").transform.GetComponent<RecordingManager>();
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

    public void SaveFile()
    {
        string destination = SaveLocation + "/Level" + LevelNumber + "-" + "Player" + PlayerNumber + ".dat";
        FileStream file;

        file = File.Create(destination);

        FrameData data = new FrameData(positionsX, positionsY);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, data);
        file.Close();
    }
}
