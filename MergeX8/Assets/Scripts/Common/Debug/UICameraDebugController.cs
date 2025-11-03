using System;
using DragonU3DSDK;
using Framework;
using Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICameraDebugController : UIWindowController
{
    private bool _cameraActive;
    private bool _cameraEnable;
    private Vector3 _cameraPos;
    private Quaternion _cameraRot;

    public override void PrivateAwake()
    {
        var cam = CameraManager.MainCamera;
        if (cam)
        {
            _cameraActive = cam.gameObject.activeSelf;
            _cameraEnable = cam.enabled;
            _cameraPos = cam.transform.position;
            _cameraRot = cam.transform.rotation;
            cam.gameObject.SetActive(true);
            cam.enabled = true;
        }

        BindLongPress("ButtonLeft", gameObject, o =>
        {
            var camera = CameraManager.MainCamera;
            if (camera)
            {
                camera.transform.Translate(-(Time.deltaTime), 0, 0, Space.Self);
            }
        });


        BindLongPress("ButtonRight", gameObject, o =>
        {
            var camera = CameraManager.MainCamera;
            if (camera)
            {
                camera.transform.Translate((Time.deltaTime), 0, 0, Space.Self);
            }
        });

        BindLongPress("ButtonForward", gameObject, o =>
        {
            var camera = CameraManager.MainCamera;
            if (camera)
            {
                camera.transform.Translate(0, 0, (Time.deltaTime), Space.Self);
            }
        });

        BindLongPress("ButtonBack", gameObject, o =>
        {
            var camera = CameraManager.MainCamera;
            if (camera)
            {
                camera.transform.Translate(0, 0, -(Time.deltaTime), Space.Self);
            }
        });

        BindLongPress("ButtonDown", gameObject, o =>
        {
            var camera = CameraManager.MainCamera;
            if (camera)
            {
                camera.transform.Translate(0, -(Time.deltaTime), 0, Space.Self);
            }
        });

        BindLongPress("ButtonUp", gameObject, o =>
        {
            var camera = CameraManager.MainCamera;
            if (camera)
            {
                camera.transform.Translate(0, (Time.deltaTime), 0, Space.Self);
            }
        });

        BindLongPress("ButtonRotateLeft", gameObject, o =>
        {
            var camera = CameraManager.MainCamera;
            if (camera)
            {
                camera.transform.Rotate(0, -(Time.deltaTime) * 100, 0, Space.Self);
            }
        });

        BindLongPress("ButtonRotateRight", gameObject, o =>
        {
            var camera = CameraManager.MainCamera;
            if (camera)
            {
                camera.transform.Rotate(0, (Time.deltaTime) * 100, 0, Space.Self);
            }
        });


        BindEvent("ButtonExit", gameObject, o =>
        {
            var camera = CameraManager.MainCamera;
            if (camera)
            {
                camera.transform.position = _cameraPos;
                camera.transform.rotation = _cameraRot;
                camera.gameObject.SetActive(_cameraActive);
                camera.enabled = _cameraEnable;
            }

            CloseWindowWithinUIMgr(true);
        });
    }


    private GameObject BindLongPress(string target, GameObject par, Action<GameObject> action)
    {
        GameObject obj = par.transform.Find(target).gameObject;
        if (obj != null)
        {
            var button = obj.GetComponent<ButtonLongPress>();
            if (button != null)
            {
                button.onPress.AddListener(() => action(obj));
            }
        }
        else
        {
            DebugUtil.LogError("未找到　" + gameObject.name + "/" + target);
        }

        return obj;
    }
}