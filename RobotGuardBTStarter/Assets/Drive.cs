using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour {
    //Cria variaveis de velocidade e rotação
	float speed = 20.0F;
    float rotationSpeed = 120.0F;
    //Reconhece a bala como prefab e spawna varias balas a partir desse prefab
    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    void Update() {
        //Controla a movimentação do player
        float translation = Input.GetAxis("Vertical") * speed;
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;
        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);

        //Define a tecla "espaço" para ser a tecla que atira 
        if(Input.GetKeyDown("space"))
        {
            GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward*2000);
        }
    }
}
