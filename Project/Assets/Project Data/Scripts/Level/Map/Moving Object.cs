using UnityEngine;

public class MovingObject : MonoBehaviour
{

    public int speed;
    public float switchTime;
    private float timer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer += switchTime;
    }

    // Update is called once per frame
    void Update()
    {
        
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            speed = -speed;
            timer += switchTime;
        }
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
