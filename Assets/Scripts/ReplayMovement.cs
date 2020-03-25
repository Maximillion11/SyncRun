using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayMovement : MonoBehaviour
{
    public bool Play = false;
    public int LevelNumber = 0;
    public int PlayerNumber = 0;
    public List<float> PositionsX = new List<float>();
    public List<float> PositionsY = new List<float>();
    public float RecordInterval = 0f;
    public Vector2 kinematicVelocity = Vector2.zero;

    private float timer = 0;
    private int frame = 0;

    private Vector3 lastPosition = Vector3.zero;

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        kinematicVelocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;

        if (Play)
        {
            if (timer >= RecordInterval)
            {
                PlayFrames();
                timer = 0;
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
    }

    private void PlayFrames()
    {
        if (frame < PositionsX.Count)
        {
            Vector2 position = new Vector2(PositionsX[frame], PositionsY[frame]);

            StartCoroutine(InterpToFrame(transform, position, RecordInterval));

            frame++;
        }
    }

    public void Reset()
    {
        Play = false;
        frame = 0;
        timer = 0;

        Vector2 position = new Vector2(PositionsX[frame], PositionsY[frame]);

        transform.position = position;
    }

    private IEnumerator InterpToFrame(Transform transform, Vector3 position, float timeToMove)
    {
        var currentPos = transform.position;
        var t = 0f;
        while (t < 1 && Play)
        {
            t += Time.deltaTime / timeToMove;
            transform.position = Vector3.Lerp(currentPos, position, t);
            yield return null;
        }
    }
}
