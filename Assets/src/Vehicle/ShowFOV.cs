using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Agent
{
	public class ShowFOV : MonoBehaviour {
		public bool show;
		public float FOV;
		public float viewDistance;
		public int raycastPoints;

		public MeshFilter FovPrefab;

		private MeshFilter fovMesh;

		void LateUpdate ()
		{
			if (show)
				updateFovMesh();
			else
				destroyFovMesh();
		}


		void updateFovMesh()
		{
			Vector3 pos = getFovMesh().transform.position;
			pos.y = 0.0025f;
			getFovMesh().transform.position = transform.position.setY(0.0025f);
			getFovMesh().transform.rotation = Quaternion.identity;

			Mesh mesh = getFovMesh().mesh;
			int[] tris = mesh.triangles;
			if (mesh.vertexCount != raycastPoints+1)
			{
				mesh.vertices = new Vector3[raycastPoints+1];
				tris = new int[raycastPoints*3];

				for(int i=0; i<raycastPoints-1; i++)
				{
					tris[3*i+0] = 0;
					tris[3*i+1] = i+2;
					tris[3*i+2] = i+1;
				}
			}

			Vector3[] vertices = mesh.vertices;
			vertices[0] = Vector3.zero;

			for (int i=0; i<raycastPoints; i++)
			{
				float angle = i*FOV/(raycastPoints-1) - FOV/2;
				RaycastHit hitInfo;
				Vector3 direction = Vector2.up.turn(angle).toVector3().normalized;
				if (Physics.Raycast(transform.position, direction, out hitInfo, viewDistance))
					vertices[i+1] = hitInfo.point-transform.position;
				else
					vertices[i+1] = direction*viewDistance;
			}

			mesh.vertices = vertices;
			mesh.uv = Enumerable.Repeat(Vector2.zero, vertices.Length).ToArray();
			mesh.triangles = tris;
			mesh.RecalculateNormals();
			getFovMesh().mesh = mesh;
		}

		private MeshFilter getFovMesh()
		{
			if (fovMesh == null)
			{
				fovMesh = (MeshFilter)Instantiate(FovPrefab);
				Vector3 pos = transform.position;
				pos.y = 0.0025f;
				fovMesh.transform.parent = transform;
			}
			return fovMesh;
		}

		private void destroyFovMesh()
		{
			if (fovMesh != null)
			{
				Destroy(fovMesh.gameObject);
				fovMesh = null;
			}
		}
	}
}
