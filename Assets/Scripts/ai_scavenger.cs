using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sensor_ray
{
    public float angle;
    public float measure;
    public float max_dist;
}

public class radar
{
    public float interval = 45.0f; // angle interval between each ray
    public float range = 4.0f; // range of the radar

    public List<sensor_ray> scanAround(GameObject self)
    {
        List<sensor_ray> sensors = new List<sensor_ray>();
        RaycastHit hit;
        Vector3 position = new Vector3(self.transform.position.x, self.transform.position.y + 1.0f, self.transform.position.z);

        for (int a = 0; a < 360; a += (int)interval) {
            sensor_ray sensor = new sensor_ray();
            Color color = Color.red;
            Vector3 dir = Quaternion.AngleAxis(a, self.transform.up) * self.transform.forward;
            bool col = Physics.Raycast(position, dir, out hit, range);
            sensor.max_dist = range;
            sensor.measure = hit.distance;
            sensor.angle = a;
            if (col && sensor.measure <= range * 0.25) {
                color = Color.red;
            } else if (col && sensor.measure <= range * 0.5) {
                color = Color.yellow;
            } else {
                color = Color.green;
            }
            Debug.DrawRay(position, dir * range, color);
            sensors.Add(sensor);
        }
        return sensors;
    }

    public float getSpeed(GameObject self)
    {
        return self.GetComponent<Rigidbody>().velocity.z;
    }
}

public class ai_scavenger : MonoBehaviour
{
    // public
    public Animator animator;
    // private
    private Transform player;
    private radar LIDAR = new radar();
    private List<sensor_ray> radar_feedback;
    private Vector3 m_Move;
    private float speed = 4.5f;
    private float rotationSpeed = 0.5f;
    private float targetAngleSave = 0.0f;

    void Start()
    {
        LIDAR.interval = 15.0f;
        LIDAR.range = 4.0f;
        animator = GetComponent<Animator>();
    }

    // find player in scene because player object appear only when connected ?
    void FindPlayer()
    {
        GameObject playerGameObject = GameObject.FindWithTag("Player");
        if (playerGameObject != null) {
            player = playerGameObject.transform;
        }
    }

    void Update()
    {
        if (player == null) {
            FindPlayer();
            return;
        }
        radar_feedback = LIDAR.scanAround(gameObject); 
        Vector3 playerPos = player.transform.position;
        float playerDistance = Vector3.Distance(playerPos, transform.position);
        float playerAngle = Mathf.Atan2(playerPos.x - transform.position.x, playerPos.z - transform.position.z) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, 0);

        if (playerDistance < 7.0f && playerDistance > 1.0f) {
            Quaternion worldRotation = Quaternion.Euler(0, playerAngle, 0);
            targetRotation = Quaternion.Inverse(transform.parent.rotation) * worldRotation;
        } else {
            targetRotation = decide(radar_feedback, playerPos, playerDistance);
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation * targetRotation, Time.deltaTime * rotationSpeed);
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
        if (playerDistance < 2.0f) {
            animator.Play("root|Anim_monster_scavenger_attack");
        } else {
            animator.Play("root|Anim_monster_scavenger_walk");
        }
    }

    Quaternion decide(List<sensor_ray> feedback, Vector3 playerPos, float playerDistance)
    {
        Vector3 directionSum = Vector3.zero;
        float closest = 4.0f;
        float closestAngle = 0.0f;
        // obstacle avoidance
        foreach (sensor_ray sensor in feedback) {
            if (sensor.measure < closest && sensor.measure > 0f) {
                closest = sensor.measure;
                closestAngle = sensor.angle;
            }
        }
        float target = 0.0f;
        float obstacleAngleOpposite = closestAngle+180;
        if (obstacleAngleOpposite > 360) {
            obstacleAngleOpposite -= 360;
        }
        target = obstacleAngleOpposite;
        return Quaternion.Euler(0, target, 0);
    }
}
