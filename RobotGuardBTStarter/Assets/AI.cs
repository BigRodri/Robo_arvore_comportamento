using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Panda;

public class AI : MonoBehaviour
{   //Cria as variaveis de ação (Player, balas, barra de vida)
    public Transform player;
    public Transform bulletSpawn;
    public Slider healthBar;   
    public GameObject bulletPrefab;

    NavMeshAgent agent;
    //Garante a leitura do player com a NavMesh para a locomoção dos player
    public Vector3 destination; // The movement destination.
    public Vector3 target;      // The position to aim to.
   
    //Variaveis de vida e velocidade de rotação
    float health = 100.0f;
    float rotSpeed = 5.0f;
    //Cria o range de visibilidade e a distancia que o tiro chega
    float visibleRange = 80.0f;
    float shotRange = 40.0f;

    void Start()
    {   //Reconhece a Navmesh como meio de navegação e diminui a velocidade do player caso atire
        agent = this.GetComponent<NavMeshAgent>();
        agent.stoppingDistance = shotRange - 5; //for a little buffer
        InvokeRepeating("UpdateHealth",5,0.5f);
    }

    void Update()
    {   //Faz com que a HealthBar sempre acompanhe o personagem
        Vector3 healthBarPos = Camera.main.WorldToScreenPoint(this.transform.position);
        healthBar.value = (int)health;
        healthBar.transform.position = healthBarPos + new Vector3(0,60,0);
    }

    void UpdateHealth()
    {  //Faz a atualização da barra de vida
       if(health < 100)
        health ++;
    }

    void OnCollisionEnter(Collision col)
    {   //Defini a perda de vida conforme o hit da bala
        if(col.gameObject.tag == "bullet")
        {
            health -= 10;
        }
    }

    [Task]
    public void PickRandomDestination()
    {   //Gera um range dentro da navmesh que o npc consegue se mover
        Vector3 dest = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
        agent.SetDestination(dest);
        Task.current.Succeed();
    }

    [Task]
    public void MoveToDestination()
    {   //Faz com que o npc se mova até o destino que ele escolher
        if (Task.isInspected)
        {
            Task.current.debugInfo = string.Format("t={0:0.00}", Time.time);
        }
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            Task.current.Succeed();
        }
    }

}

