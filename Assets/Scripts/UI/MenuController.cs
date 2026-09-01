using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
     private void Start()
    {
        Debug.Log("MenuController Start");
    }

    public void StartGame()
    {
        Debug.Log("===== START BUTTON =====");

        if(SceneTransition.Instance == null)
        {
            Debug.LogError("SceneTransition NULL");
            return;
        }

        Debug.Log("Load Village");

        SceneTransition.Instance.LoadScene("World_Village");
    }
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Closed");
    }
}
