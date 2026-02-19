using UnityEngine;

public class Spawnpoint : MonoBehaviour
{

    [SerializeField] GameObject graphics;
    void Awake()
    {
        graphics.SetActive(false); // Når vi starter spillet, bliver spawnpointsne usynlige
    }
}
