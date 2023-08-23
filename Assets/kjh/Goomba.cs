using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goomba : Enemy
{
    public GameObject stop;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();


        move = true;

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        TouchDown();



    }

    private void TouchDown()
    {

        if (IsSkyDetected())
        {

            move = false;

            Destroy(gameObject, 1);
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {


        if (collision.collider.gameObject.CompareTag("Enemy"))
        {


            transform.Rotate(0, 180, 0);


            moveflip = moveflip * -1;


        }
        else if (collision.collider.gameObject.CompareTag("Enemy_Shell") && false == Enemy_shell.fsecMove)//&& collision.collider.GetComponent<Rigidbody2D>().velocity == new Vector2(0, 0))
        {
            Debug.Log(collision.collider.GetComponent<Rigidbody2D>().velocity);
            transform.Rotate(0, 180, 0);


            moveflip = moveflip * -1;
        }
        else if (collision.collider.gameObject.CompareTag("Enemy_Shell"))
        {
            Destroy(gameObject);

            var a = Instantiate(stop, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);


            a.GetComponent<Rigidbody2D>().velocity = new Vector2(collision.transform.position.x, 0);






        }


    }
}
