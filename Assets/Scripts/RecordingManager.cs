using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RecordingManager : MonoBehaviour
{
    public int MaxPlayerNumber = 2;
    public int ActivePlayerNumber = -1;
    public bool PlayerIsActive = false;
    public bool RecordedPlayerIsActive = false;
    public float RecordInterval = 0.02f;

    private string SaveLocation = "";
    private GameObject Player;
    public List<GameObject> RecordedPlayers = new List<GameObject>();
    public List<GameObject> PlayerIcons = new List<GameObject>();

    public GameObject PlayerSpawns;
    private int levelNumber = 0;

    private void Awake()
    {
        levelNumber = SceneManager.GetActiveScene().buildIndex;

        SaveLocation = Application.persistentDataPath + "/SavedData/";
        Directory.CreateDirectory(SaveLocation);

        for (int i = 0; i < MaxPlayerNumber; i++)
        {
            RecordedPlayers.Add(null);
        }

        for (int i = 0; i < MaxPlayerNumber; i++)
        {
            if (File.Exists(SaveLocation + "/Level" + levelNumber + "-" + "Player" + i + ".dat"))
            {
                CreatePlayerIcons(i, true);
                CreateRecordedPlayer(i, true);
                ActivePlayerNumber++;
            }
            else
            {
                CreatePlayerIcons(i, false);
                CreateRecordedPlayer(i, false);
            }
        }
    }

    private void CreatePlayer()
    {
        Transform spawn = PlayerSpawns.transform.GetChild(ActivePlayerNumber);

        Player = (GameObject)GameObject.Instantiate(Resources.Load("Player"), spawn.position, Quaternion.identity);
        Player.name = "Player";
        Player.GetComponent<RecordMovement>().PlayerNumber = ActivePlayerNumber;
        Player.GetComponent<RecordMovement>().RecordInterval = RecordInterval;
        Player.GetComponent<RecordMovement>().SaveLocation = SaveLocation;
    }

    private void CreateRecordedPlayer(int playerNumber, bool isRecorded)
    {
        if (isRecorded)
        {
            FrameData data = LoadFile(SaveLocation + "/Level" + levelNumber + "-" + "Player" + playerNumber + ".dat");

            Vector2 spawnLocation = new Vector2(data.PositionsX[0], data.PositionsY[0]);

            GameObject playerRecorded = (GameObject)GameObject.Instantiate(Resources.Load("PlayerRecorded"), spawnLocation, Quaternion.identity);
            playerRecorded.name = "PlayerRecorded" + playerNumber;
            playerRecorded.GetComponent<ReplayMovement>().PlayerNumber = playerNumber;
            playerRecorded.GetComponent<ReplayMovement>().RecordInterval = RecordInterval;

            playerRecorded.GetComponent<ReplayMovement>().PositionsX = data.PositionsX;
            playerRecorded.GetComponent<ReplayMovement>().PositionsY = data.PositionsY;

            RecordedPlayers[playerNumber] = playerRecorded;
        }
        else
        {
            RecordedPlayers[playerNumber] = null;
        }
    }

    public void ClearRecordedPlayers()
    {
        foreach (GameObject recordedPlayer in RecordedPlayers)
        {
            if (recordedPlayer != null)
            {
                Destroy(recordedPlayer);
            }
        }

        for (int i = 0; i < MaxPlayerNumber; i++)
        {
            PlayerIcons[i].GetComponent<Image>().color = Color.red;
            DeleteFile(SaveLocation + "/Level" + levelNumber + "-" + "Player" + i + ".dat");
            CreateRecordedPlayer(i, false);
        }

        DestroyPlayer(true);
    }

    private void ResetRecordedPlayers()
    {
        foreach (GameObject recordedPlayer in RecordedPlayers)
        {
            if (recordedPlayer != null)
            {
                recordedPlayer.GetComponent<ReplayMovement>().Reset();
            }
        }
    }

    private void CreatePlayerIcons(int playerNumber, bool alreadyRecorded)
    {
        GameObject playerIcon;

        if (playerNumber == 0)
        {
            playerIcon = (GameObject)GameObject.Instantiate(Resources.Load("PlayerIcon"), GameObject.Find("PlayersTextStop").transform);
        }
        else
        {
            playerIcon = (GameObject)GameObject.Instantiate(Resources.Load("PlayerIcon"), GameObject.Find("PlayerIcon" + (playerNumber - 1)).transform);
        }
        playerIcon.name = "PlayerIcon" + playerNumber;
        playerIcon.transform.position += new Vector3(24, 0, 0);
        playerIcon.transform.SetParent(GameObject.Find("PlayersText").transform);

        if (alreadyRecorded)
        {
            playerIcon.GetComponent<Image>().color = Color.white;
        }

        PlayerIcons.Add(playerIcon);
    }

    private void UpdatePlayerIcons(int playerNumber)
    {
        for (int i = 0; i < MaxPlayerNumber; i++)
        {
            if (PlayerIsActive && i == playerNumber)
            {
                PlayerIcons[i].GetComponent<Image>().color = Color.green;
            }
            else
            {
                if (RecordedPlayers[i] != null)
                {
                    PlayerIcons[i].GetComponent<Image>().color = Color.white;
                }
                else
                {
                    PlayerIcons[i].GetComponent<Image>().color = Color.red;
                }
            }
        }
    }

    public void ActivatePlayer(int buttonClicked)
    {
        StopPlayer();

        ActivePlayerNumber += 1;
        PlayerIsActive = true;

        DestroyReliantPlayers(buttonClicked);

        Destroy(RecordedPlayers[ActivePlayerNumber]);
        CreatePlayer();
        UpdatePlayerIcons(ActivePlayerNumber);
        PlayRecordedPlayers();
    }

    private void DestroyReliantPlayers(int playerNumber)
    {
        for (int i = playerNumber; i < RecordedPlayers.Count; i++)
        {
            if (RecordedPlayers[i] != null)
            {
                Destroy(RecordedPlayers[i]);
                RecordedPlayers[i] = null;
                ActivePlayerNumber -= 1;
            }
        }
    }

    public void StopPlayer()
    {
        if (PlayerIsActive)
        {
            Player.GetComponent<RecordMovement>().SendData();
            CreateRecordedPlayer(ActivePlayerNumber, true);
            DestroyPlayer(false);
            UpdatePlayerIcons(ActivePlayerNumber);
        }

        if (RecordedPlayerIsActive)
        {
            ResetRecordedPlayers();
            RecordedPlayerIsActive = false;
        }
    }

    public void PlayRecordedPlayers()
    {
        foreach (GameObject recordedPlayer in RecordedPlayers)
        {
            if (recordedPlayer != null)
            {
                recordedPlayer.GetComponent<ReplayMovement>().Play = true;
                RecordedPlayerIsActive = true;
            }
        }
    }

    private void DestroyPlayer(bool wipe)
    {
        Destroy(Player);
        Player = null;
        PlayerIsActive = false;

        if (wipe)
        {
            ActivePlayerNumber = -1;
        }
    }

    public void SaveFile(FrameData injData, int playerNumber)
    {
        string destination = SaveLocation + "/Level" + levelNumber + "-" + "Player" + playerNumber + ".dat";
        FileStream file;

        file = File.Create(destination);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, injData);
        file.Close();
    }

    private FrameData LoadFile(string destination)
    {
        FileStream file;
        
        file = File.OpenRead(destination);

        BinaryFormatter bf = new BinaryFormatter();
        FrameData data = (FrameData)bf.Deserialize(file);
        file.Close();

        return data;
    }

    private void DeleteFile(string destination)
    {
        if (File.Exists(destination))
        {
            File.Delete(destination);
        }
    }

    public void FinishLevel()
    {
        StopPlayer();

        int nextSceneID = levelNumber + 1;

        if (nextSceneID < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneID);
        }
        else
        {
            SceneManager.LoadScene("Level0");
        }

    }
}
