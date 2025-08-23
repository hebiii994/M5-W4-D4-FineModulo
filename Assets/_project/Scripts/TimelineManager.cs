using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI; 
public class TimelineManager : MonoBehaviour
{
    [Header("Riferimenti")]
    [SerializeField] private PlayableDirector _director;
    [SerializeField] private Cinemachine.CinemachineVirtualCamera _introVcam;
    [SerializeField] private Cinemachine.CinemachineVirtualCamera _playerVcam;
    [SerializeField] private TimelinePlayerController _playerInputController;

    [Header("UI per lo Skip")]
    [SerializeField] private GameObject _skipPromptUI;
    [SerializeField] private Image _fillMeterImage;
    [SerializeField] private GameObject _keyPanel;

    [Header("Impostazioni Skip")]
    [SerializeField] private float _timeToSkip = 1.5f;
    private float _skipTimer = 0f;
    private bool _canSkip = true;

    private void Awake()
    {
        if (_director == null) _director = GetComponent<PlayableDirector>();
        if (_fillMeterImage != null) _fillMeterImage.fillAmount = 0;
    }

    private void Update()
    {
        if (!_canSkip) return;

        if (Input.GetKey(KeyCode.X))
        {
            _skipTimer += Time.deltaTime;
            _fillMeterImage.fillAmount = _skipTimer / _timeToSkip;
            if (_skipTimer >= _timeToSkip)
            {
                Skip();
            }
        }
        else 
        {
            _skipTimer = 0f;
            _fillMeterImage.fillAmount = 0f;
        }
    }

    public void Skip()
    {
        if (!_canSkip) return;

        _canSkip = false; 

        Debug.Log("Timeline saltata!");

        _director.Stop();
        if (_playerInputController != null)
        {
            _playerInputController.EnableControl();
        }
        if (_playerVcam != null)
        {
            _playerVcam.m_Lens.NearClipPlane = -2.0f;
        }
        if ( _keyPanel != null)
        {
            _keyPanel.SetActive(true);
        }

        if (_skipPromptUI != null) _skipPromptUI.SetActive(false);

        gameObject.SetActive(false);
    }
}