using System.Collections;
using UnityEngine;

public class GiantController : MonoBehaviour
{
    public Transform player;
    public float speed = 2f;
    public float gameOverDistance = 2f;
    public Animator anim;

    [Header("Combat & Health")]
    public int minHits = 3;
    public int maxHits = 7;

    private int requiredHits;
    private int currentHits = 0;

    public bool isDead = false; // 거인의 사망 상태
    public bool isStunned = false;
    public bool isWin = false;

    [Header("Roar Settings")]
    public float roarCheckInterval = 3f; 
    [Range(0, 100)]
    public int roarChance = 20;          
    private float roarTimer;

    [Header("Audio Settings")]
    private AudioSource audioSource;
    public AudioClip hitSound;  // 돌에 맞았을 때 소리
    public AudioClip roarSound; // 울 때 소리
    public AudioClip dieSound;  // 죽을 때 소리

    public StoneSpawner stoneSpawner;
    public EndingManager endingManager;
    void Start()
    {
        requiredHits = Random.Range(minHits, maxHits + 1);
        audioSource = GetComponent<AudioSource>();
        Debug.Log("이번 거인은 " + requiredHits + "대 맞아야 죽음");
    }
    void Update()
    {
        if (player == null || isDead || isStunned || isWin) return;

        // 1. 현재 거인이 울고 있는지 확인
        bool isCrying = anim != null && anim.GetCurrentAnimatorStateInfo(0).IsName("roar");

        if (!isCrying)
        {
            transform.LookAt(player);
            transform.position += transform.forward * speed * Time.deltaTime;
        }

        // 2. 게임 오버 체크
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance < gameOverDistance)
        {
            isWin = true;
       
            anim.SetTrigger("WinPose");

            stoneSpawner.StopSpawning();

            StartCoroutine(LoseRoutine());
        }

        // 3. 울음 타이머 (울지 않을 때만 타이머가 가게 할 수도 있고, 계속 가게 할 수도 있습니다)
        roarTimer += Time.deltaTime;
        if (roarTimer >= roarCheckInterval)
        {
            TryRoar();
            roarTimer = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Stone")) return;
        Debug.Log(other.name);
        TakeDamage();
        Destroy(other.gameObject);
    }

    public void TakeDamage()
    {
        if (isDead || isWin) return;

        currentHits++;

        Debug.Log("현재 맞은 횟수: " + currentHits);

        if (currentHits >= requiredHits)
        {
            Die();
        }

        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        if (!isStunned)
        {
            StartCoroutine(HitStunRoutine());
        }

    }


    IEnumerator HitStunRoutine()
    {
        isStunned = true;
        anim.SetTrigger("Hit"); // [중요] 애니메이터에 "Hit" 트리거 필요

        yield return new WaitForSeconds(1.0f); // 1초 동안 멈춤 (시간은 조절 가능)

        isStunned = false;
    }

    void TryRoar()
    {
        // 이미 울고 있다면 중복 실행 방지
        if (isDead || isStunned || isWin || anim.GetCurrentAnimatorStateInfo(0).IsName("roar")) return;

        int chance = Random.Range(0, 100);
        if (chance < roarChance)
        {
            anim.SetTrigger("Roar");
            // 울기 시작하면 이동을 멈추기 위해 즉시 애니메이션 파라미터 조정
            if (audioSource != null && roarSound != null)
            {
                audioSource.PlayOneShot(roarSound);
            }
        }
    }

    void Die()
    {
        isDead = true;
        anim.SetTrigger("isdead");
        GetComponent<Collider>().enabled = false;
        Debug.Log("거인이 사망했습니다.");
        StartCoroutine(DisableAfterDeath());
        if (audioSource != null && dieSound != null)
        {
            audioSource.PlayOneShot(dieSound);
        }

        stoneSpawner.StopSpawning();
        endingManager.Win();
    }

    IEnumerator DisableAfterDeath()
    {
        yield return new WaitForSeconds(5f);

        gameObject.SetActive(false);
    }

    IEnumerator LoseRoutine()
    {
        yield return new WaitForSeconds(2f);

        endingManager.Lose();
    }

    public void ResetGiant()
    {
        isDead = false;
        isStunned = false;
        isWin = false;

        currentHits = 0;

        requiredHits = Random.Range(minHits, maxHits + 1);

        GetComponent<Collider>().enabled = true;

        Debug.Log("거인 초기화 완료");

        StopAllCoroutines();
    }
}