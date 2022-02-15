using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

public class RoomManager : MonoBehaviourPunCallbacks
{

    private GameObject spawnedPlayerPrefab;
    public string roomName;
    private void Start()
    {
        Debug.Log("RoomManager started");
        JoinRoom(roomName);
    }

    // PUN Callbacks that is Called when the local player left the room. We need to load the launcher scene.
    public override void OnLeftRoom()
    {
        // Scene at index 0 is the lobby, we can see this in the build settings.
        PhotonNetwork.Destroy(spawnedPlayerPrefab);
        SceneManager.LoadScene(0);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("PUN Basic Launcher: OnJoinedRoom() called by PUN. Now this client is in " + PhotonNetwork.CurrentRoom.Name);
        spawnedPlayerPrefab = PhotonNetwork.Instantiate("Network Player", transform.position, transform.rotation);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log("Joined room failed, returing to the main lobby");
        SceneManager.LoadScene(0);
    }

    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.LogFormat("New player join called: OnPlayerEnteredRoom()"); // not seen if you're the player connecting


        //if (PhotonNetwork.IsMasterClient)
        //{
        //    Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
        //    LoadRoom();
        //}
    }

    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.LogFormat("OnPlayerLeftRoom()"); // seen when other disconnects


        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

            LoadRoom();
        }
    }
    public void JoinRoom(string roomName)
    {
        Debug.Log("PUN Basic Launcher: Attempting to Join a random room");
        // attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
        //PhotonNetwork.JoinRoom("Room 1");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        bool success = PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
        if (!success)
        {
            SceneManager.LoadScene(0);
        }
    }

    // Method to call when user leave room
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    void LoadRoom()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
        } else
        {
            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            //PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
            //PhotonNetwork.LoadLevel("Room 1");
        }
    }
}