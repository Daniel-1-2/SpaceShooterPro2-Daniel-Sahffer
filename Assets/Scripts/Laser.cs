using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;

    // Update is called once per frame
    void Update(){
        if(this.gameObject.tag == "Laser"){
            MoveUp();
        }
        else if(this.tag == "EnemyLaser"){
            MoveDown();
        }
    }

    void MoveUp(){
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
        if (transform.position.y >= 8)
        {
            if (this.transform.parent != null)
            {
                Destroy(this.transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }

    void MoveDown(){
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y <= -8)
        {
            if (this.transform.parent != null)
            {
                Destroy(this.transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }
}
