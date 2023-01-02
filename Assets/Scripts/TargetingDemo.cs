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

    public float shotSpeed = .2f;
	public float missileSpeed = .1f;

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
		var radians = missile.eulerAngles.z * Mathf.PI / 180;
		var dX = missileDist * Mathf.Sin(radians);
		var dY = missileDist * Mathf.Cos(radians);
		var newTarget = missile.position + new Vector3(-dX, dY, 0);

		return newTarget;
	}

	private void UpdateCannon()
	{
		var target = missile.position;
		for (var i = 0; i < 3; i++) {
			target = CalculateRefinedTarget(target);
		}

		// point to the calculated target
		var deltaPos = cannon.position - target;
		var angle = Mathf.Atan2(deltaPos.y, deltaPos.x) * 180 / Mathf.PI + 90;
		cannon.eulerAngles = new Vector3(0, 0, angle);

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
		var radians = shot.eulerAngles.z * Mathf.PI / 180;
		var dY = shotSpeed * Mathf.Cos(radians);
		var dX = shotSpeed * Mathf.Sin(radians);
		shot.position += new Vector3(-dX, dY, 0);

		// check if target intercepted
		if (Vector3.Distance(shot.position, missile.position) < 1) {
			shot.position = new Vector3(0, -5, 0);
		}
	}

	private void UpdateMissile()
	{
		var radians = missile.eulerAngles.z * Mathf.PI / 180;
		var dY = missileSpeed * Mathf.Cos(radians);
		var dX = missileSpeed * Mathf.Sin(radians);
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
