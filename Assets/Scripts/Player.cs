using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

#region Variables
    //Player
    Animation anim;
    public float movementSpeed;
    public float turningSpeed;
    public float attackTimer;
    public float attackRange;
    public float minDamage;
    public float maxDamage;
    private float damage;
    private bool moving;
    private bool attacking;
    private bool attacked;
    private float currentAttackTimer;

    //PMR
    public GameObject playerMovePoint;
    private Transform pmr;
    private bool triggeringPMR;
    private bool followingEnemy;
    
    //Enemy Variables
    private bool triggeringEnemy;
    private GameObject _enemy;

#endregion

#region Core Game Functions
    void Start()
    {
        pmr = Instantiate(playerMovePoint.transform, this.transform.position, Quaternion.identity);
        currentAttackTimer = attackTimer;
        anim = GetComponent<Animation>();
    }

    void Update()
    {
        HandleClickToMove();
        HandleWASDMovement();
        HandleBasicAttack();
    }
    #endregion

#region Control Functions
    
    void HandleClickToMove()
    {
        //Player movement
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        float hitDistance = 0.0f;

        if (playerPlane.Raycast(ray, out hitDistance))
        {
            Vector3 mousePosition = ray.GetPoint(hitDistance);
            if (Input.GetMouseButtonDown(0))
            {
                moving = true;
                triggeringPMR = false;
                pmr.transform.position = mousePosition;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "Enemy")
                    {
                        _enemy = hit.collider.gameObject;
                        followingEnemy = true;
                    }
                }
                else
                {
                    _enemy = null;
                    followingEnemy = false;
                }
            }
        }
        if (followingEnemy && Input.GetKeyDown(KeyCode.Escape))
        {
            followingEnemy = false;
        }
        if (moving)
        {
            if (followingEnemy)
            {
                transform.position = Vector3.MoveTowards(transform.position, _enemy.transform.position - (new Vector3(attackRange,0,attackRange)), movementSpeed);
                this.transform.LookAt(_enemy.transform);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, pmr.transform.position, movementSpeed);
                this.transform.LookAt(pmr.transform);
                anim.CrossFade("ElfArcher_Walk");
            }
        }
        else
        {
            if (attacking)
            {
                HandleBasicAttack();
            }
            else
            anim.CrossFade("ElfArcher_Stand");
        }

        if (triggeringPMR)
        {
            moving = false;
        }
    }

    void HandleWASDMovement()
    {
        float horizontal = Input.GetAxis("Horizontal") * turningSpeed * Time.deltaTime;
        transform.Rotate(0, horizontal, 0);
        float vertical = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
        transform.Translate(0, 0, vertical);
        if (horizontal != 0 || vertical !=0)
        {
            anim.CrossFade("ElfArcher_Walk");
        }
        
    }

    void HandleBasicAttack()
    {
        if (attacked)
        {
            currentAttackTimer -= 1 * Time.deltaTime;
        }
        else
        {
            currentAttackTimer = attackTimer;
           
            if (triggeringEnemy)
            {
                damage = Random.Range(minDamage, maxDamage);
                print(damage);
                anim.CrossFade("ElfArcher_Attack1");
                transform.LookAt(_enemy.transform);
            }
            attacked = true;
        }
     
        if (currentAttackTimer <=0)
        {
            currentAttackTimer = attackTimer;
            attacked = false;
        }
    }
#endregion

#region Collision Handling
   
    
#endregion

#region Trigger Handling

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PMR")
        {
            triggeringPMR = true;
           
        }
        if (other.tag == "Enemy")
        {
            triggeringEnemy = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "PMR")
        {
            triggeringPMR = false;
        }
        if (other.tag == "Enemy")
        {
            triggeringEnemy = false;
        }
    }

    #endregion
}
