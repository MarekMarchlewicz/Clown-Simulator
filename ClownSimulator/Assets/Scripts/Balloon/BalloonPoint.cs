using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BalloonPoint : MonoBehaviour
{
	public System.Action<BalloonPoint> OnDestroyed;

    private Balloon parent;

    private List<BalloonPoint> neighbours = new List<BalloonPoint>();

    public void Initialize(Balloon balloon, float inflatingTime)
    {
        parent = balloon;

        transform.localScale = Vector3.zero;

        StartCoroutine(Inflate(inflatingTime));
    }

    private IEnumerator Inflate(float inflatingTime)
    {
        float startTime = Time.time;

        while(Time.time - startTime < inflatingTime)
        {
            float lerp = (Time.time - startTime) / inflatingTime;

            transform.localScale = Vector3.one * lerp;

            yield return new WaitForEndOfFrame();
        }

        transform.localScale = Vector3.one;
    }

    public void Pierce(Vector3 piercePosition, float deflatingTime, float deflatingDelay)
    {
        StopAllCoroutines();

        StartCoroutine(Deflate(piercePosition, deflatingTime, deflatingDelay));
    }

    private IEnumerator Deflate(Vector3 piercePosition, float deflatingTime, float deflatingDelay)
    {
        float startTime = Time.time;

        while (Time.time - startTime < deflatingTime)
        {
            if(Time.time - startTime > deflatingDelay)
            {
                foreach(BalloonPoint neighbour in neighbours)
                {
                    neighbour.Pierce(transform.position, deflatingTime, deflatingDelay);
                }
            }

            float lerp = (Time.time - startTime) / deflatingTime;

            Vector3 force = (transform.position - piercePosition).normalized * (1f - lerp) * parent.ForceMultiplier;

            parent.AddForceAtPosition(force, transform.position);

            transform.localScale = Vector3.one * (1f - lerp);

            yield return new WaitForEndOfFrame();
        }

//		if (OnDestroyed != null) 
//		{
//			OnDestroyed (this);
//		}
//
//        Destroy(gameObject);
    }

    public void AddNeighbour(BalloonPoint newNeighbour)
    {
        neighbours.Add(newNeighbour);

		newNeighbour.OnDestroyed += OnNeighbourDestroyed;
    }

	private void OnNeighbourDestroyed(BalloonPoint neighbourToRemove)
	{
		neighbours.Remove(neighbourToRemove);
	}

    public List<BalloonPoint> GetNeighbours()
    {
        return neighbours;
    }
}
