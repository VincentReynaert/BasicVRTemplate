using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WhiteboardMarker : MonoBehaviour
{
    [SerializeField] private Transform _tip;
    [SerializeField, Range(1, 16)] private int _penSize = 5;
    [SerializeField] private TipType tipType = TipType.Cube;
    private Renderer _renderer;
    private Color32[] _colors;
    private Color32 _color;
    private float _tipLength;
    private RaycastHit _touch;
    private Whiteboard _whiteboard;
    private Vector2 _touchPos;
    private bool _touchedLastFrame;
    private Vector2 _lastTouchPos;
    private Quaternion _lastTouchRot;

    private void Start()
    {
        _renderer = _tip.GetComponent<Renderer>();
        _color = _renderer.material.color;
        _colors = Enumerable.Repeat(_color, _penSize * _penSize).ToArray();
        switch (tipType)
        {
            case TipType.Sphere:
                _tipLength = _tip.localScale.z;
                break;
            case TipType.Cube:
                _tipLength = _tip.localScale.z * Mathf.Sqrt(3);
                break;
            default:
                _tipLength = 0.1f;
                break;
        }
    }
    private void Update()
    {
        DrawRay();
    }
    private void Draw()
    {
        if (Physics.SphereCast(_tip.position, _tipLength / 2, transform.up, out _touch))
        {
            if (_touch.transform.CompareTag("Whiteboard"))
            {
                if (_whiteboard == null)
                {
                    _whiteboard = _touch.transform.GetComponent<Whiteboard>();
                    _colors = _whiteboard._texture.GetPixels32();
                }

                _touchPos = new Vector2(_touch.textureCoord.x, _touch.textureCoord.y);
                int x = (int)(_touchPos.x * _whiteboard._textureSize.x); //- (_penSize / 2));
                int y = (int)(_touchPos.y * _whiteboard._textureSize.y); //- (_penSize / 2));

                if (y < 0 || y > _whiteboard._textureSize.y || x < 0 || x > _whiteboard._textureSize.x) return;

                if (_touchedLastFrame)
                {
                    //Debug.Log("SET" + Time.frameCount);
                    _colors[y * _whiteboard._textureSize.x + x] = _color;
                    //_whiteboard._texture.SetPixels32(x, y, _penSize, _penSize, _colors);
                    for (float f = 0.01f; f < 1.00f; f += 0.01f)
                    {
                        int lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x, f);
                        int lerpY = (int)Mathf.Lerp(_lastTouchPos.y, y, f);
                        _colors[lerpY * _whiteboard._textureSize.x + lerpX] = _color;
                        //_whiteboard._texture.SetPixels(lerpX, lerpY, _penSize, _penSize, _colors);

                        //_colors[lerpX * SIZE + lerpY] = f;
                    }
                    _whiteboard._texture.SetPixels32(_colors);
                    //transform.rotation = _lastTouchRot;
                    _whiteboard._texture.Apply();
                }
                _lastTouchPos = new Vector2(x, y);
                //_lastTouchRot = transform.rotation;
                _touchedLastFrame = true;
                return;
            }
        }
        _whiteboard = null;
        _touchedLastFrame = false;
    }
    private void DrawRay()
    {
        if (Physics.Raycast(_tip.position, transform.up, out _touch, _tipLength))
        {
            if (_touch.transform.CompareTag("Whiteboard"))
            {
                if (_whiteboard == null)
                {
                    _whiteboard = _touch.transform.GetComponent<Whiteboard>();
                    _colors = _whiteboard._texture.GetPixels32();
                }

                _touchPos = new Vector2(_touch.textureCoord.x, _touch.textureCoord.y);
                int x = (int)(_touchPos.x * _whiteboard._textureSize.x); //- (_penSize / 2));
                int y = (int)(_touchPos.y * _whiteboard._textureSize.y); //- (_penSize / 2));

                if (y < 0 || y > _whiteboard._textureSize.y || x < 0 || x > _whiteboard._textureSize.x) return;

                if (_touchedLastFrame)
                {
                    //_colors[y * _whiteboard._textureSize.x + x] = _color;
                    MarkPixelsToColour(new Vector2(x, y), _penSize, _color);
                    for (float f = 0.01f; f < 1.00f; f += 0.01f)
                    {
                        int lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x, f);
                        int lerpY = (int)Mathf.Lerp(_lastTouchPos.y, y, f);
                        //_colors[lerpY * _whiteboard._textureSize.x + lerpX] = _color;
                        MarkPixelsToColour(new Vector2(lerpX, lerpY), _penSize, _color);

                    }
                    _whiteboard._texture.SetPixels32(_colors);
                    _whiteboard._texture.Apply();
                }
                _lastTouchPos = new Vector2(x, y);
                _touchedLastFrame = true;
                return;
            }
        }
        _whiteboard = null;
        _touchedLastFrame = false;
    }

    public void MarkPixelsToColour(Vector2 center_pixel, int pen_thickness, Color color_of_pen)
    {
        // Figure out how many pixels we need to colour in each direction (x and y)
        int center_x = (int)center_pixel.x;
        int center_y = (int)center_pixel.y;
        //int extra_radius = Mathf.Min(0, pen_thickness - 2);

        for (int x = center_x - pen_thickness; x <= center_x + pen_thickness; x++)
        {
            // Check if the X wraps around the image, so we don't draw pixels on the other side of the image
            if (x >= (int)_whiteboard._textureSize.x || x < 0)
                continue;

            for (int y = center_y - pen_thickness; y <= center_y + pen_thickness; y++)
            {
                MarkPixelToChange(x, y, color_of_pen);
            }
        }
    }
    public void MarkPixelToChange(int x, int y, Color color)
    {
        // Need to transform x and y coordinates to flat coordinates of array
        int array_pos = y * (int)_whiteboard._textureSize.x + x;

        // Check if this is a valid position
        if (array_pos > _colors.Length || array_pos < 0)
            return;

        _colors[array_pos] = color;
    }

}

public enum TipType
{
    Sphere, Cube
}