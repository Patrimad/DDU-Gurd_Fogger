using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if(PV.IsMine) // Er true hvis photonview er ejet af den lokale spiller
        {
            CreateController();
        }
    }

    void CreateController()
    {
        Debug.Log("Instantiated PlayerPrefab");
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerPrefab"), Vector3.zero, Quaternion.identity);
    }
}
