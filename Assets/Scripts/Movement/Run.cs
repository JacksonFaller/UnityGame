using UnityEngine;

public class Run : MonoBehaviour
{
    [SerializeField]
    private float _runSpeed = 20f;

    [SerializeField]
    private float _accelerationSpeed = 5f;

    [SerializeField]
    private float _decelerationnSpeed = 10f;

    [SerializeField]
    private float _airSpeedMultiplier = 0.5f;

    [SerializeField]
    private float _airControlLerp = 10;

    private RunState _runState = RunState.Deceleration;

    public Vector2 Walk(float x, Vector2 velocity, MovementContext context)
    {
        if (!context.CanMove) return velocity;

        if (!context.Collision.OnGround)
        {
            return Vector2.Lerp(velocity,
               new Vector2(x * _runSpeed * _airSpeedMultiplier, velocity.y), _airControlLerp * Time.deltaTime);
        }

        return GetRunVelocity(x, velocity);
    }

    public Vector2 GetRunVelocity(float x, Vector2 velocity)
    {
        float horizontalAxisRaw = Input.GetAxisRaw(Configuration.Input.HorizontalAxis);
        switch (_runState)
        {
            case RunState.Run:
            {
                if (horizontalAxisRaw == 0 ||
                    (horizontalAxisRaw < 0 && velocity.x > 0) ||
                    (horizontalAxisRaw > 0 && velocity.x < 0))
                {
                    _runState = RunState.Deceleration;
                }
                else
                {
                    return new Vector2(_runSpeed * x, velocity.y);
                }
                break;
            }
            case RunState.Acceleration:
            {
                if (horizontalAxisRaw == 0 || (x < 0 && velocity.x > 0) || (x > 0 && velocity.x < 0))
                {
                    _runState = RunState.Deceleration;
                }
                else
                {
                    if (Mathf.Abs(velocity.x) == _runSpeed)
                    {
                        _runState = RunState.Run;
                    }
                    else
                    {
                        return Vector2.Lerp(velocity,
                            new Vector2(_runSpeed * x, velocity.y), Time.deltaTime * _accelerationSpeed);
                    }
                }
                break;
            }
            case RunState.Deceleration:
            {
                if ((horizontalAxisRaw > 0 && velocity.x >= 0) || (horizontalAxisRaw < 0 && velocity.x <= 0))
                {
                    _runState = RunState.Acceleration;
                }
                else
                {
                    return Vector2.Lerp(velocity,
                        new Vector2(0, velocity.y), Time.deltaTime * _decelerationnSpeed);
                }
                break;
            }
        }

        return velocity;
    }

    public enum RunState
    {
        Acceleration,
        Deceleration,
        Run
    }
}
