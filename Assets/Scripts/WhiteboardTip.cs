using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WhiteboardTip : MonoBehaviour
{
    [SerializeField, Range(1, 10)] private int _penSize = 5;
    private Renderer _renderer;
    private Color[] _colors;
    private Whiteboard _whiteboard;
    private Vector2Int _textureSize;
    private Vector2 _touchPos;
    private bool _touchedLastFrame;
    private Vector2 _lastTouchPos;
    private Quaternion _lastTouchRot;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        _colors = Enumerable.Repeat(_renderer.material.color, _penSize * _penSize).ToArray();
    }
    private void OnCollisionEnter(Collision collision)
    {
        print("touch :"+collision.gameObject.name);
        if (!collision.collider.CompareTag("Whiteboard")) return;
        if (_whiteboard == null)
        {
            _whiteboard = collision.transform.GetComponent<Whiteboard>();
            _textureSize = _whiteboard._textureSize;
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (!collision.collider.CompareTag("Whiteboard")) return;
        Draw(collision);

    }
    private void OnCollisionExit(Collision collision)
    {
        if (!collision.collider.CompareTag("Whiteboard")) return;
        _whiteboard = null;
        _touchedLastFrame = false;
    }

    private void Draw(Collision collision)
    {
        ContactPoint contactPoint = collision.GetContact(0);

        _touchPos = collision.transform.position - contactPoint.point;
        int x = (int)(_touchPos.x * _textureSize.x - (_penSize / 2));
        int y = (int)(_touchPos.y * _textureSize.y - (_penSize / 2));

        if (y < 0 || y > _textureSize.y || x < 0 || x > _textureSize.x) return;

        if (_touchedLastFrame)
        {

            var texture = _whiteboard._texture;
            texture.SetPixels(x, y, _penSize, _penSize, _colors);
            for (float f = 0.01f; f < 1.00f; f += 0.03f)
            {
                int lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x, f);
                int lerpY = (int)Mathf.Lerp(_lastTouchPos.y, y, f);
                texture.SetPixels(lerpX, lerpY, _penSize, _penSize, _colors);
            }
            transform.rotation = _lastTouchRot;
            texture.Apply();
        }
        _lastTouchPos = new Vector2(x, y);
        _lastTouchRot = transform.rotation;
        _touchedLastFrame = true;
        return;
    }
}
