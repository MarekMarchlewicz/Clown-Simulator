using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Balloon : MonoBehaviour
{
    [SerializeField]
    private BalloonLifecycle lifecycle;

    [SerializeField]
    private GameObject balloonPointPrefab;

    [SerializeField]
    private float newBalloonPointRatio = 0.2f;

    [SerializeField]
    private float inflatingTime = 0.2f;

    [SerializeField]
    private float deflatingTime = 0.2f;

    [SerializeField]
    private float deflatingDelay = 0.05f;

    public float ForceMultiplier = 5f;

    private float lastSpawnBalloonPointTime;

    private BalloonPoint lastAddedBallonPoint;

    private Rigidbody mRigidBody;

    private void Awake()
    {
        mRigidBody = GetComponent<Rigidbody>();
    }

    public void AddForceAtPosition(Vector3 force, Vector3 position)
    {
        mRigidBody.AddForceAtPosition(force, position);
    }

    private void Update()
    {
        if (lifecycle == BalloonLifecycle.Inflating)
        {
            if (Input.GetMouseButton(0))
            {
                if (Time.time - lastSpawnBalloonPointTime > newBalloonPointRatio)
                {
                    Vector3 spawnPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
                    spawnPosition.z = 0f;

                    BalloonPoint newBalloonPoint = Instantiate(balloonPointPrefab, spawnPosition, Quaternion.identity, transform).GetComponent<BalloonPoint>();

                    newBalloonPoint.Initialize(this, inflatingTime);

                    if (lastAddedBallonPoint != null)
                    {
                        lastAddedBallonPoint.AddNeighbour(newBalloonPoint);
                        newBalloonPoint.AddNeighbour(lastAddedBallonPoint);
                    }

                    lastAddedBallonPoint = newBalloonPoint;

                    lastSpawnBalloonPointTime = Time.time;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                lifecycle = BalloonLifecycle.Idle;
            }
        }
        else if (lifecycle == BalloonLifecycle.Idle)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if(Physics.Raycast(ray, out hit, 100f))
                {
                    BalloonPoint balloonPoint = hit.collider.GetComponent<BalloonPoint>();

                    if(balloonPoint != null)
                    {
                        balloonPoint.Pierce(hit.point, deflatingTime, deflatingDelay);
                    }
                }
            }
        }
    }
}
