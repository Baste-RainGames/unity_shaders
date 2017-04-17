﻿using UnityEngine;
using UnityEditor;
using System.Collections;

public class ConeOfSightRaycaster : MonoBehaviour {

	public GameObject ConeOfSight;
	public int DepthBufferSize;
	public float SightAngle;
	public float MaxDistance;
	public bool DrawDebug;

	private Quaternion m_Rotation;
	private float[] m_aDepthBuffer;
	private Material m_ConeOfSightMat;

	void Start () {
		m_ConeOfSightMat = ConeOfSight.GetComponent<Renderer>().sharedMaterial;
		m_aDepthBuffer = new float[DepthBufferSize];
		m_ConeOfSightMat.SetInt("_BufferSize", DepthBufferSize);
	}
	

	void FixedUpdate () {
		//if (m_Rotation != this.transform.rotation){
			m_Rotation = this.transform.rotation;
			UpdateViewDepthBuffer();
		//}
	}

	void OnDrawGizmos(){
		Handles.DrawWireArc(this.transform.position,Vector3.up,this.transform.position,360,MaxDistance);

		float halfangle = SightAngle/2 * Mathf.PI / 180;
		float viewangle = this.transform.rotation.eulerAngles.y * Mathf.PI / 180;

		Vector3 p1 = GetVector(-halfangle - viewangle, MaxDistance);
		Vector3 p2 = GetVector(halfangle - viewangle, MaxDistance);

		Debug.DrawRay(this.transform.position, p1);
		Debug.DrawRay(this.transform.position, p2);
	}

	void UpdateViewDepthBuffer(){
		
		float anglestep = (SightAngle / DepthBufferSize) * Mathf.PI / 180;
		float viewangle = m_Rotation.eulerAngles.y * Mathf.PI / 180;
		for(int i = 0; i < DepthBufferSize; i++){
			float angle = anglestep*(i-DepthBufferSize/2) - viewangle;
			Vector3 dest = GetVector(angle, MaxDistance);
			Ray r = new Ray(this.transform.position, dest);
		
			RaycastHit hit = new RaycastHit();
			if(Physics.Raycast(r,out hit)){
				m_aDepthBuffer[i] = (hit.distance/MaxDistance);
				//if (DrawDebug)
					//Debug.DrawRay(this.transform.position, hit.point,Color.red);
			}else{
				m_aDepthBuffer[i] = -1;
				if (DrawDebug)
					Debug.DrawRay(this.transform.position, dest);
			}
		}
		m_ConeOfSightMat.SetFloatArray("_SightDepthBuffer",m_aDepthBuffer);
	}

	Vector3 GetVector(float angle, float dist){
		float x = Mathf.Cos(angle)*dist;
		float z = Mathf.Sin(angle)*dist;
		return new Vector3(x, this.transform.position.y,z);
	}

}