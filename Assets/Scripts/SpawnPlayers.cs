using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpawnPlayers : MonoBehaviourPunCallbacks
{
    public GameObject PlayerPrefab1; // Префаб первого персонажа
    public GameObject PlayerPrefab2; // Префаб второго персонажа
    public Transform SpawnPoint1; // Точка спавна для первого игрока
    public Transform SpawnPoint2; // Точка спавна для второго игрока

    void Start()
    {
        // Спавн первого игрока при старте комнаты
        SpawnPlayer(PhotonNetwork.LocalPlayer.ActorNumber);
    }

    // public override void OnPlayerEnteredRoom(Player newPlayer)
    // {
    //     // Спавн второго игрока, когда новый игрок присоединяется
    //     SpawnPlayer(newPlayer.ActorNumber);
    // }

    private void SpawnPlayer(int actorNumber)
    {
        GameObject playerPrefab;

        // Определяем, какой префаб использовать в зависимости от номера игрока
        if (actorNumber == 1    )
        {
            playerPrefab = PlayerPrefab1;
            PhotonNetwork.Instantiate(playerPrefab.name, SpawnPoint1.position, Quaternion.identity);
        }
        else
        {
            playerPrefab = PlayerPrefab2;
            Transform spawnPoint = actorNumber % 2 == 0 ? SpawnPoint2 : SpawnPoint1;
            PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, Quaternion.identity);
        }
    }
}