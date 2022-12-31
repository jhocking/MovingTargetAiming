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
    [SerializeField] GameObject cannon;
    [SerializeField] GameObject shot;
    [SerializeField] GameObject missile;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
