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

    private Vector3 lastSpawnBalloonPoint;

	private List<BalloonPoint> balloonPoints;

	private BalloonVisualiser visualiser;

    private BalloonPoint lastAddedBallonPoint;

    private Rigidbody mRigidBody;

    private Transform spawnTarget;

    private Color currentColour = Color.white;

    public void Initialize(Transform newSpawnTarget)
    {
        mRigidBody = GetComponent<Rigidbody>();

		visualiser = GetComponent<BalloonVisualiser> ();
		visualiser.Initialize (this);

		balloonPoints = new List<BalloonPoint> ();

        lifecycle = BalloonLifecycle.Idle;

        spawnTarget = newSpawnTarget;
    }

    public void AddForceAtPosition(Vector3 force, Vector3 position)
    {
        mRigidBody.AddForceAtPosition(force, position);
    }

    public void StartInflating()
    {
        if (lifecycle == BalloonLifecycle.Idle)
        {
            lifecycle = BalloonLifecycle.Inflating;
        }
    }

    public void StopInflating()
    {
        if (lifecycle == BalloonLifecycle.Inflating)
        {
            lifecycle = BalloonLifecycle.Inflated;

            BalloonPoint newBalloonPoint = Instantiate(balloonPointPrefab, spawnTarget.position, spawnTarget.rotation, transform).GetComponent<BalloonPoint>();

            balloonPoints.Add(newBalloonPoint);

            // First Point
            if (lastAddedBallonPoint == null)
            {
                lastAddedBallonPoint = newBalloonPoint;
            }
            // Next point
            else
            {
                newBalloonPoint.Initialize(this, inflatingTime, currentColour, 0f);

                Vector3 newDirection = newBalloonPoint.transform.position - lastAddedBallonPoint.transform.position;
                newDirection.Normalize();

                lastAddedBallonPoint.SetNext(newBalloonPoint);
                newBalloonPoint.SetPrevious(lastAddedBallonPoint);
            }

            lastAddedBallonPoint = newBalloonPoint;

            lastSpawnBalloonPoint = spawnTarget.position;
        }
    }

    public void SetColour(Color newColour)
    {
        currentColour = newColour;
    }

    private void Update()
    {
        if (lifecycle == BalloonLifecycle.Inflating)
        {
            if (Vector3.Distance(lastSpawnBalloonPoint, spawnTarget.position) > newBalloonPointRatio)
            {
                BalloonPoint newBalloonPoint = Instantiate(balloonPointPrefab, spawnTarget.position, spawnTarget.rotation, transform).GetComponent<BalloonPoint>();

                balloonPoints.Add(newBalloonPoint);

                // First Point
                if (lastAddedBallonPoint == null)
                {
                    newBalloonPoint.Initialize(this, inflatingTime, currentColour, 0f);

                    lastAddedBallonPoint = newBalloonPoint;
                }
                // Next point
                else
                {
                    newBalloonPoint.Initialize(this, inflatingTime, currentColour, 0.03f);

                    lastAddedBallonPoint.SetNext(newBalloonPoint);
                    newBalloonPoint.SetPrevious(lastAddedBallonPoint);
                }

                lastAddedBallonPoint = newBalloonPoint;

                lastSpawnBalloonPoint = spawnTarget.position;
            }
        }
        else if (lifecycle == BalloonLifecycle.Inflated)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100f))
                {
                    BalloonPoint balloonPoint = hit.collider.GetComponent<BalloonPoint>();

                    if (balloonPoint != null)
                    {
                        balloonPoint.Pierce(hit.point, deflatingTime, deflatingDelay);
                    }
                }
            }
        }

        if(lifecycle != BalloonLifecycle.Idle)
		    visualiser.UpdateMesh ();
    }

	public List<BalloonPoint> GetPoints()
	{
		return balloonPoints;
	}
}
