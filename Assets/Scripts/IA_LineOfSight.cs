using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

[RequireComponent(typeof(SphereCollider))]

public class IA_LineOfSight : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private float detection_delay = 0.5f;
    
    //move to another script maybe
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    private Collider player_collider;
    private SphereCollider detection_collider;
  //  private Bounds player_bounds;
    private Coroutine detect_player;

    [SerializeField] private bool isChasing = false;
    [SerializeField] private float attackDistance = 3.5f;
    [SerializeField] private float loseDistance = 14f;

    bool tmp = false;

    private void Awake()
    {
        detection_collider = this.GetComponent<SphereCollider>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        animator.Play("root|Anim_monster_scavenger_Idle1");
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.tag == "Player" || other.tag == "PlayerMain") && !target)
        {
            target = other.gameObject;
            detect_player = StartCoroutine(DetectPlayer());
            player_collider = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
        }
    }

    private void LoseTarget()
    {
        if (agent.remainingDistance >= loseDistance)
        {
            Debug.Log("Lost");
            target = null;
            animator.Play("root|Anim_monster_scavenger_Idle1");
            StopCoroutine(detect_player);
            agent.ResetPath();
            isChasing = false;
            tmp = false;
        }
    }

    private void FixedUpdate()
    {
        if (isChasing)
        {
            GoToPlayer();
            LoseTarget();
        }
    }

    IEnumerator DetectPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(detection_delay);

            Vector3[] points = GetBoundingPoints(player_collider.bounds);

            int points_hidden = 0;

            foreach (Vector3 point in points)
            {
                Vector3 target_direction = point - this.transform.position;
                float target_distance = Vector3.Distance(this.transform.position, point);
                float target_angle = Vector3.Angle(target_direction, this.transform.forward);

                if (IsPointCovered(target_direction, target_distance) || target_angle > 70)
                    ++points_hidden;
            }

            if (points_hidden >= points.Length)
                Debug.Log("player is hidden");
            else
            {
                Debug.Log("Player is visible");
                StopCoroutine(detect_player);
                isChasing = true;
            }
        }
    }

    void GoToPlayer()
    {
       // tmp = agent.remainingDistance;
        if (agent.hasPath && agent.remainingDistance <= attackDistance)
        {
            animator.Play("root|Anim_monster_scavenger_attack");
            tmp = false;
            agent.isStopped = true;
        }
        else
        {
            if (tmp == false)
            {
                animator.Play("root|Anim_monster_scavenger_walk");
                tmp = true;
            }
            agent.isStopped = false;
        }
        agent.SetDestination(target.transform.position);
//        if (tmp != agent.remainingDistance) 
            Debug.Log(agent.remainingDistance);
    }

    private bool IsPointCovered(Vector3 target_direction, float target_distance)
    {
        RaycastHit[] hits = Physics.RaycastAll(this.transform.position, target_direction, detection_collider.radius);
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                float cover_distance = Vector3.Distance(this.transform.position, hit.point);

                if (cover_distance < target_distance)
                {
                    Debug.DrawRay(this.transform.position, target_direction * detection_collider.radius, Color.red, 0.1f);
                    return true;
                }
            }
        }
        Debug.DrawRay(this.transform.position, target_direction * detection_collider.radius, Color.green, 0.1f);
        return false;
    }

    private Vector3[] GetBoundingPoints(Bounds bounds)
    {
        Vector3[] bounding_points =
        {
            bounds.min,
            bounds.max,
            new Vector3( bounds.min.x, bounds.min.y, bounds.max.z ),
            new Vector3( bounds.min.x, bounds.max.y, bounds.min.z ),
            new Vector3( bounds.max.x, bounds.min.y, bounds.min.z ),
            new Vector3( bounds.min.x, bounds.max.y, bounds.max.z ),
            new Vector3( bounds.max.x, bounds.min.y, bounds.max.z ),
            new Vector3( bounds.max.x, bounds.max.y, bounds.min.z )
        };

        return bounding_points;
    }
}
