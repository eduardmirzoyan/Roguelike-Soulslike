using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldOfViewMesh : MonoBehaviour
{
	[SerializeField] private FieldOfView fov;

	public List<Transform> visibleTargets = new List<Transform>();

	public float meshResolution;

	public MeshFilter viewMeshFilter;
	Mesh viewMesh;

	void Start()
	{
		viewMesh = new Mesh();
		viewMesh.name = "View Mesh";
		viewMeshFilter.mesh = viewMesh;


		fov = GetComponentInParent<FieldOfView>();
	}


	void LateUpdate()
	{
		DrawFieldOfView();
	}


	void DrawFieldOfView()
	{
		int stepCount = Mathf.RoundToInt(fov.viewAngle * meshResolution);
		float stepAngleSize = fov.viewAngle / stepCount;


		List<Vector3> viewPoints = new List<Vector3>();
		for (int i = 0; i <= stepCount; i++)
		{
			float angle = transform.eulerAngles.y - fov.viewAngle / 2 + stepAngleSize * i;
			Vector3 dir = fov.DirFromAngle(angle, false);

			RaycastHit2D hit = Physics2D.Raycast(fov.transform.position, dir, fov.viewRadius, fov.obstacleMask);
			if(hit.collider == null)
            {
				viewPoints.Add(transform.position + dir.normalized * fov.viewRadius);

			}
			else
            {
				viewPoints.Add(transform.position + dir.normalized * hit.distance);
			}
		}

		int vertexCount = viewPoints.Count + 1;
		Vector3[] vertices = new Vector3[vertexCount];
		int[] triangles = new int[(vertexCount - 2) * 3];

		vertices[0] = Vector3.zero;
		for (int i = 0; i < vertexCount - 1; i++)
		{
			vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

			if (i < vertexCount - 2)
			{
				triangles[i * 3 + 2] = 0;
				triangles[i * 3 + 1] = i + 1;
				triangles[i * 3] = i + 2;
			}
		}

		viewMesh.Clear();

		viewMesh.vertices = vertices;
		viewMesh.triangles = triangles;
		viewMesh.RecalculateNormals();
	}

}