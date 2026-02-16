using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

    private void Awake()
    {
        if(Instance) // Tjekker om der er en room manager i scenen
        {
            Destroy(gameObject); // Ødelægger sig selv hvis der er
            return;
        }
        DontDestroyOnLoad(gameObject); // Hvis der ikke er andre room managers, gør den sig selv til instance, og ødelægger ikke sig selv
        Instance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }
}
