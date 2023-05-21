using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private MovimentControler player;

    new private Rigidbody2D rigidbody;

    public float speed = 0.2f;

    private Vector2 dir;

    public int hitPoint = 1;

    public bool solidEnemy = true;

    

    // Start is called before the first frame update
    void Start()
    {
        
        
        player = GameObject.FindWithTag("Player").GetComponent<MovimentControler>();
        rigidbody = GetComponent<Rigidbody2D>();

        if (solidEnemy == true)
        {
            GetComponent<Rigidbody2D>(); 
        }
        
         
    }

    // Update is called once per frame
    void Update()
    {
        if (player.getStage() == 1 || player.getStage() == 2)
        {
            dir = (player.transform.position - this.transform.position).normalized;

            rigidbody.MovePosition(this.transform.position + (Vector3)dir * speed * Time.deltaTime);
        }
        

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(this.transform.position, this.transform.position + (Vector3)dir);
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        hitPoint -= 1;
        Debug.Log(hitPoint);
        if(hitPoint <= 0)
        {
            Destroy(this.gameObject);
        }
        
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        hitPoint -= 1;

        if (hitPoint <= 0)
        {
            Destroy(this.gameObject);
        }

    }


}
