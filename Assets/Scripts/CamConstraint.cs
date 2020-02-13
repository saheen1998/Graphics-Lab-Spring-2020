using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamConstraint : MonoBehaviour
{
    public Transform cameraOrbit;
    
	private Vector3 targetCenter = new Vector3(1, 0, 0);

	private void OnEnable() {
		StartCoroutine(EnumResetPos());
	}

	public void ResetPos(){
		StartCoroutine(EnumResetPos());
	}

    IEnumerator EnumResetPos()
    {
        yield return new WaitForSeconds(0.05f);
        var objects = GameObject.FindGameObjectsWithTag("Point");
		if(objects.Length != 0){
			
			Transform[] targets = new Transform[objects.Length];
			for(int i = 0; i<objects.Length; i++){
				targets[i] = objects[i].GetComponent<Transform>();
			}

			var bounds = new Bounds(targets[0].position, Vector3.zero);
			for(int i = 0; i<objects.Length; i++){
				bounds.Encapsulate(targets[i].position);
			}

			targetCenter = bounds.center;
		} else
		{
            LogHandler.Logger.Log(gameObject.name + " - CamConstraint.cs: Points not found!", LogType.Warning);
		}

        cameraOrbit.position = targetCenter;
    }

    void Update()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 0);
        transform.LookAt(cameraOrbit.position);
    }
}
