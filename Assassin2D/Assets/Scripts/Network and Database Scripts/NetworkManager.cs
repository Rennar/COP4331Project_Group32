using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// Matthew Hoskin

public class NetworkManager : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    private const string typeName = "UniqueGameName";
    private const string gameName = "RoomName";

    private void StartServer()
    {
        Network.InitializeServer(2, 25000, !Network.HavePublicAddress());
        MasterServer.RegisterHost(typeName, gameName);

    }

    private void JoinServer(HostData hostData) //hostdata is fucked
    {
        Network.Connect(hostData);
    }

    void OnGUI()
    {
        if (!Network.isClient && !Network.isServer)
        {
            if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
                StartServer();

            if (GUI.Button(new Rect(100, 250, 250, 100), "Refresh Hosts"))
                RefreshHostList();

            if (hostList != null)
            {
                for (int i = 0; i < hostList.Length; i++)
                {
                    if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hostList[i].gameName))
                        JoinServer(hostList[i]);
                }
            }
        }
    }

    private HostData[] hostList; //hostdata is fucked

    private void RefreshHostList()
    {
        MasterServer.RequestHostList(typeName);
    }

    void OnMasterServerEvent(MasterServerEvent msEvent) //masterserverevent is fucked
    {
        if (msEvent == MasterServerEvent.HostListReceived)
            hostList = MasterServer.PollHostList();
    }

    public GameObject playerPrefab;

    void OnServerInitialized()
    {
        SpawnPlayerLeft();
    }

    void OnConnectedToServer()
    {
        SpawnPlayerRight();
    }

    private void SpawnPlayerLeft()
    {
        Network.Instantiate(playerPrefab, new Vector3(-18.71f, 0f, 0f), Quaternion.identity, 0);
    }

    private void SpawnPlayerRight()
    {
        Network.Instantiate(playerPrefab, new Vector3(18.0f, 10.0f, 0f), Quaternion.identity, 0);

    }
}
