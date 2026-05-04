using UnityEngine;

public class SMusicBackground : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (FindObjectsByType<SMusicBackground>().Length > 1)
        {
            Destroy(gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
