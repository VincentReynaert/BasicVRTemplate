using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class WhiteboardTip : MonoBehaviour
{
    [SerializeField] private Transform _tip;
    [SerializeField, Range(1, 10)] private int _penSize = 5;
    private Renderer _renderer;
    private Color32[] _colors;
    private Color32 _color;
    private Whiteboard _whiteboard;
    private Vector2Int _textureSize;
    private Vector2 _touchPos;
    private bool _touchedLastFrame;
    private Vector2 _lastTouchPos;
    private Quaternion _lastTouchRot;

    private void Start()
    {
        _renderer = _tip.GetComponent<Renderer>();
        _color = _renderer.material.color;
        _colors = Enumerable.Repeat(_color, _penSize * _penSize).ToArray();
    }
    private void OnCollisionEnter(Collision collision)
    {
        print("touch :" + collision.gameObject.name);
        if (!collision.collider.CompareTag("Whiteboard")) return;
        if (_whiteboard == null)
        {
            _whiteboard = collision.transform.GetComponent<Whiteboard>();
            _colors = _whiteboard._texture.GetPixels32();
            _textureSize = _whiteboard._textureSize;
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (!collision.collider.CompareTag("Whiteboard")) return;
        if (_whiteboard != null) Draw(collision);

    }
    private void OnCollisionExit(Collision collision)
    {
        print("untouch :" + collision.gameObject.name);
        if (!collision.collider.CompareTag("Whiteboard")) return;
        _whiteboard = null;
        _touchedLastFrame = false;
    }

    private void Draw(Collision collision)
    {
        print("Draw");
        ContactPoint contactPoint = collision.GetContact(0);

        _touchPos = new Vector2(contactPoint.point.x, contactPoint.point.y);
        int x = (int)(_touchPos.x * _whiteboard._textureSize.x - (_penSize / 2));
        int y = (int)(_touchPos.y * _whiteboard._textureSize.y - (_penSize / 2));

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
