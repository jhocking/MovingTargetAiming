# MovingTargetAiming
WebGL build at https://jhocking.itch.io

Here's a demo of the recursive algorithm I devised for intercepting a moving target. Decades ago I had programmed this as a little cheat/mod for an open-source Missile Command style game, years later implemented it in AS3 to make a Flash demo, and just now decided to port it to C# for use in Unity.

In the interactive demo, the cannon constantly aims correctly to hit the missiles raining down. Press any key to fire a shot and watch it hit the missile.

Here's a QA page where I discuss the algorithm (plus link to the original Flash demo):<br>
https://gamedev.stackexchange.com/questions/28481/how-to-lead-a-moving-target-from-a-moving-shooter/28582#28582

A commenter there pointed out a big advantage of my approach (versus how other people were doing it): even if there is no exact solution, the shooter will still fire roughly at the target.
