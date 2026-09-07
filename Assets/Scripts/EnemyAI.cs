using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    //Initialises variables
    private Transform Player;
    private float playerDistance;
    public float detectRadius;
    public float fireRate;
    public float projectileSpeed;
    private float nextFireTime;
    private Transform enemy;
    public Transform shootPoint;
    public GameObject projectile;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Gets position of player and defines enemy position
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        enemy = enemy.transform;
    }

    // Update is called once per frame
    void Update()
    {
        //Gets distance from enemy and player, and looks at player when within detectRadius
        playerDistance = Vector3.Distance(Player.position, transform.position);
        if (playerDistance <= detectRadius)
        {
            transform.LookAt(Player);

            //Fire rate created by shooting after amount of time has passed
            if(Time.time > nextFireTime)
            {
                enemyShoot();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    void enemyShoot()
    {
        //Creates a clone of the projectile prefab at the shootPoint of enemy and adds force to propel
        GameObject firedProjectile = Instantiate(projectile, shootPoint.position, transform.rotation);
        firedProjectile.GetComponent<Rigidbody>().AddForce(transform.forward * projectileSpeed);

    }

}
