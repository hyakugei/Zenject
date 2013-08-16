using UnityEngine;
using System.Collections;

public class MovingStarts : MonoBehaviour {
	
	public float Speed;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		float planMove = Speed * Time.deltaTime;
		transform.Translate(Vector3.down * planMove, Space.World);
		
		if(transform.position.y < -39)
		{
			transform.position = new Vector3(transform.position.x, 40f , transform.position.z);
		}
	}
}
