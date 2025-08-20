using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    [SerializeField] private Transform _minBounds;
    [SerializeField] private Transform _maxBounds;

    public Vector2 MinBounds => new Vector2(_minBounds.position.x, _minBounds.position.z);
    public Vector2 MaxBounds => new Vector2(_maxBounds.position.x, _maxBounds.position.z);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}