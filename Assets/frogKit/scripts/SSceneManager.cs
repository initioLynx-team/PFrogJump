using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SSceneManager : MonoBehaviour
{



    public String MenuName;
    public String Dialogue1;

    public void LoadMenu()
    {
        LoadNextScene(MenuName);
    }

    public void LoadDialogue1()
    {
        LoadNextScene(Dialogue1);
    }

    public void LoadNextScene(string sceneName)
    {
        StartCoroutine(TransitionRoutine(sceneName));
    }
    IEnumerator TransitionRoutine(string sceneName)
    {
        yield return new WaitForSeconds(0.1f);

        SceneManager.LoadScene(sceneName);
    }
}
