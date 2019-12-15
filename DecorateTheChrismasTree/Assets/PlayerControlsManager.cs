using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerControlsManager : MonoBehaviour
{
    [Header("Params")]
    [SerializeField] float rotationSpeed = 0.5f;
    [SerializeField] float rotationDegree = 90f;
    [SerializeField] float timerDelay = 2f;
    [SerializeField] float step = 0.2f;

    [SerializeField] int _currentPos = 0;
    int currentPos {
        get => _currentPos;
        set {
            if (value > rows.Count - 1)
                _currentPos = 0;
            else if (value < 0)
                _currentPos = rows.Count - 1;
            else
                _currentPos = value;
        }
    }

    [Space]
    [SerializeField] List<GameObject> propsPrefabs;
    [SerializeField] List<RowData> rows = new List<RowData>();

    // Виділити в окремі данні значення кроку step для кожної ялинки
    // а також максимальні та мінімальні значення висот maxHeight, minHeight

    float horizontalRotation;
    float verticalMove;
    float controlsSpeedTimer = 0f;
    Transform viewPoint;

    const float maxHeight = 0.9f;
    const float minHeight = -1.1f;

    Vector3 lookDir => new Vector3(transform.position.x - viewPoint.position.x, 0f, transform.position.z - viewPoint.position.z);


    private void Awake()
    {
        viewPoint = transform.GetChild(0);

        // Initializate the rows
        for (float i = minHeight + 0.05f, rowInd = 0f; i < maxHeight; i += step, rowInd++) {
            rows.Add(new RowData((int)rowInd));
        }
        rows.Reverse();
    }
    
    void Update()
    {
        RoundingInput();
        Debug.DrawRay(viewPoint.position, lookDir);
    }

    void FixedUpdate()
    {
        PlayerControls();

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            HangUpDecoration();
        }
    }

    private void PlayerControls()
    {
        if (controlsSpeedTimer >= timerDelay) {
            if(horizontalRotation != 0) {
                RotateToDirection(horizontalRotation);
                ChangingIndexInRow((int)horizontalRotation);
                //Debug.Log("In-row position is " + rows[currentPos].currentIndex);
            }

            if (verticalMove != 0) {
                Vector3 tempStep = new Vector3(0f, step * verticalMove, step * 0.5f * verticalMove);
                viewPoint.localPosition += tempStep;
                if (viewPoint.localPosition.y > maxHeight || viewPoint.localPosition.y < minHeight)
                    viewPoint.localPosition -= tempStep;
                else {
                    currentPos += (int)verticalMove;
                    if ((verticalMove < 0 && (currentPos + 1) % 2 != 0) ||
                         verticalMove > 0 && (currentPos - 1) % 2 == 0) {
                        RotateToDirection(verticalMove);
                        Debug.Log("Move up/down and right");
                    }
                    else if (verticalMove < 0 && (currentPos + 1) % 2 == 0 ||
                             verticalMove > 0 && (currentPos - 1) % 2 != 0) {
                            RotateToDirection(-verticalMove);
                            Debug.Log("Move up/down and left");
                    }
                    //Debug.Log("Current row is " + currentPos);
                }
            }

            controlsSpeedTimer = 0f;
        }
        else
            controlsSpeedTimer += Time.deltaTime + rotationSpeed;
    }

    private void RotateToDirection(float direction)
    {
        transform.Rotate(new Vector3(0f, -rows[currentPos].rowAngle * direction));
    }

    private void ChangingIndexInRow(int increment)
    {
        RowData row = rows[currentPos];
        row.currentIndex += increment;
        rows[currentPos] = row;
    }

    private void HangUpDecoration()
    {
        if (!rows[currentPos].decorations[rows[currentPos].currentIndex] && 
            Physics.Raycast(viewPoint.position, lookDir, out RaycastHit hit, 10f)) {
            if (hit.collider.gameObject.tag == "XmasTree") {
                Instantiate(
                            propsPrefabs[Convert.ToInt32(UnityEngine.Random.Range(0f, propsPrefabs.Count - 1))],
                            hit.point, viewPoint.rotation
                            );
                rows[currentPos].decorations[rows[currentPos].currentIndex] = true;
            }
        }
    }

    private void RoundingInput()
    {
        horizontalRotation = Input.GetAxis("Horizontal");
        if (horizontalRotation > 0f)
            horizontalRotation = 1f;
        else if (horizontalRotation < 0f)
            horizontalRotation = -1f;

        verticalMove = Input.GetAxis("Vertical");
        if (verticalMove > 0f)
            verticalMove = 1f;
        else if (verticalMove < 0f)
            verticalMove = -1f;
    }
}
