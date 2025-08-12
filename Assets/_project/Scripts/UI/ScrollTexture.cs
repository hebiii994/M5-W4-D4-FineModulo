using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollTexture : MonoBehaviour
{
    [SerializeField] private float _scrollSpeedX = 0f;
    [SerializeField] private float _scrollSpeedY = 0.1f;

    private RawImage _rawImage;

    private void Awake()
    {
        _rawImage = GetComponent<RawImage>();
    }


    void Update()
    {
        Rect newUvRect = new Rect(
            _rawImage.uvRect.position + new Vector2(_scrollSpeedX, _scrollSpeedY) * Time.deltaTime,
            _rawImage.uvRect.size
        );

        _rawImage.uvRect = newUvRect;
    
}
}
