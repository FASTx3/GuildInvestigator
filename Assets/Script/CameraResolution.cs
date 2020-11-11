using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    public float _x_ratio = 16;
    public float _y_ratio = 9;

    float _scale_height;
    float _scale_width;
    void Awake()
    {
        Camera _camera = GetComponent<Camera>();
        Rect _rect = _camera.rect;
        
        if(_x_ratio > _y_ratio)
        {
            _scale_height = ((float)Screen.width / Screen.height) / (_x_ratio/_y_ratio);
            _scale_width = 1 / _scale_height;

            if(_scale_height < 1)
            {
                _rect.height = _scale_height;
                _rect.y = (1 - _scale_height) / 2f;
            }
            else
            {
                _rect.width = _scale_width;
                _rect.x = (1f - _scale_width) / 2f;
            }
        }
        else
        {
            _scale_width = ((float)Screen.height / Screen.width) / (_y_ratio/_x_ratio);
            _scale_height = 1 / _scale_width;

            if(_scale_width < 1)
            {
                _rect.width = _scale_width;
                _rect.x = (1 - _scale_width) / 2f;
            }
            else
            {
                _rect.height = _scale_height;
                _rect.y = (1f - _scale_height) / 2f;
            }
        }
        
        
        /*
        float _scale_height = ((float)Screen.width / Screen.height) / (16f/9f);
        float _scale_width = 1 / _scale_height;

        if(_scale_height < 1)
        {
            _rect.height = _scale_height;
            _rect.y = (1 - _scale_height) / 2f;
        }
        else
        {
            _rect.width = _scale_width;
            _rect.x = (1f - _scale_width) / 2f;
        }
        */
        _camera.rect = _rect;
    }
}
