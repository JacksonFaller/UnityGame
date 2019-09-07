using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeBounce : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D _playerRigidbody2D = null;

    [SerializeField]
    private float _boostPower = 20f;

    [SerializeField]
    private float _boostTime = 0.3f;

    public bool IsLedgeBouncing { get; private set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Configuration.Tags.JumpTrigger))
        {
            if (_playerRigidbody2D.velocity.y > 0 && _playerRigidbody2D.velocity.x != 0)
            {
                _playerRigidbody2D.velocity += new Vector2(0, _boostPower);
                StartCoroutine(EnableLedgeBounce(_boostTime));
            }
        }
    }

    private IEnumerator EnableLedgeBounce(float time)
    {
        IsLedgeBouncing = true;
        yield return new WaitForSeconds(time);
        IsLedgeBouncing = false;
    }
}
