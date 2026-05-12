using UnityEngine;

public class StoneSpawner : MonoBehaviour
{
    public GameObject stonePrefab; // 돌 프리팹
    public Transform player;       // 플레이어 위치
    public float spawnRadius = 2f; // 플레이어 주변 생성 거리
    public float spawnInterval = 3f; // 생성 간격 (초)

   public bool gameEnded = false;

    void Start()
    {
        // 게임 끝날 때까지 반복 생성
        InvokeRepeating("SpawnStone", 1f, spawnInterval);

        gameEnded = false;
    }

   public void SpawnStone()
    {
        if (player == null) return;

        // 플레이어 주변 랜덤 위치 계산
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = new Vector3(player.position.x + randomCircle.x,
                                       player.position.y + 0.5f,
                                       player.position.z + randomCircle.y);

        GameObject stone =
       Instantiate(stonePrefab, spawnPos, Quaternion.identity);
    }

    public void StopSpawning()
    {
        gameEnded = true;

        CancelInvoke("SpawnStone");

        Debug.Log("돌 생성 중지");
    }

    public void StartSpawning()
    {
        gameEnded = false;

        CancelInvoke("SpawnStone");

        InvokeRepeating("SpawnStone", 1f, spawnInterval);

        Debug.Log("돌 생성 다시 시작");
    }


}