using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelOneMovePlatform : MonoBehaviour
{
    private Animator an;
    private bool down = false;
    // Start is called before the first frame update
    void Start()
    {
        an = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag=="Hero"&&!down)
        {
            Invoke("MovePlat", 0.5f);
            down = true;
        }
    }
    void MovePlat()
    {
        down = false;
        an.SetTrigger("Down");
    }
}
