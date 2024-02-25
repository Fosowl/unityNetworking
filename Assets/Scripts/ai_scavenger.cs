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
    public float range = 5.0f; // range of the radar

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
    public Animator animator;
    // private
    radar LIDAR = new radar();
    List<sensor_ray> radar_feedback;
    float speed;
    private Vector3 m_Move;
    public GameObject player;

    void Start()
    {
        LIDAR.interval = 15.0f;
        LIDAR.range = 4.0f;
        animator = GetComponent<Animator>();
        speed = 4.0f;
    }

    void Update()
    {
        radar_feedback = LIDAR.scanAround(gameObject); 
        Vector3 playerPos = player.transform.position;
        float playerDistance = Vector3.Distance(playerPos, transform.position);
        Quaternion targetRotation = decide(radar_feedback, playerPos, playerDistance);
        Quaternion relativeRotation = Quaternion.Inverse(transform.rotation) * targetRotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation * relativeRotation, Time.deltaTime * speed);
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
        float closest = 10.0f;
        float closestAngle = 0.0f;
        // obstacle avoidance
        foreach (sensor_ray sensor in feedback) {
            if (sensor.measure < closest && sensor.measure > 0f) {
                closest = sensor.measure;
                closestAngle = sensor.angle;
            }
        }
        float playerAngle = Mathf.Atan2(playerPos.x - transform.position.x, playerPos.z - transform.position.z) * Mathf.Rad2Deg;
        float opposite = 0.0f;
        if (playerDistance < 8.0f) {
            opposite = playerAngle;
        } else {
            opposite = closest < 3.0f ? closestAngle-90 : playerAngle;
        }
        return Quaternion.Euler(0, opposite > 360 ? (opposite - 360) : opposite, 0);
    }

}
