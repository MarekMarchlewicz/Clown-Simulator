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
		vertices.Clear ();
		triangles.Clear ();

		points = ballon.GetPoints ().Count;

		for (int i = 0; i < points; i++) 
		{
			AddTubeToPoint(i, ballon.GetPoints()[i]);    
		}

		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
	}

	private void AddTubeToPoint(int num, BalloonPoint point) 
	{
		Vector3 pointPosition = point.transform.position;

		float turn = Mathf.PI * 2 / points;

		//for (int s = 0; s < 2; s++) 
		//{
//			float angle = s * turn;
//
//			Vector3 vertex = point.transform.position + Quaternion.Euler (point.transform.right * angle) * point.transform.forward;
//
//			vertices.Add (vertex);
		//}

		vertices.Add (pointPosition - point.transform.up * point.transform.localScale.x * 0.5f);

		vertices.Add (pointPosition + point.transform.up * point.transform.localScale.x * 0.5f);

		if (vertices.Count >= 4) 
		{
			int start = vertices.Count - 4;

			triangles.Add (start + 0);
			triangles.Add (start + 1);
			triangles.Add (start + 2);
			triangles.Add (start + 1);
			triangles.Add (start + 3);
			triangles.Add (start + 2);    
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
}
