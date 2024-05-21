using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorManager : Singleton<LevelEditorManager>
{
    bool readyToSpawn = false;
    GameObject goToSpawn = null;

    private void Update()
    {
        if (!readyToSpawn || goToSpawn == null)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseRay.origin, mouseRay.direction, out RaycastHit hitInfo, 100.0f))
            {
                Instantiate(goToSpawn, hitInfo.point, Quaternion.Euler(0.0f, -180.0f, 0.0f));
                readyToSpawn = false;
            }
        }
    }

    public void StandbyToSpawn(GameObject objPrefab)
    {
        readyToSpawn = true;
        goToSpawn = objPrefab;
    }
}
