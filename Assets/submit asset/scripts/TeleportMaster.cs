using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class TeleportMaster : MonoBehaviour
{
    [Header("Components")]
    public ParticleSystem teleportEffect; // 파티클 시스템
    public AudioSource audioSource;       // 사운드 (Play One Shot용)
    public AudioClip teleportSound;       // 효과음 파일

    [Header("Teleport Settings")]
    public Transform playerOrigin;        // XR Origin (또는 XRRig)
    public Transform destination;         // 오두막 내부 목적지

    [Header("Fade Settings")]
    public Image fadeImage;               // 아까 만든 FadeImage
    public float fadeDuration = 1.0f;     // 페이드 걸리는 시간

    private XRGrabInteractable grabInteractable;
    [SerializeField]
    public bool isTeleporting = false;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(StartTeleportSequence);
    }

    private void StartTeleportSequence(SelectEnterEventArgs args)
    {
        if (isTeleporting) return;
        StartCoroutine(TeleportRoutine());
    }

    IEnumerator TeleportRoutine()
    {
        isTeleporting = true;

        // 1. 파티클과 사운드 실행
        if (teleportEffect != null) teleportEffect.Play();
        if (audioSource != null && teleportSound != null)
            audioSource.PlayOneShot(teleportSound);

        // 2. 화면 페이드 아웃 (검게 변함)
        yield return StartCoroutine(Fade(0, 1));

        // 3. 실제 위치 이동
        playerOrigin.position = destination.position;
        playerOrigin.rotation = destination.rotation;

        // 잠시 대기 (도착 후 로딩 느낌)
        yield return new WaitForSeconds(0.5f);

        // 4. 화면 페이드 인 (다시 밝아짐)
        yield return StartCoroutine(Fade(1, 0));

        isTeleporting = false;

        // 돌을 사용했으니 사라지게 함 (선택 사항)
        gameObject.SetActive(false);
    }

    IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = endAlpha;
        fadeImage.color = color;
    }
}