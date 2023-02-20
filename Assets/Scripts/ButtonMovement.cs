using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonMovement : MonoBehaviour
{
    private int _tileType;
    private MapGenerator _mapGenerator;
    private Image _image;
    private float _alfaChange = 0.1f;
    private float _minAlfa = 0.5f;
    private float _maxAlfa = 1f;
    
    private bool _directionChange = false;
    private void Awake()
    {
        _mapGenerator = FindObjectOfType<MapGenerator>();
        _image = GetComponent<Image>();
        _tileType = (int) Enum.Parse(typeof(TileTypes.Types), gameObject.tag);
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ChangeButtonSize());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator ChangeButtonSize()
    {
        yield return new WaitForEndOfFrame();
        while(true)
        {
            if (_mapGenerator.GetButtonSelection() == _tileType && !_mapGenerator.HasMoleBeenSelected())
            {
                Color newColor = _image.color;
                if (newColor.a - _alfaChange <= _minAlfa)
                {
                    _directionChange = true;
                }

                if (newColor.a + _alfaChange >= _maxAlfa)
                {
                    _directionChange = false;
                }
            
                switch (_directionChange)
                {
                    case false:
                        newColor.a -= _alfaChange;
                        _image.color = newColor;
                        break;
                    case true:
                        newColor.a += _alfaChange;
                        _image.color = newColor;
                        break;
                }
            }
            else
            {
                Color newColor = _image.color;
                newColor.a = 1;
                _image.color = newColor;
                yield return null;
            }

            yield return new WaitForSeconds(0.2f);
        }
    }
}
