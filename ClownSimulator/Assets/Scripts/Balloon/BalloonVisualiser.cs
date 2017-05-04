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

	private List<Vector3> vertices = new List<Vector3>();
	private List<int> triangles = new List<int>();

	[SerializeField] private float radius2 = 2f;

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
//		vertices.Clear ();
//		triangles.Clear ();
//
//		points = ballon.GetPoints ().Count;
//
//		for (int i = 0; i < points; i++) 
//		{
//			AddTubeToPoint(i, ballon.GetPoints()[i]);    
//		}
//
//		mesh.vertices = vertices.ToArray();
//		mesh.triangles = triangles.ToArray();

		Torus ();
	}

	private void AddTubeToPoint(int num, BalloonPoint point) 
	{
		Vector3 pointPosition = point.transform.position;

		float turn = Mathf.PI * 2 / points;

		for (int s = 0; s < sides; s++) 
		{
			float angle = s * turn;

			Vector3 vertex = point.transform.position + Quaternion.Euler (point.transform.right * angle) * point.transform.forward;

			vertices.Add (vertex);
		}

		if (vertices.Count >= sides * 4)
		{
			for (int side = 0; side < sides; sides++) 
			{
				int start = vertices.Count - sides * 4;

				triangles.Add (start + 0);
				triangles.Add (start + 1);
				triangles.Add (start + 2);
				triangles.Add (start + 1);
				triangles.Add (start + 3);
				triangles.Add (start + 2);
			}
		}
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

	private void Torus()
	{
		

		List<BalloonPoint> points = ballon.GetPoints ();

		int pointsCount = points.Count;

		if (pointsCount < 2)
			return;

		#region Vertices		
		Vector3[] vertices = new Vector3[(pointsCount+1) * (sides+1)];
		float _2pi = Mathf.PI * 2f;
		for( int seg = 0; seg <= pointsCount; seg++ )
		{
			
			int currSeg = seg % (pointsCount);

			float radius1 = points[currSeg].transform.localScale.x;

			float t1 = (float)currSeg / pointsCount * _2pi;
			Vector3 r1 = points[currSeg].transform.position;// new Vector3( Mathf.Cos(t1) * radius1, 0f, Mathf.Sin(t1) * radius1 );//

			for( int side = 0; side <= sides; side++ )
			{
				int currSide = side == sides ? 0 : side;

				Vector3 normale =  Vector3.Cross( r1, Vector3.up );
				float t2 = (float)currSide / sides * _2pi;
				Vector3 r2 = Quaternion.AngleAxis( -t1 * Mathf.Rad2Deg, Vector3.up ) *new Vector3( Mathf.Sin(t2) * radius2, Mathf.Cos(t2) * radius2 );

				float angle = t2;

				Vector3 vertex = Quaternion.Euler (points[currSeg].transform.right * t2 * Mathf.Rad2Deg) * points[currSeg].transform.up;

				Debug.DrawRay(r1, vertex, Color.red);

				r2 = vertex * radius2;

				vertices[side + seg * (sides+1)] = r1 + r2;
			}
		}
		#endregion

		#region Triangles
		int nbFaces = vertices.Length;
		int nbTriangles = nbFaces * 2;
		int nbIndexes = nbTriangles * 3;
		int[] triangles = new int[ nbIndexes ];

		int i = 0;
		for( int seg = 0; seg <= pointsCount; seg++ )
		{			
			for( int side = 0; side <= sides - 1; side++ )
			{
				int current = side + seg * (sides+1);
				int next = side + (seg < (pointsCount) ?(seg+1) * (sides+1) : 0);

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
