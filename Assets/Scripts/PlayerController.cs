using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject bulletObjA;
    Transform frontPosition;
    Animator anim;

    public int playerID;
    int stamina;
    int attackPower;
    float playerSpeed;
    float bulletSpeed;
    float maxShotDelay;
    float curShotDelay;

    bool isTouchTop;
    bool isTouchBottom;
    bool isTouchRight;
    bool isTouchLeft;

    float xLength;
    float yLength;

    void Start()
    {
        anim = GetComponent<Animator>();
        frontPosition = transform.Find("FrontPosition");

        stamina = 5;
        attackPower = 1;
        playerSpeed = 3;
        bulletSpeed = 10;
        maxShotDelay = 0.2f;
        curShotDelay = 0;
    }

    void Update()
    {
        IsDead();
        SetLength();
        Move();
        Fire();
        Reload();
    }

    void IsDead()
    {
        if (stamina <= 0)
            Destroy(gameObject);
    }

    void SetLength()
    {
        xLength = frontPosition.position.x - transform.position.x;
        yLength = frontPosition.position.y - transform.position.y;
    }

    void Move()
    {
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");

        if ((isTouchTop && v == 1) || (isTouchBottom && v == -1))
            v = 0;

        transform.position += (new Vector3(v * xLength, v * yLength, 0) * playerSpeed * Time.deltaTime);
        transform.Rotate(0, 0, h * playerSpeed * -30 * Time.deltaTime);

        if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal"))
            anim.SetInteger("Input", (int)h);
    }

    void Fire()
    {
        if (Input.GetButton("Fire1") && curShotDelay >= maxShotDelay)
        {
            GameObject bullet = Instantiate(bulletObjA, transform.position, transform.rotation);
            bullet.GetComponent<Bullet>().Ready(attackPower, playerID);
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            rigid.AddForce(new Vector2(xLength, yLength) * bulletSpeed, ForceMode2D.Impulse);

            curShotDelay = 0;
        }
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch(collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = true;
                    break;
                case "Bottom":
                    isTouchBottom = true;
                    break;
                case "Right":
                    isTouchRight = true;
                    break;
                case "Left":
                    isTouchLeft = true;
                    break;
            }
        }
        else if (collision.gameObject.tag == "Bullet")
        {
            if(collision.gameObject.GetComponent<Bullet>().GetPlayerID() != playerID)
            {
                stamina--;
                Destroy(collision.gameObject);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = false;
                    break;
                case "Bottom":
                    isTouchBottom = false;
                    break;
                case "Right":
                    isTouchRight = false;
                    break;
                case "Left":
                    isTouchLeft = false;
                    break;
            }
        }
    }
}