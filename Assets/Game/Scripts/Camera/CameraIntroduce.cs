using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CameraIntroduce : PersistentObject
{
    public float transitionDelay = 2.0f;
    public float duration = 5;
    public CinemachineVirtualCamera targetCamera;

    CinemachineVirtualCamera currentCamera;
    bool introduced = false;

    public void Transition()
    {
        if (introduced)
        {
            return;
        }
        introduced = true;
        currentCamera = CameraManager.ActiveCineCamera;
        StartCoroutine(StartTransition());
    }

    private IEnumerator StartTransition()
    {
        yield return new WaitForSeconds(transitionDelay);

        InputManager.Instance.TogglePlayerControls(false);
        CameraManager.Instance.ChangeVirtualCamera(targetCamera);
        yield return UIManager.Instance.gladeVillageIntroMenu.FadeIn(2.0f);

        yield return new WaitForSecondsRealtime(duration);

        yield return UIManager.Instance.gladeVillageIntroMenu.FadeOut(2.0f);
        CameraManager.Instance.ChangeVirtualCamera(currentCamera);
        InputManager.Instance.TogglePlayerControls(true);
    }

    public override byte[] GetSaveData()
    {
        using (var stream = new MemoryStream())
        {
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(introduced);
            }
            return stream.ToArray();
        }
    }

    public override void LoadSaveData(byte[] data)
    {
        using (var stream = new MemoryStream(data))
        {
            using (var reader = new BinaryReader(stream))
            {
                introduced = reader.ReadBoolean();
            }
        }
    }
}
