using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(XROrigin))]
public class CharacterSizeController : MonoBehaviour
{
    CharacterController character;
    XROrigin origin;
    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<CharacterController>();
        origin = GetComponent<XROrigin>();
    }

    // Update is called once per frame
    void Update()
    {
        character.center = new Vector3(Camera.main.transform.localPosition.x, origin.CameraInOriginSpaceHeight / 2, Camera.main.transform.localPosition.z);
        character.height = origin.CameraInOriginSpaceHeight;
    }
}
