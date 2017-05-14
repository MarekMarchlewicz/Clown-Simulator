using UnityEngine;
using System.Collections;

public class BalloonPoint : MonoBehaviour
{
    private Balloon parent;

	private BalloonPoint previous, next;

	public float Size { get { return transform.localScale.x; } }

	public Vector3 LocalPosition { get { return transform.localPosition; } }

	private BalloonLifecycle localState;

	public void Initialize(Balloon balloon, float inflatingTime, float targetScale = 1f)
    {
        parent = balloon;

        transform.localScale = Vector3.zero;

		StartCoroutine(Inflate(inflatingTime, targetScale));
    }

	private IEnumerator Inflate(float inflatingTime, float targetScale)
    {
		localState = BalloonLifecycle.Inflating;

        float startTime = Time.time;

        while(Time.time - startTime < inflatingTime)
        {
            float lerp = (Time.time - startTime) / inflatingTime;

			transform.localScale = Vector3.one * lerp * targetScale;

            yield return new WaitForEndOfFrame();
        }

		transform.localScale = Vector3.one * targetScale;

		localState = BalloonLifecycle.Idle;
    }

    public void Pierce(Vector3 piercePosition, float deflatingTime, float deflatingDelay)
    {
		if (localState == BalloonLifecycle.Idle) 
		{
			StopAllCoroutines ();

			StartCoroutine (Deflate (piercePosition, deflatingTime, deflatingDelay));
		}
    }

    private IEnumerator Deflate(Vector3 piercePosition, float deflatingTime, float deflatingDelay)
    {
		localState = BalloonLifecycle.Deflating;

        float startTime = Time.time;

		bool trigerredNeighbours = false;
        while (Time.time - startTime < deflatingTime)
        {

			if(! trigerredNeighbours && Time.time - startTime > deflatingDelay)
            {
				if (next != null) 
				{
					next.Pierce(transform.position, deflatingTime, deflatingDelay);
				}

				if (previous != null) 
				{
					previous.Pierce(transform.position, deflatingTime, deflatingDelay);
				}

				trigerredNeighbours = true;
            }

            float lerp = (Time.time - startTime) / deflatingTime;

            Vector3 force = (transform.position - piercePosition).normalized * (1f - lerp) * parent.ForceMultiplier;

            parent.AddForceAtPosition(force, transform.position);

            transform.localScale = Vector3.one * (1f - lerp);

            yield return new WaitForEndOfFrame();
        }

		transform.localScale = Vector3.zero;
    }

	public BalloonPoint GetNext()
	{
		return next;
	}

	public void SetNext(BalloonPoint balloonPoint)
	{
		next = balloonPoint;
	}

	public BalloonPoint GetPrevious()
	{
		return previous;
	}

	public void SetPrevious(BalloonPoint balloonPoint)
	{
		previous = balloonPoint;
	}
}
