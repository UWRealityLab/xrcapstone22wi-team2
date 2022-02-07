using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    // This client's version number. Users are separated from each other by gameVersion.
    [SerializeField]    
    string gameVersion = "1";

    // Max number of players per room
    [SerializeField]
    private byte maxPlayerPerRoom = 4;

    // MonoBehaviour method called on GameObject by Unity during early initialization phase.
    private void Awake()
    {
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master
        // client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        ConnectToPhoton();
    }

    public void ConnectToPhoton()
    {
        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("PUN Basic Launcher: Attempting to connect to the server");
            // we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    public void JoinRoom()
    {
        Debug.Log("PUN Basic Launcher: Attempting to Join a random room");
        // attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
        PhotonNetwork.JoinRandomRoom();
    }

    // Method called if PUN is able to connect
    public override void OnConnectedToMaster()
    {
        //base.OnConnectedToMaster();
        Debug.Log("PUN Basic Launcher: OnConnectedToMaster() was called by PUN");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        //base.OnDisconnected(cause);
        Debug.LogWarningFormat("PUN Basic Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //base.OnJoinRandomFailed(returnCode, message);
        Debug.Log("PUN Basic Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayerPerRoom;
        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        //base.OnJoinedRoom();
        Debug.Log("PUN Basic Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
    }
}
