using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx.Async;

public class GameRetryWindow : MonoBehaviour
{
    //------------------------------------------------------------------
    // public
    //------------------------------------------------------------------
    public async UniTask<bool> Open()
    {
        gameObject.SetActive(true);
        return true;
    }

    public void RetryGame()
    {
        var nowSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(nowSceneName);
    }

    public void QuitGame()
    {
        GameQuit();
    }

    //------------------------------------------------------------------
    // private
    //------------------------------------------------------------------
    void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
        UnityEngine.Application.Quit();
#endif
    }


}
