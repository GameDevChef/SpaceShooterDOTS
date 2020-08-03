using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class PlayerMovementSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;
        Vector2 mouseDirection = new Vector2(Input.mousePosition.x - Screen.width / 2f, Input.mousePosition.y - Screen.height / 2f) / Screen.height;
        bool forward = Input.GetKey(KeyCode.W);
        bool backward = Input.GetKey(KeyCode.S);
        bool right = Input.GetKey(KeyCode.D);
        bool left = Input.GetKey(KeyCode.A);

        var jobHandle = Entities.WithName("PlayerMovementSystem").
            ForEach((ref Translation translation, ref Rotation rotation, ref PlayerShipData shipData) =>
            {
                float yaw = mouseDirection.x * deltaTime * shipData.rotationSpeed;
                float currentYaw = (shipData.currentRotationY += yaw);

                float pitch = mouseDirection.y * deltaTime * shipData.rotationSpeed;
                float currentPitch = (shipData.currentRotationX -= pitch);

                float targetRoll = 0f;
                if (right)
                    targetRoll = -45f;
                else if (left)
                    targetRoll = 45f;
                float currentRoll = Mathf.MoveTowards(shipData.currentRotationZ, targetRoll, deltaTime * 20f);

                rotation.Value = Quaternion.Euler(currentPitch, currentYaw, currentRoll);

                float targetThrottle = shipData.throttle;
                if (forward)
                    targetThrottle = 1f;
                else if (backward)
                    targetThrottle = -1f;

                float throttle = Mathf.MoveTowards(shipData.throttle, targetThrottle, deltaTime * shipData.throttleSpeed);
                var forwardVector = math.forward(rotation.Value) * throttle;
                translation.Value += forwardVector * shipData.moveSpeed;
                shipData.throttle = throttle;
                shipData.currentRotationY = currentYaw;
                shipData.currentRotationX = currentPitch;
                shipData.currentRotationZ = currentRoll;
            }).Schedule(inputDeps);
        jobHandle.Complete();
        return jobHandle;
    }
}