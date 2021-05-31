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
    [Task]
    public void PickDestination(int x, int z) 
    {   //Faz com que o detentor desse codigo escolhe um novo destino e va ate ele
        Vector3 dest = new Vector3(x, 0, z);
        agent.SetDestination(dest);
        Task.current.Succeed();

    }
    
    [Task]
    public void TargetPlayer()  
    {   //Faz com que o robo ande para onde ele decidir
        target = player.transform.position; Task.current.Succeed();
    }
    [Task]
    public bool Fire()  
    {   //Booleano para fazer com que o robo atire
        GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);   //instancia a bala como game object e faz ela spawnar varias vezes
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 2000);                                                 //Adiciona uma força ao componente da bala
        return true;                                                                                                                //Retorna verdadeiro e faz com que tudo acima aconteça
    }
    [Task]
    public void LookAtTarget()    
    {  //Faz com que o robo mire fixamente ao seu target
        Vector3 direction = target - this.transform.position;

        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);
        if (Task.isInspected)
            Task.current.debugInfo = string.Format("angle={0}", Vector3.Angle(this.transform.forward, direction)); 
        if (Vector3.Angle(this.transform.forward, direction) < 5.0f)
        {
            Task.current.Succeed();         
        }
    }
    [Task]
    bool SeePlayer()    
    {   //Faz com que o robo detentor do codigo, quando estiver no mesmo local que o player,
        //se retornar verdadeiro faz com que o robo pare de andar e cria ym raycast em direção ao player,
        //caso retorne falso, ele contina andando pelo mapa
        Vector3 distance = player.transform.position - this.transform.position;
        RaycastHit hit;
        bool SeePlayer = false;
        Debug.DrawRay(this.transform.position, distance, Color.red);    
        if (Physics.Raycast(this.transform.position, distance, out hit))
        {
            if (hit.collider.gameObject.tag == "wall")  
            {
                SeePlayer = true;
            }
        }
        if (Task.isInspected)    
            Task.current.debugInfo = string.Format("wall={0}", SeePlayer);

        if (distance.magnitude < visibleRange && !SeePlayer)
            return true;
        else
            return false;
    }
    [Task]
    bool Turn(float angle) 
    {
        var p = this.transform.position + Quaternion.AngleAxis(angle, Vector3.up) * this.transform.forward;
        target = p;
        return true;
    }
    [Task]
    public bool IsHealthLessThan(float health)      
    {
        return this.health < health;                        //atualiza a barra de vida
    }
    [Task]
    public bool Explode()                           
    {   //Quando chegar ao final da barra de vida o objeto é destruido
        Destroy(healthBar.gameObject);
        Destroy(this.gameObject);
        return true;
    }
}


