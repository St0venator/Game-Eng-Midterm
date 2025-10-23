using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballSpawner : MonoBehaviour
{
    public GameObject ball;
    public int bounds;
    public int offset;
    public int spacing;
    public int lifetime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            spawnBalls();

        }
    }

    void spawnBalls()
    {
        for (int x = offset; x < bounds; x += spacing)
        {
            for (int z = offset; z < bounds; z += spacing)
            {
                GameObject newBall = Instantiate(ball, new Vector3(x, 250, z), Quaternion.identity);

                Destroy(newBall, lifetime);
            }
        }
    }
}
