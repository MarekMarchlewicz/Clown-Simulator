using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonVisualiser : MonoBehaviour 
{
	[SerializeField] private int sides = 18;

	private Balloon ballon;

	private MeshRenderer meshRenderer;

	private MeshFilter meshFilter;

	private Mesh mesh;

	private int points;

	public void Initialize(Balloon parent)
	{
		ballon = parent;

		meshRenderer = ballon.GetComponent<MeshRenderer> ();
		meshFilter = ballon.GetComponent<MeshFilter> ();

		mesh = meshFilter.mesh;
	}

	private void Update()
	{
		RefreshMesh ();
	}

	private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) 
	{
		float u = 1 - t;
		float tt = t * t;
		float uu = u * u;
		float uuu = uu * u;
		float ttt = tt * t;

		Vector3 p = uuu * p0;
		p += 3 * uu * t * p1;
		p += 3 * u * tt * p2;
		p += ttt * p3;

		return p;
	}

	private void RefreshMesh()
	{
		List<BalloonPoint> points = ballon.GetPoints ();

		int pointsCount = points.Count;

		if (pointsCount < 2)
			return;

		#region Vertices		
		Vector3[] vertices = new Vector3[(pointsCount) * (sides+1)];

		BalloonPoint point;

		for( int seg = 0; seg < pointsCount; seg++ )
		{
			point = points[seg];

			float radius = point.transform.localScale.x;

			Vector3 position = points[seg].LocalPosition;

			for( int side = 0; side <= sides; side++ )
			{
				int currSide = side == sides ? 0 : side;

				float angle = -(float)currSide / sides * 360f;

				Vector3 vertex = Quaternion.Euler (points[seg].transform.right * angle) * points[seg].transform.forward;

				vertex *= radius;

				vertices[side + seg * (sides + 1)] = position + vertex;
			}
		}
		#endregion

		#region Triangles
		int facesNo = vertices.Length;
		int trianglesNo = facesNo * 2;
		int indexes = trianglesNo * 3;
		int[] triangles = new int[ indexes ];

		int i = 0;
		for( int seg = 0; seg < pointsCount - 1; seg++ )
		{			
			for( int side = 0; side <= sides - 1; side++ )
			{
				int current = side + seg * (sides+1);

				int next = side + (seg + 1) * (sides+1);

				if( i < triangles.Length - 6 )
				{
					triangles[i++] = current;
					triangles[i++] = next;
					triangles[i++] = next+1;

					triangles[i++] = current;
					triangles[i++] = next+1;
					triangles[i++] = current+1;
				}
			}
		}
		#endregion

		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
	}
}
