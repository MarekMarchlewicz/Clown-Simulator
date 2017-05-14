using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BalloonVisualiser))]
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

	private List<BalloonPoint> balloonPoints;

	private BalloonVisualiser visualiser;

    private BalloonPoint lastAddedBallonPoint;

    private Rigidbody mRigidBody;

    private void Awake()
    {
        mRigidBody = GetComponent<Rigidbody>();

		visualiser = GetComponent<BalloonVisualiser> ();
		visualiser.Initialize (this);

		balloonPoints = new List<BalloonPoint> ();
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

					balloonPoints.Add (newBalloonPoint);
                    
					// First Point
					if (lastAddedBallonPoint == null) 
					{
						newBalloonPoint.Initialize(this, inflatingTime, 0f);

						lastAddedBallonPoint = newBalloonPoint;
					} 
					// Next point
					else 
					{
						newBalloonPoint.Initialize(this, inflatingTime, 1f);

						Vector3 newDirection = newBalloonPoint.transform.position - lastAddedBallonPoint.transform.position;
						newDirection.Normalize ();

						newBalloonPoint.transform.forward = newDirection;

						lastAddedBallonPoint.SetNext (newBalloonPoint);
						newBalloonPoint.SetPrevious (lastAddedBallonPoint);
					}

					lastAddedBallonPoint = newBalloonPoint;

                    lastSpawnBalloonPointTime = Time.time;

					for (int i = 0; i < balloonPoints.Count; i++) 
					{
						balloonPoints [i].gameObject.name = "Point" + i.ToString ();
					}
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

		visualiser.UpdateMesh ();
    }

	public List<BalloonPoint> GetPoints()
	{
		return balloonPoints;
	}
}
