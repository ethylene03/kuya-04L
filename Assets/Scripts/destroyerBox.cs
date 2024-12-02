using UnityEngine;

public class destroyerBox : MonoBehaviour
{
    public string[] tags;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D col) {
        foreach(string t in tags) {
            if(col.gameObject.tag == t)
                Destroy(col.gameObject);
        }
    }
}
