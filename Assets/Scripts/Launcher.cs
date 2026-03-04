using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;


public class Launcher : MonoBehaviourPunCallbacks
{

    public static Launcher Instance;


    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject RoomListItemPrefab;
    [SerializeField] GameObject PlayerListItemPrefab;
    [SerializeField] GameObject startGameButton;

    void Awake()
    {
        Instance= this;
    }

    void Start()
    {
        
        Debug.Log("Connecting to Master");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() // Når du er forbundet til photons main server
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby() //Når man er connected til photon
    {
        MenuManager.Instance.OpenMenu("title");
        Debug.Log("Joined Lobby");
        PhotonNetwork.NickName = "Player" + Random.Range(0, 1000).ToString("0000"); // Giv spilleren et tilfældigt player number
    }

    public void CreateRoom() // Lav et room
    {
        if(string.IsNullOrEmpty(roomNameInputField.text)) // Hvis der ikke bliver tastet noget, så kan man ikke lave roomet
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInputField.text); //Lav et room med det givne navn
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name; // Display room navnet

        Player[] players = PhotonNetwork.PlayerList; // Tilføj players til playerlist

        for(int i = 0; i < players.Count(); i++) 
        {
            Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]); // Vist playerlist
        }

        startGameButton.SetActive(PhotonNetwork.IsMasterClient); // Gør, så du kun kan starte gamet hvis du er master client
    }

    public override void OnMasterClientSwitched(Player newMasterClient) // Sætter ny master client hvis nuværende master client går ud
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient); // Sætter start game button til aktiv for nye master client
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed: " +  message; // Viser text hvis room creation fejler
        MenuManager.Instance.OpenMenu("error"); // Viser fejlen hvis room creation fejler
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1); // Loader Scene 1. PhotonNetwork sørger for at alle spillere gør det efter der er trykket start game
    }

    public void LeaveRoom() 
    {
        PhotonNetwork.LeaveRoom(); //Forlader room
        MenuManager.Instance.OpenMenu("loading"); // Åbner loading menuen, hvilket fører tilbage til start menuen
    }

    public void JoinRoom (RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name); 
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("title"); // Viser startmenuen efter man har forladt rummet.
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) // Funktion, der laver room list
    {
        
        for (int i = roomListContent.childCount - 1; i >= 0; i--) 
        {
            Destroy(roomListContent.GetChild(i).gameObject);
        }

        foreach (RoomInfo info in roomList)
        {
            // Spring over hvis rummet er fjernet
            if (info.RemovedFromList)
                continue;

            // Lav knappen
            GameObject entry = Instantiate(RoomListItemPrefab, roomListContent);

            // Sikkerhedstjek: Find scriptet
            if (entry.TryGetComponent(out RoomListItem listItem))
            {
                listItem.Setup(info);
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer); // Opdaterer roomet når en player joiner.
    }
}
