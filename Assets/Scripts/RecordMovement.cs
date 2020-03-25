using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class RecordMovement : MonoBehaviour
{
    public int LevelNumber = 0;
    public int PlayerNumber = 0;
    public string SaveLocation = "";

    private Vector2 startPosition = new Vector2();
    private Dictionary<int, float> horizontal = new Dictionary<int, float>();
    private Dictionary<int, bool> jumpBool = new Dictionary<int, bool>();
    private int lastFrame = 0;

    private int frame = 0;
    private float inputX = 0;

    private void Awake()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        if (x != inputX)
        {
            RecordFrame("Horizontal", x.ToString());
            inputX = x;
        }

        bool jump = Input.GetButtonDown("Jump");
        if (jump)
        {
            RecordFrame("Jump", jump.ToString());
        }

        frame++;
    }

    private void RecordFrame(string inputType, string inputValue)
    {
        if (inputType == "Horizontal")
        {
            horizontal.Add(frame, int.Parse(inputValue));
        }
        if (inputType == "Jump")
        {
            jumpBool.Add(frame, bool.Parse(inputValue));
        }
    }

    public void SaveFile()
    {
        lastFrame = frame;

        string destination = SaveLocation + "/Level" + LevelNumber + "-" + "Player" + PlayerNumber + ".dat";
        FileStream file;

        file = File.Create(destination);

        FrameData data = new FrameData(startPosition.x, startPosition.y, lastFrame, horizontal, jumpBool);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, data);
        file.Close();
    }
}
