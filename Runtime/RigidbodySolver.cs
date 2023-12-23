using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoroPhysics
{
  public class RigidbodySolver : MonoBehaviour
  {
    [Range(60, 1000)] public int operations = 250;
    public Rigidbody rb;

    private float lastCalledTime;
    private float lastCalledDelta;
    private Vector3 lastCalledForce;
    private Vector3 lastCalledTorque;
    private Vector3 newAngularVelocity;
    private Vector3 newLinearVelocity;

    private List<Action<float>> _callbacks = new List<Action<float>>();

    public Vector3 velocity
    {
      get { return newLinearVelocity; }
    }

    public Vector3 angularVelocity
    {
      get { return newAngularVelocity; }
    }

    public void Subscribe(Action<float> callback)
    {
      _callbacks.Add(callback);
    }

    public void Unsubscribe(Action<float> callback)
    {
      _callbacks.Remove(callback);
    }

    private void Start()
    {
      lastCalledTime = Time.time;
    }

    private void FixedUpdate()
    {
      float maxDeltaTime = 1f / operations;
      float newTime = Time.time;
      int steps = 0;

      newAngularVelocity = rb.angularVelocity;
      newLinearVelocity = rb.velocity;

      while (newTime - lastCalledTime >= maxDeltaTime)
      {
        foreach (var callback in _callbacks)
        {
          callback(maxDeltaTime);
        }

        lastCalledTime += maxDeltaTime;
        lastCalledDelta += maxDeltaTime;
        steps++;

        newAngularVelocity = PredictAngularVelocity(lastCalledTorque / steps, maxDeltaTime * steps);
        newLinearVelocity = PredictVelocity(lastCalledForce / steps, maxDeltaTime * steps);
      }

      if (steps > 0)
      {
        rb.AddForce(lastCalledForce / steps, ForceMode.Force);
        rb.AddTorque(lastCalledTorque / steps, ForceMode.Force);
      }

      lastCalledForce = Vector3.zero;
      lastCalledTorque = Vector3.zero;
      lastCalledDelta = 0f;
    }

    public void AddForce(Vector3 force, ForceMode forceMode = ForceMode.Force)
    {
      lastCalledForce += force;
    }

    public void AddTorque(Vector3 torque)
    {
      lastCalledTorque += torque;
    }

    public void AddForceAtPosition(Vector3 force, Vector3 position, ForceMode forceMode = ForceMode.Force)
    {
      AddForce(force);
      AddTorque(Vector3.Cross(position - rb.worldCenterOfMass, force));
    }

    public Vector3 GetVelocityAtPoint(Vector3 point)
    {
      return newLinearVelocity + Vector3.Cross(newAngularVelocity, point - rb.worldCenterOfMass);
    }

    public Vector3 GetVelocity()
    {
      return newLinearVelocity;
    }

    public Vector3 GetNewPosition(float delta)
    {
      return rb.position + delta * newLinearVelocity;
    }

    public Vector3 GetNewPosition()
    {
      return GetNewPosition(lastCalledDelta);
    }

    public Vector3 GetNewPositionAtPoint(Vector3 point)
    {
      Vector3 deltaTheta = lastCalledDelta * newAngularVelocity;
      Vector3 r = point - rb.worldCenterOfMass;

      r = RotateX(r, deltaTheta.x);
      r = RotateY(r, deltaTheta.y);
      r = RotateZ(r, deltaTheta.z);

      return r + rb.worldCenterOfMass + lastCalledDelta * newLinearVelocity;
    }

    private Vector3 PredictAngularVelocity(Vector3 torque, float delta)
    {
      Quaternion inertiaTensorWorldRotation = rb.rotation * rb.inertiaTensorRotation;
      Vector3 torqueInDiagonalSpace = Quaternion.Inverse(inertiaTensorWorldRotation) * torque;
      Vector3 angularVelocityChangeInDiagonalSpace;
      angularVelocityChangeInDiagonalSpace.x = torqueInDiagonalSpace.x / rb.inertiaTensor.x;
      angularVelocityChangeInDiagonalSpace.y = torqueInDiagonalSpace.y / rb.inertiaTensor.y;
      angularVelocityChangeInDiagonalSpace.z = torqueInDiagonalSpace.z / rb.inertiaTensor.z;

      return rb.angularVelocity + delta * (inertiaTensorWorldRotation * angularVelocityChangeInDiagonalSpace);
    }

    private Vector3 PredictVelocity(Vector3 force, float delta)
    {
      return rb.velocity + delta * force / rb.mass;
    }
    public Vector3 RotateX(Vector3 point, float angle)
    {
      float newY = point.y * Mathf.Cos(angle) - point.z * Mathf.Sin(angle);
      float newZ = point.y * Mathf.Sin(angle) + point.z * Mathf.Cos(angle);

      return new Vector3(point.x, newY, newZ);
    }

    public Vector3 RotateY(Vector3 point, float angle)
    {
      float newX = point.x * Mathf.Cos(angle) + point.z * Mathf.Sin(angle);
      float newZ = -point.x * Mathf.Sin(angle) + point.z * Mathf.Cos(angle);

      return new Vector3(newX, point.y, newZ);
    }

    public Vector3 RotateZ(Vector3 point, float angle)
    {
      float newX = point.x * Mathf.Cos(angle) - point.y * Mathf.Sin(angle);
      float newY = point.x * Mathf.Sin(angle) + point.y * Mathf.Cos(angle);

      return new Vector3(newX, newY, point.z);
    }
  }
}