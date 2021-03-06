using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

[RequireComponent(typeof(Sight))]
public class EnemyFSM : MonoBehaviour
{
   public enum EnemyState
   {
      GoToBase, AttackBase, ChasePlayer, AttackPlayer 
   }

   public EnemyState currentState;

   private Sight _sight;
   
   private Transform baseTransform;

   public float baseAttackDistance, playerAttackDistance;

   private NavMeshAgent agent;

   private Animator animator;

   public Weapon weapon;
   

   

   private void Awake()
   {
      _sight = GetComponent<Sight>();
      baseTransform = GameObject.FindWithTag("Base").transform;
      agent = GetComponentInParent<NavMeshAgent>();
      animator = GetComponentInParent<Animator>();
   }

   private void Update()
   {

      switch (currentState)
      {
         case  EnemyState.GoToBase:
            GoToBase();
            break;
         case EnemyState.AttackBase:
            AttackBase();
            break;
         case  EnemyState.ChasePlayer:
            ChasePlayer();
            break;
         case EnemyState.AttackPlayer:
            AttackPlayer();
            break;
         
         default:
            break;
         
      }
      
      
   }

   void GoToBase()
   {
      animator.SetBool("Shot Bullet Bool", false);
      //print("Ir a base");
      agent.isStopped = false;
      agent.SetDestination(baseTransform.position);
     

      if (_sight.detectedTarget != null)
      {
         currentState = EnemyState.ChasePlayer;
         return;
      }

      float distanceToBase = Vector3.Distance(transform.position, baseTransform.position);

      if (distanceToBase < baseAttackDistance)
      {
         currentState = EnemyState.AttackBase;
      }
   }

   void AttackBase()
   {
      agent.isStopped = true;
      LookAt(baseTransform.position);
      ShootTarget();
     // print("Atacar la base");
   }

   void ChasePlayer()
   {
      
      animator.SetBool("Shot Bullet Bool", false);
      
     // print("Seguir jugador");

      if (_sight.detectedTarget == null)
      {
         currentState = EnemyState.GoToBase;
         return;
      }

      agent.isStopped = false;
      agent.SetDestination(_sight.detectedTarget.transform.position);

      float distanceToPlayer = Vector3.Distance(transform.position,_sight.detectedTarget.transform.position);

      if (distanceToPlayer < playerAttackDistance)
      {
         currentState = EnemyState.AttackPlayer;
      }

   }

   void AttackPlayer()
   {
      //print("Atacar jugador");

      agent.isStopped = true;

      if (_sight.detectedTarget == null)
      {
         currentState = EnemyState.GoToBase;
         return;
      }
      LookAt(_sight.detectedTarget.transform.position);
      ShootTarget();

      float distanceToPlayer = Vector3.Distance(transform.position, _sight.detectedTarget.transform.position);


      if (distanceToPlayer > playerAttackDistance*1.1f)
      {
         currentState = EnemyState.ChasePlayer;
      }
      
      

   }

   
   void ShootTarget()
   {
      if (weapon.ShootBullet("Enemy Bullet",0))
      {
         animator.SetBool("Shot Bullet Bool", true);
      }

      

   }

   void LookAt(Vector3 targetPos)
   {
      var directionToLook = Vector3.Normalize(targetPos - transform.position);
      directionToLook.y = 0;
      transform.parent.forward = directionToLook;
   }

   private void OnDrawGizmos()
   {
      Gizmos.color = Color.green;
      Gizmos.DrawWireSphere(transform.position, playerAttackDistance);
      
      Gizmos.color = Color.blue;
      Gizmos.DrawWireSphere(transform.position, baseAttackDistance);
      
   }
}
