using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class Whiteboard : MonoBehaviour
//{
//    public Vector2Int _textureSize = new Vector2Int(2048, 2048);
//    public Texture2D _texture;
//    // Start is called before the first frame update
//    void Start()
//    {
//        Renderer renderer = GetComponent<Renderer>();
//        _texture = new Texture2D(_textureSize.x, _textureSize.y);
//        renderer.material.mainTexture = _texture;
//    }


//}
public class Whiteboard : MonoBehaviour
{
    public Vector2Int _textureSize = new Vector2Int(512,512*16/9);
    public Texture2D _texture;
    private Renderer _renderer;

    private void Start()
    {
        InitializeTexture();
    }

    private void InitializeTexture()
    {
        _texture = new Texture2D(_textureSize.x, _textureSize.y);
        _texture.filterMode = FilterMode.Trilinear;
        _texture.wrapMode = TextureWrapMode.Clamp;
        _texture.SetPixels32(new Color32[_textureSize.x * _textureSize.y]);
        _texture.Apply();

        _renderer = GetComponent<Renderer>();
        _renderer.material.mainTexture = _texture;
    }

    public void Clear()
    {
        _texture.SetPixels32(new Color32[_textureSize.x * _textureSize.y]);
        _texture.Apply();
    }

    public void Save(string filename)
    {
        byte[] bytes = _texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(filename, bytes);
    }

    public void ChangeSize(Vector2Int newSize)
    {
        _textureSize = newSize;
        InitializeTexture();
    }
}