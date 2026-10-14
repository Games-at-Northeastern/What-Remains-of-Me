using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class MainMenuIdleVideo : MonoBehaviour
{
    [SerializeField] private VideoClip[] clips;
    [SerializeField] private RawImage img;
    [SerializeField] private Image fadeImage;
    [SerializeField] private CanvasGroup menuCanvas;
    [SerializeField] private float inactiveThreshold;
    [SerializeField] private float fadeTime;
    private RenderTexture rt;
    private VideoPlayer videoPlayer;
    private float lastInputTime;
    private Coroutine activeRoutine;
    private CancellationTokenSource cancelTransition;
    private bool transitioning = false;


    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        rt = RenderTexture.GetTemporary(Screen.width, Screen.height);
        videoPlayer.targetTexture = rt;
        img.texture = rt;

        img.gameObject.SetActive(false);
        fadeImage.gameObject.SetActive(false);
        menuCanvas.alpha = 1f;

        lastInputTime = Time.time;
    }

    void OnEnable()
    {
        InputSystem.onEvent += OnInput;
    }

    void OnDisable()
    {
        InputSystem.onEvent -= OnInput;
        if (activeRoutine != null)
            StopCoroutine(activeRoutine);
        if (cancelTransition != null)
            cancelTransition.Cancel();
    }

    void Start()
    {
        activeRoutine = StartCoroutine(IdleMenu());
    }

    IEnumerator IdleMenu()
    {
        float remainingTime = Time.time - lastInputTime;
        while (remainingTime < inactiveThreshold)
        {
            yield return new WaitForSeconds(remainingTime);
            remainingTime = Time.time - lastInputTime;
        }
        cancelTransition?.Cancel();
        cancelTransition = new();
        TransitionToVideo(cancelTransition.Token);
    }

    IEnumerator IdleVideo()
    {
        float time = 0f;
        while (videoPlayer.isPlaying
            && time < inactiveThreshold
            && Time.time - lastInputTime < inactiveThreshold)
        {
            yield return null;
            time += Time.deltaTime;
        }
        cancelTransition?.Cancel();
        cancelTransition = new();
        TransitionToMenu(cancelTransition.Token);
    }

    async void TransitionToVideo(CancellationToken token)
    {
        transitioning = true;
        await FadeIn(token);
        if (!token.IsCancellationRequested)
        {
            menuCanvas.alpha = 0f;
            PlayRandom();
            await FadeOut(token);
        }
        if (!token.IsCancellationRequested)
        {
            lastInputTime = Time.time;
            activeRoutine = StartCoroutine(IdleVideo());
        }
        transitioning = false;
    }

    async void TransitionToMenu(CancellationToken token)
    {
        transitioning = true;
        await FadeIn(token);
        if (!token.IsCancellationRequested)
        {
            videoPlayer.Stop();
            img.gameObject.SetActive(false);
            menuCanvas.alpha = 1f;
            await FadeOut(token);
        }
        if (!token.IsCancellationRequested)
        {
            lastInputTime = Time.time;
            activeRoutine = StartCoroutine(IdleMenu());
        }
        transitioning = false;
    }

    void TransitionToMenuImmediate()
    {
        videoPlayer.Stop();
        img.gameObject.SetActive(false);
        fadeImage.gameObject.SetActive(false);
        menuCanvas.gameObject.SetActive(true);
        menuCanvas.alpha = 1f;
        activeRoutine = StartCoroutine(IdleMenu());
    }

    void PlayRandom()
    {
        int randomIdx = Random.Range(0, clips.Length);
        videoPlayer.clip = clips[randomIdx];
        videoPlayer.Play();
        img.gameObject.SetActive(true);
    }

    void OnInput(InputEventPtr ptr, InputDevice device)
    {
        lastInputTime = Time.time;
        if (transitioning || menuCanvas.alpha < 1f || !menuCanvas.gameObject.activeSelf)
        {
            cancelTransition?.Cancel();
            StopCoroutine(activeRoutine);
            TransitionToMenuImmediate();
        }
    }

    private async Task FadeIn(CancellationToken token)
    {
        SetAlpha(fadeImage, 0f);
        float passed = 0f;
        fadeImage.gameObject.SetActive(true);

        while (fadeImage.color.a < 1f && !token.IsCancellationRequested)
        {
            Color c = fadeImage.color;
            float alpha = Mathf.Lerp(0f, 1f, passed / fadeTime);
            SetAlpha(fadeImage, alpha);

            passed += Time.deltaTime;
            await Task.Yield();
        }
    }

    private async Task FadeOut(CancellationToken token)
    {
        SetAlpha(fadeImage, 1f);
        float passed = 0f;
        fadeImage.gameObject.SetActive(true);

        while (fadeImage.color.a > 0f && !token.IsCancellationRequested)
        {
            float alpha = Mathf.Lerp(1f, 0f, passed / fadeTime);
            SetAlpha(fadeImage, alpha);

            passed += Time.deltaTime;
            await Task.Yield();
        }
    }

    static void SetAlpha(Image img, float a)
    {
        Color c = img.color;
        img.color = new(c.r, c.g, c.b, a);
    }
}
