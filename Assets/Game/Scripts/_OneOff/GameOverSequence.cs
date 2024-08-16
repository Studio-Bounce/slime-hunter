using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverSequence : MonoBehaviour
{
    public void GameOver()
    {
        Debug.Log("Game is over, doing something...");
        StartCoroutine(ExitGame());
    }

    IEnumerator ExitGame()
    {
        yield return new WaitForSeconds(10);

        UnityEditor.EditorApplication.isPlaying = false;
    }
}
