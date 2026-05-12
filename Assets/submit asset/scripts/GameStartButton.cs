using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameStartButton : MonoBehaviour
{
    [Header("UI")]
    public Image fadeImage;

    public GameObject startText;
    public GameObject tutorialText;

    [Header("Player")]
    public Transform player;
    public Transform battlePoint;

    [Header("Game")]
    public GameObject giantSpawner;
    public GameObject stoneSpawner;

    [Header("Sound")]
    public AudioSource openingMusic;
    public AudioSource roarSound;

    bool started = false;

    public void StartGame()
    {
        if (started) return;

        started = true;

        StartCoroutine(GameRoutine());
    }

    IEnumerator GameRoutine()
    {
        // 화면 검게
        yield return StartCoroutine(Fade(0, 1, 1f));

        // 음악 시작
        openingMusic.Play();

        // 포효
        roarSound.Play();

        // 플레이어 이동
        player.position = battlePoint.position;

        yield return new WaitForSeconds(1f);

        // 화면 밝게
        yield return StartCoroutine(Fade(1, 0, 1f));

        // 게임 스타트
        startText.SetActive(true);

        yield return new WaitForSeconds(2f);

        startText.SetActive(false);

        // 튜토리얼
        tutorialText.SetActive(true);

        yield return new WaitForSeconds(3f);

        tutorialText.SetActive(false);

        // 거인 생성 시작
        giantSpawner.SetActive(true);

        // 돌 생성 시작
        stoneSpawner.SetActive(true);
    }

    IEnumerator Fade(float start, float end, float time)
    {
        float t = 0;

        Color c = fadeImage.color;

        while (t < time)
        {
            t += Time.deltaTime;

            c.a = Mathf.Lerp(start, end, t / time);

            fadeImage.color = c;

            yield return null;
        }
    }
}