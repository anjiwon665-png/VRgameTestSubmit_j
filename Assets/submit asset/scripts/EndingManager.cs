using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    [Header("Ending UI")]
    public GameObject endingPanel;
    public TextMeshProUGUI resultText;

    [Header("Fade")]
    public Image fadeImage;

    [Header("Player")]
    public Transform player;
    public Transform battlePoint;

    [Header("Giant")]
    public GiantController giant;
    public Transform giantStartPoint;

    [Header("Stone")]
    public StoneSpawner stoneSpawner;

    [Header("Sound")]
    public AudioSource openingMusic;
    public AudioSource roarSound;

    bool isRestarting = false;

    // 승리
    public void Win()
    {
        endingPanel.SetActive(true);

        resultText.text = "YOU WIN!";
    }

    // 패배
    public void Lose()
    {
        endingPanel.SetActive(true);

        resultText.text = "YOU LOSE...";
    }

    // 재시작
    public void RestartGame()
    {
        if (isRestarting) return;
        StartCoroutine(RestartRoutine());
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator RestartRoutine()
    {
        isRestarting = true;

        endingPanel.SetActive(false);

        // 화면 암전
        yield return StartCoroutine(Fade(0, 1, 1f));

        // 플레이어 위치 초기화
        player.position = battlePoint.position;

        // 상태 초기화
        giant.ResetGiant();

        // 거인 초기화

        giant.transform.position =
        giantStartPoint.position;

        giant.transform.rotation =
        giantStartPoint.rotation;

        giant.anim.Rebind();
        giant.anim.Update(0f);


        // 돌 생성 다시 시작
        stoneSpawner.StartSpawning();

        // 음악

        openingMusic.Stop();
        openingMusic.Play();
        

        // 포효
        roarSound.Play();

        yield return new WaitForSeconds(1f);

        // 화면 복귀
        yield return StartCoroutine(Fade(1, 0, 1f));

        isRestarting = false;
    }

    public void EndGame()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif

        Debug.Log("게임 종료");
    }

    IEnumerator Fade(float start, float end, float time)
    {
        float t = 0;

        Color c = fadeImage.color;

        while (t < time)
        {
            t += Time.deltaTime;

            c.a = Mathf.Lerp(start, end, t / time);

            c.a = end;
            fadeImage.color = c;

            yield return null;
        }
    }
}