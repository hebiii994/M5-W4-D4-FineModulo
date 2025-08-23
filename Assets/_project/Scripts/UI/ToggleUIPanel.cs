using UnityEngine;

public class ToggleUIPanel : MonoBehaviour
{
    [SerializeField] private GameObject _panelToToggle;

    [SerializeField] private KeyCode _toggleKey = KeyCode.F1;

    void Update()
    {
        if (Input.GetKeyDown(_toggleKey))
        {
            if (_panelToToggle != null)
            {
                _panelToToggle.SetActive(!_panelToToggle.activeSelf);
            }
        }
    }
}