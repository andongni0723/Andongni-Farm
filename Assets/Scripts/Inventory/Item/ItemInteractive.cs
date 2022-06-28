using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteractive : MonoBehaviour
{
    private bool isAnimating;

    private WaitForSeconds pause = new WaitForSeconds(0.04f);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isAnimating)
        {
            if (other.transform.position.x < transform.position.x)
            {
                // Person at left; Shake to right
                StartCoroutine(RotateRight());
            }
            else
            {
                // Person at right; Shake to left
                StartCoroutine(RotateLeft());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!isAnimating)
        {
            if (other.transform.position.x > transform.position.x)
            {
                // Person at left; Shake to right
                StartCoroutine(RotateRight());
            }
            else
            {
                // Person at right; Shake to left
                StartCoroutine(RotateLeft());
            }
        }
    }

    private IEnumerator RotateLeft()
    {
        isAnimating = true;

        for (int i = 0; i < 4; i++)
        {
            transform.GetChild(0).Rotate(0, 0, 2);
            yield return pause;
        }

        for (int i = 0; i < 5; i++)
        {
            transform.GetChild(0).Rotate(0, 0, -2);
            yield return pause;
        }

        transform.GetChild(0).Rotate(0, 0, 2);
        yield return pause;

        isAnimating = false;
    }

    private IEnumerator RotateRight()
    {
        isAnimating = true;

        for (int i = 0; i < 4; i++)
        {
            transform.GetChild(0).Rotate(0, 0, -2);
            yield return pause;
        }

        for (int i = 0; i < 5; i++)
        {
            transform.GetChild(0).Rotate(0, 0, 2);
            yield return pause;
        }

        transform.GetChild(0).Rotate(0, 0, -2);
        yield return pause;

        isAnimating = false;
    }
}
