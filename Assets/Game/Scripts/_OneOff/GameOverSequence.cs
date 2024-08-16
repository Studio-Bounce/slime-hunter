using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverSequence : MonoBehaviour
{
    public void GameOver()
    {
        UIManager.Instance.ShowEndScreen();
    }
}
