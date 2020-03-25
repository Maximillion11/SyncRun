using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;

public class RecordingManager : MonoBehaviour
{
    public int LevelNumber = 0;
    public int MaxPlayerNumber = 2;
    public int ActivePlayerNumber = 0;
    public bool PlayerIsActive = false;
    public bool RecordedPlayerIsActive = false;

    private string SaveLocation = "";
    private GameObject Player;
    public List<GameObject> RecordedPlayers = new List<GameObject>();
    public List<GameObject> PlayerIcons = new List<GameObject>();

    private void Awake()
    {
        SaveLocation = Application.persistentDataPath + "/SavedData/";
        Directory.CreateDirectory(SaveLocation);

        for (int i = 0; i < MaxPlayerNumber; i++)
        {
            RecordedPlayers.Add(null);
        }

        for (int i = 0; i < MaxPlayerNumber; i++)
        {
            if (File.Exists(SaveLocation + "/Level" + LevelNumber + "-" + "Player" + i + ".dat"))
            {
                CreatePlayerIcons(i, true);
                CreateRecordedPlayer(i, true);
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
        Player = (GameObject)GameObject.Instantiate(Resources.Load("Player"), new Vector2(-5.6f, -0.51f), Quaternion.identity);
        Player.name = "Player";
        Player.GetComponent<RecordMovement>().LevelNumber = LevelNumber;
        Player.GetComponent<RecordMovement>().PlayerNumber = ActivePlayerNumber;
        Player.GetComponent<RecordMovement>().SaveLocation = SaveLocation;
    }

    private void CreateRecordedPlayer(int playerNumber, bool isRecorded)
    {
        if (isRecorded)
        {
            GameObject playerRecorded = (GameObject)GameObject.Instantiate(Resources.Load("PlayerRecorded"), new Vector2(-5.6f, -0.51f), Quaternion.identity);
            playerRecorded.name = "PlayerRecorded" + playerNumber;
            playerRecorded.GetComponent<Player_Movement>().LevelNumber = LevelNumber;
            playerRecorded.GetComponent<Player_Movement>().PlayerNumber = playerNumber;

            LoadFile(SaveLocation + "/Level" + LevelNumber + "-" + "Player" + playerNumber + ".dat", playerRecorded.GetComponent<Player_Movement>());

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
            DeleteFile(SaveLocation + "/Level" + LevelNumber + "-" + "Player" + i + ".dat");
            CreateRecordedPlayer(i, false);
        }

        DestroyPlayer();
    }

    private void ResetRecordedPlayers()
    {
        foreach (GameObject recordedPlayer in RecordedPlayers)
        {
            if (recordedPlayer != null)
            {
                recordedPlayer.GetComponent<Player_Movement>().Reset();
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
        playerIcon.transform.parent = GameObject.Find("PlayersText").transform;

        if (alreadyRecorded)
        {
            playerIcon.GetComponent<Image>().color = Color.green;
        }

        PlayerIcons.Add(playerIcon);
    }

    private void UpdatePlayerIcons(int playerNumber)
    {
        for (int i = 0; i < MaxPlayerNumber; i++)
        {
            if (PlayerIsActive && i == playerNumber)
            {
                PlayerIcons[i].GetComponent<Image>().color = Color.white;
            }
            else
            {
                if (File.Exists(SaveLocation + "/Level" + LevelNumber + "-" + "Player" + i + ".dat"))
                {
                    PlayerIcons[i].GetComponent<Image>().color = Color.green;
                }
                else
                {
                    PlayerIcons[i].GetComponent<Image>().color = Color.red;
                }
            }
        }
    }

    public void ActivatePlayer(int playerNumber)
    {
        StopPlayer();

        ActivePlayerNumber = playerNumber;
        PlayerIsActive = true;

        if (ActivePlayerNumber < MaxPlayerNumber)
        {
            Destroy(RecordedPlayers[playerNumber]);
            CreatePlayer();
            UpdatePlayerIcons(ActivePlayerNumber);
            PlayRecordedPlayers();
        }
    }

    public void StopPlayer()
    {
        if (PlayerIsActive)
        {
            Player.GetComponent<RecordMovement>().SaveFile();
            CreateRecordedPlayer(Player.GetComponent<RecordMovement>().PlayerNumber, true);
            DestroyPlayer();
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
                recordedPlayer.GetComponent<Player_Movement>().Reset();
                recordedPlayer.GetComponent<Player_Movement>().Play = true;
                RecordedPlayerIsActive = true;
            }
        }
    }

    private void DestroyPlayer()
    {
        Destroy(Player);
        Player = null;
        PlayerIsActive = false;
    }

    private void LoadFile(string destination, Player_Movement rm)
    {
        FileStream file;
        
        file = File.OpenRead(destination);

        BinaryFormatter bf = new BinaryFormatter();
        FrameData data = (FrameData)bf.Deserialize(file);
        file.Close();

        rm.StartPosition = new Vector2(data.StartPositionX, data.StartPositionY);
        rm.LastFrame = data.LastFrame;
        rm.Horizontal = data.Horizontal;
        rm.JumpBool = data.JumpBool;
    }

    private void DeleteFile(string destination)
    {
        if (File.Exists(destination))
        {
            File.Delete(destination);
        }
    }
}
