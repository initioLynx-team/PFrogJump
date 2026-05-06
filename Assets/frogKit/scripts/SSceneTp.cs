using UnityEngine;

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
                SceneTransition st = FindAnyObjectByType<SceneTransition>();
                if (st != null)
                {
                    st.LoadNextScene(sceneName);
                }
                else
                {
                    Debug.LogError("SceneTransition not found");
                }
            }
        }
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (sceneAsset != null)
        {
            sceneName = sceneAsset.name;
        }
#endif
    }
}
