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

    public float shotSpeed = .8f;
	public float missileSpeed = .4f;

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

	}

	private void UpdateMissile()
	{
		//var dY:Number = missileSpeed * Math.cos(missile.rotation * Math.PI / 180);
		//var dX:Number = missileSpeed * Math.sin(missile.rotation * Math.PI / 180);

		//missile.x += dX;
		//missile.y -= dY;

		if (!GeometryUtility.TestPlanesAABB(frustum, missileRenderer.bounds))
		{
			ResetMissile();
		}
	}

	private void ResetMissile()
	{
		missile.position = new Vector3(
			cam.transform.position.x + (Random.Range(1, -1) * cam.orthographicSize),
			cam.transform.position.y + cam.orthographicSize, 0
		);

		var angle = Random.Range(135, 215);
		missile.eulerAngles = new Vector3(0, 0, angle);
	}
}

/*
//the targeting algorithm
private function calcNewTarget(curTarget:Point):Point {

	//calculate how long the shot takes
	var dX:Number = curTarget.x - cannon.x;
	var dY:Number = curTarget.y - cannon.y;
	var dist:Number = Math.sqrt(dX * dX + dY * dY);
	var time:Number = dist / shotSpeed;

	//calculate where the missile will have moved to
	dY = time * missileSpeed * Math.cos(missile.rotation * Math.PI / 180);
	dX = time * missileSpeed * Math.sin(missile.rotation * Math.PI / 180);
	var newTarget:Point = new Point(missile.x + dX, missile.y - dY);

	return newTarget;
}

private function updateCannon():void {
	var target:Point = new Point(missile.x, missile.y);
	for (var i:int = 0; i < 3; i++) {
		target = calcNewTarget(target);
	}

	//point to the calculated target
	var dX:Number = target.x - cannon.x;
	var dY:Number = target.y - cannon.y;
	var angle:Number = Math.atan2(dY, dX) * 180 / Math.PI + 90;
	cannon.rotation = angle;
}

		private function updateMissile():void {
			var dY:Number = missileSpeed * Math.cos(missile.rotation * Math.PI/180);
			var dX:Number = missileSpeed * Math.sin(missile.rotation * Math.PI/180);
			
			missile.x += dX;
			missile.y -= dY;
			
			//reset if moved outside area
			if (missile.y > stage.stageHeight * .7 || missile.x < -10 || missile.x > stage.stageWidth + 10) {
				missile.y = -10;
				missile.x = Math.random() * stage.stageWidth;
				missile.rotation = Math.random() * 90 + 135;
			}
		}
		
		private function updateShot():void {
			var dY:Number = shotSpeed * Math.cos(shot.rotation * Math.PI/180);
			var dX:Number = shotSpeed * Math.sin(shot.rotation * Math.PI/180);
			
			shot.x += dX;
			shot.y -= dY;
			
			//check if target intercepted
			if (shot.hitTestObject(missile)) {
				shot.x = -10;
			}
			
			//reset if moved outside area
			if (shot.y < 0 || shot.x < 0 || shot.x > stage.stageWidth) {
				shot.x = -10;
			}
		}
*/