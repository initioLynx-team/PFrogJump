using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AutoNextScene : MonoBehaviour
{
    [SerializeField] private string nextSceneName ="Swamp_Zona3";
    [SerializeField] private float waitTime=3f;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds (waitTime);

        SceneManager.LoadScene(nextSceneName);
    }

}
