using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI; 
public class TimelineManager : MonoBehaviour
{
    [Header("Riferimenti")]
    [SerializeField] private PlayableDirector director;
    [SerializeField] private Cinemachine.CinemachineVirtualCamera introVcam;
    [SerializeField] private Cinemachine.CinemachineVirtualCamera playerVcam;
    [SerializeField] private TimelinePlayerController playerInputController;

    [Header("UI per lo Skip")]
    [SerializeField] private GameObject skipPromptUI;
    [SerializeField] private Image fillMeterImage;

    [Header("Impostazioni Skip")]
    [SerializeField] private float timeToSkip = 1.5f;
    private float _skipTimer = 0f;
    private bool _canSkip = true;

    private void Awake()
    {
        if (director == null) director = GetComponent<PlayableDirector>();
        if (fillMeterImage != null) fillMeterImage.fillAmount = 0;
    }

    private void Update()
    {
        if (!_canSkip) return;

        if (Input.GetKey(KeyCode.X))
        {
            _skipTimer += Time.deltaTime;
            fillMeterImage.fillAmount = _skipTimer / timeToSkip;
            if (_skipTimer >= timeToSkip)
            {
                Skip();
            }
        }
        else 
        {
            _skipTimer = 0f;
            fillMeterImage.fillAmount = 0f;
        }
    }

    public void Skip()
    {
        if (!_canSkip) return;

        _canSkip = false; 

        Debug.Log("Timeline saltata!");

        director.Stop();
        if (playerInputController != null)
        {
            playerInputController.EnableControl();
        }

        if (skipPromptUI != null) skipPromptUI.SetActive(false);

        gameObject.SetActive(false);
    }
}