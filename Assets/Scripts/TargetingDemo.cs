using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
targeting algorithm:
first simulate shooting at the target's current position
calculate how long it takes the shot to get there, and then where the target is at that time
now do another iteration, this time shooting at that new target position
repeat a few times because the target estimate gets more accurate each time
*/

public class TargetingDemo : MonoBehaviour
{
    // For a "real" project, a better architecture would be for each object
    // to be it's own class with methods. However for this simple demo it was
    // more straightforward to simply control everything in one place.
    [SerializeField] Transform cannon;
    [SerializeField] Transform shot;
    [SerializeField] Transform missile;

    public float shotSpeed = .4f;
	public float missileSpeed = .2f;

	private Camera cam;
	private Plane[] frustum;

	private MeshRenderer shotRenderer;
	private MeshRenderer missileRenderer;

    void Start()
    {
        // used to spawn missiles according to the camera settings
        cam = Camera.main;

		// beware https://forum.unity.com/threads/checking-if-gameobject-is-visible-by-camera.454021/#post-2942105
		// instead https://answers.unity.com/questions/560057/detect-if-entity-is-visible-rendererisvisible-will.html
		frustum = GeometryUtility.CalculateFrustumPlanes(cam);

		// components used for visibility checks
		shotRenderer = shot.GetComponent<MeshRenderer>();
		missileRenderer = missile.GetComponent<MeshRenderer>();
	}

    void Update()
    {
        UpdateMissile();

		if (GeometryUtility.TestPlanesAABB(frustum, shotRenderer.bounds))
        {
            UpdateShot();
        }
        else
        {
            UpdateCannon();
        }
    }

	// the core function of this targeting algorithm
	private Vector3 CalculateRefinedTarget(Vector3 curTarget) {

		// calculate how long the shot takes
		var shotDist = Vector3.Distance(cannon.position, curTarget);
		var time = shotDist / shotSpeed;

		// calculate where the missile will have moved to
		var missileDist = time * missileSpeed;
		var missileRadians = missile.eulerAngles.z * Mathf.PI / 180;
		var dX = missileDist * Mathf.Sin(missileRadians);
		var dY = missileDist * Mathf.Cos(missileRadians);
		var newTarget = missile.position + new Vector3(-dX, dY, 0);

		return newTarget;
	}

	private void UpdateCannon()
	{
		var target = missile.position;
		for (var i = 0; i < 3; i++) {
			target = CalculateRefinedTarget(target);
		}

		// TODO this is just for testing
		shot.position = target;

		// TODO point to the calculated target
		//var dX:Number = target.x - cannon.x;
		//var dY:Number = target.y - cannon.y;
		//var angle:Number = Math.atan2(dY, dX) * 180 / Math.PI + 90;
		//cannon.rotation = angle;

		if (Input.anyKeyDown) {
			FireCannon();
		}
	}

	private void FireCannon() {
		shot.position = cannon.position;
		shot.rotation = cannon.rotation;
	}

	private void UpdateShot()
	{
		// TODO

		//check if target intercepted
		//if (shot.hitTestObject(missile)) {
		//	shot.y = -5;
		//}
	}

	private void UpdateMissile()
	{
		var missileRadians = missile.eulerAngles.z * Mathf.PI / 180;
		var dY = missileSpeed * Mathf.Cos(missileRadians);
		var dX = missileSpeed * Mathf.Sin(missileRadians);
		missile.position += new Vector3(-dX, dY, 0);

		if (!GeometryUtility.TestPlanesAABB(frustum, missileRenderer.bounds))
		{
			ResetMissile();
		}
	}

	private void ResetMissile()
	{
		missile.position = new Vector3(
			cam.transform.position.x + (Random.Range(1f, -1f) * cam.orthographicSize),
			cam.transform.position.y + cam.orthographicSize, 0
		);

		var angle = Random.Range(135, 215);
		missile.eulerAngles = new Vector3(0, 0, angle);
	}
}
