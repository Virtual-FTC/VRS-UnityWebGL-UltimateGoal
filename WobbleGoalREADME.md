# Wobble Goals and Weighted Cubes 

This branch contains the following new assets:

* WobbleGoal prefab
* "TestGoalPhysics" Scene
* Weighted Cubes prefab
* 5 new layers

# Wobble Goal

The wobble goal uses the existing 3D object (from Blender) as a visual source only-- there are no physics / collider information on the existing object.

The goal physics are defined in three main shapes:

1. The base GameObject "WobbleGoal" has a Rigidbody and a sphere collider on the "WobbleSphere" layer that interacts with the "floor" layer only (not "WobblePhysics" nor "default", as we don't want anything else interacting with it). The sphere creates the "wobble" when the goal is on the floor.

2. The "Spine" object, which represents the middle of the goal, is on the layer "GoalPhysics" (which interacts with everything except the "WobbleSphere" layer and itself).

3. The "Tray" objects ("Bottom", "Center", and "Top") all have a collection of primitive objects on the "GoalPhysics" layer that approximate the shape of their respective trays. These shapes exist in the editor as a small segment of the tray and are then revolved via a circular array script at Runtime Start. The Inertia Tensor of the base Rigidbody component (from #1 above) is recalculated after each circular array is created, thus ensuring the object has proper physics.

# Weighted Cube

The cube prefab has two new layers: TargetPhysics and InnerTargetPhysics. The TargetPhysics layer gives extra control for how these targets should interact with other layers. The InnerTargetPhysics is only for the inner capsule colliders so they don't interact with the TargetPhysics colliders. 

The inner capsules can be turned on and off to change the inertia tensor for the whole object, so it behaves like a real weighted cube with different centers of mass. 

The mass of these cube prefabs can be set in the Rigidbody component of the base GameObject; mass units are arbitrary and need to be set in ratio to each other and the force units of the robot.

# Layers

See the image below for the proper physics layer configuration.

![wobble goal physics](/Documentation/images/WobbleGoalPhysics.png "Physics Project Settings")

