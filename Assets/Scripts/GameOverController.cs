using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public void OnMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnPlayAgainButton()
    {
        SceneManager.LoadScene("MainScene");
    }
}
