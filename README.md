# Borororo Physics

This physics package was created when I was making the custom vehicle physics in Unity. The problem that I tried to solve was not enough calculations in one second of time. When I decreased the `Fixed Timestep` performance started to falling down. I learned about the substepping and tried to implement own. Here we are with this solution which is really simple, but helped me to reduce the load on the CPU.
I did not measure performance, but overall could implement code which required more frequent calculations and now is stable.

# Installation

1. Open `Window -> Package Manager`
2. Click the plus sign in top left corner
3. `Add package from Git URL`
4. Paste the link `https://github.com/MBorowiecki/BorororoPhysics.git`

# Usage

_Important note! When using this package, I recommend setting the `Fixed Timestep` in the `Project Settings -> Time` to at least `1/120`. Otherwise calculations may be incorrect_

### Rigidbody Solver

1. Add `Rigidbody Solver` script to the object
2. Specify frequency of calculations in the added script
3. Add calculations code

```cs
// Take the RigidbodySolver instance from the object
RigidbodySolver rigidbodySolver;

// Subscribe to the timestep event in Start function
private void Start() {
  rigidbodySolver.Subscribe(UpdatePhysics)
}

// Create the method which takes delta time param
private void UpdatePhysics(float delta) {
  // ...Physics code
  // All code inside this function can access the methods from rigidbodySolver.
  // If you want to use the same solver in methods in child elements, remember to pass the RigidbodySolver instance to them and call physics methods inside this root UpdatePhysics method.
}

// Variables that can be accessed:
rigidbodySolver.frequency; // Frequency of calculations in one second
rigidbodySolver.rb; // Rigidbody associated with the RigidbodySolver
rigidbodySolver.velocity; // Linear velocity of Rigidbody in world coordinates
rigidbodySolver.angularVelocity; // Angular velocity of Rigidbody in world coordinates

// Methods that can be accessed:
rigidbodySolver.Subscribe(Action<float> action); // Subscribes to the substep physics solver method. Takes one argument of type Action<float>, argument has to be the method with one float (delta time) argument.
rigidbodySolver.Unsubscrive(Action<float> action); // Unsubscrives from the substep physics solver method.
rigidbodySolver.AddForce(Vector3 force, ForceMode? forceMode); // Adds linear force. Force provided must be in the world coordinates.
rigidbodySolver.AddTorque(Vector3 torque); // Adds torque. Torque must be in the world coordinates.
rigidbodySolver.AddForceAtPosition(Vector3 force, Vector3 position, ForceMode? forceMode); // Adds linear force and torque to the Rigidbody in specified offset from center of mass. Force must be in the world coordinates. Position must be the offset from the center of mass in world coordinates.
rigidbodySolver.GetVelocityAtPoint(Vector3 point); // Returns the linear velocity at specified point with offset from Rigidbody center of mass. Point must be a offset from the center of mass in world coordinates.
rigidbodySolver.GetVelocity(); // Returns the linear velocity in world coordinates.
rigidbodySolver.GetNewPosition(); // Returns the position of rigidbody in world coordinates.
rigidbodySolver.GetNewPositionAtPoint(Vector3 point); // Returns new position of rigidbody in point. Point must be offset from center of mass in world coordinates.
```

# Issues

If you have any issues with the script or suggestions you can contact me directly using the email `borowieckimateusz1@gmail.com` or by creating the issue in this repo.
