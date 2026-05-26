using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class SSceneTp : MonoBehaviour
{

#if UNITY_EDITOR
    public UnityEditor.SceneAsset sceneAsset;
#endif

    [HideInInspector]
    public string sceneName;



    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                LoadNextScene(sceneName);
            }
        }
    }
    public void LoadNextScene(string sceneName)
    {
        StartCoroutine(TransitionRoutine(sceneName));
    }

    IEnumerator TransitionRoutine(string sceneName)
    {
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(sceneName);
    }
    private void OnValidate()
    {
#if UNITY_EDITOR
        if (sceneAsset != null)
        {
            sceneName = sceneAsset.name;
            Debug.Log(sceneName);
        }
#endif
    }
}
