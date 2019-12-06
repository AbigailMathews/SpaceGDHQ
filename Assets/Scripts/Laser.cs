using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{

    [SerializeField]
    float speed = 10f;

	public bool IsEnemyLaser { get; set; }

    Renderer _renderer;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
		if (IsEnemyLaser) {
			transform.Translate (Vector3.down * speed * Time.deltaTime);
		} else {
			transform.Translate (Vector3.up * speed * Time.deltaTime);
		}

        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject, 1.5f);
        }

        Destroy(gameObject, 1.5f);
    }
}
