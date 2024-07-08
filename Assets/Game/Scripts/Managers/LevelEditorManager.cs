using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEditorManager : Singleton<LevelEditorManager>
{
    bool readyToSpawn = false;
    GameObject goToSpawn = null;
    GameObject previewGO = null;

    public string sceneName;

    private void Update()
    {
        if (!readyToSpawn || goToSpawn == null)
            return;

        // Click -> Spawn the object
        if (Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseRay.origin, mouseRay.direction, out RaycastHit hitInfo, 100.0f))
            {
                GameObject slimeGO = Instantiate(goToSpawn, hitInfo.point, Quaternion.Euler(0.0f, -180.0f, 0.0f));
                SceneManager.MoveGameObjectToScene(slimeGO, SceneManager.GetSceneByName(sceneName));
                if (previewGO != null)
                    Destroy(previewGO);
                readyToSpawn = false;
            }
        }
    }

    public void StandbyToSpawn(GameObject objPrefab, GameObject objPreviewPrefab)
    {
        readyToSpawn = true;
        goToSpawn = objPrefab;

        // Show the image at the mouse position
        previewGO = Instantiate(objPreviewPrefab, Vector3.zero, Quaternion.Euler(0.0f, -180.0f, 0.0f));
        SceneManager.MoveGameObjectToScene(previewGO, SceneManager.GetSceneByName(sceneName));
    }
}
