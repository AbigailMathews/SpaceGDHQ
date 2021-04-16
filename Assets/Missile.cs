using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField]
    float speed = 20f;

    Renderer _renderer;

    Enemy targetShip;

    float step;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();

        // find nearest enemy
        targetShip = FindClosestEnemy();
        Debug.Log("Created a missle with target", targetShip);
    }

    // Update is called once per frame
    void Update()
    {

        step = speed * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, targetShip.transform.position, step);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetShip.transform.rotation, step);

        Debug.Log(transform.rotation);

        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject, 1.5f);
        }

        Destroy(gameObject, 1.5f);
    }

    Enemy FindClosestEnemy()
    {
        Enemy target = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (Enemy enemy in Enemy.Enemies)
        {
            Vector3 diff = enemy.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                target = enemy;
                distance = curDistance;
            }
        }
        return target;
    }
}
