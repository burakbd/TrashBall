using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BallScript : MonoBehaviour
{
    public GameObject yes;
    int _rotationSpeed = 15;
    AudioSource atmaSesi;
    AudioSource hata;
    AudioSource basket;
    AudioSource yesSesi;

    private struct QuadraticCurveData
    {
        public Vector2 startVector;     
        public Vector2 middleVector;    
        public Vector2 endVector;       
    }

   
    private Vector3 ballSpawnPosition;

   
    private Vector2 swipeStartPosition;
   
    private Vector2 swipeEndPosition;
    
    private Vector2 overallSwipeDirection;
  
    private Vector2 swipeCurveRight, swipeCurveLeft;

   
    private Rigidbody ballRigidBody;

  
    private bool isBallInMovement = false;
    
    private bool isBallThrowCurved = false;
   
    private bool isLastForceApplied = false;
   
    private bool isGoalScored = false;
  
    private bool isRespawnCorrutineActive = false;
  
    private bool isRespawnCancelled = false;



    private QuadraticCurveData curveData;


    private float swipeStartTime, swipeIntervalTime, movementStartTime;

    [SerializeField] private float throwForceX;

    [SerializeField] private float throwForceY;

    [SerializeField] private float throwForceZ;

    private float raycastDepth;


    [SerializeField] private int startingLives;
    private int currentLives;
    [SerializeField] private int respawnTime;


    private TrailRenderer trailRenderer;
    private bool alreadyDidOneTime = false;


    void Start()
    {
        ballSpawnPosition = gameObject.transform.position;
        ballRigidBody = GetComponent<Rigidbody>();
        trailRenderer = gameObject.GetComponent<TrailRenderer>();
        currentLives = startingLives;
        atmaSesi = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
        hata = GameObject.FindGameObjectWithTag("Hata").GetComponent<AudioSource>();
        basket = GameObject.FindGameObjectWithTag("Collider").GetComponent<AudioSource>();
        yesSesi = GameObject.FindGameObjectWithTag("YesSesi").GetComponent<AudioSource>();
        atmaSesi.enabled = false;
    }


    void Update()
    {
        Application.targetFrameRate = 60;
        if (alreadyDidOneTime == false)
        {
            transform.Rotate(0, _rotationSpeed * Time.deltaTime, 0);
            transform.Rotate(_rotationSpeed * Time.deltaTime, 0, 0);
            transform.Rotate(0, 0, _rotationSpeed * Time.deltaTime);
        }


        if (this.isPlayerAlive())
        {

            if (Input.touchCount > 0 && !isBallInMovement)
            {
                Touch actualTouch = Input.GetTouch(0);

                if (actualTouch.phase == TouchPhase.Began)
                {
                    this.GetTouchStartData(actualTouch);
                    atmaSesi.enabled = true;
                }
                else
                {

                    this.CompareActualTouchToHighestCurves(actualTouch);

                    if (actualTouch.phase == TouchPhase.Ended)
                    {
                        this.GetTouchEndData(actualTouch);
                        this.CalculateBallDirectionAndShoot();
                        this.StartCoroutine(AwaitToSpawnBall());
                        atmaSesi.enabled = false;
                    }
                }
            }

            else if (isBallInMovement && isBallThrowCurved)
            {
                float timeElapsed = (Time.time - movementStartTime)/ swipeIntervalTime;

                if (timeElapsed <= 0.8f)
                {
                    this.CurveQuadraticBall(timeElapsed, curveData);
                    atmaSesi.enabled = true;
                }

                else if (!isLastForceApplied)
                {
                    isLastForceApplied = true;
                    ballRigidBody.AddForce(-(curveData.middleVector - swipeEndPosition).x / 15, 0 , 1);
                }
            }
        }
    }


    private bool isPlayerAlive()
    {
        return currentLives > 0;
    }

    private void CompareActualTouchToHighestCurves(Touch touch)
    {
        if (touch.position.x > swipeCurveRight.x)
        {
            swipeCurveRight = touch.position;
        }
        else if (touch.position.x < swipeCurveLeft.x)
        {
            swipeCurveLeft = touch.position;
        }
    }


    private void GetTouchStartData(Touch touch)
    {
        swipeStartTime = Time.time;
        swipeStartPosition = touch.position;
        swipeCurveRight = touch.position;
        swipeCurveLeft = touch.position;
    }

    private void GetTouchEndData(Touch touch)
    {
        swipeIntervalTime = Time.time - this.swipeStartTime;
        swipeEndPosition = touch.position;
        overallSwipeDirection = swipeStartPosition - swipeEndPosition;
    }


    private void CalculateBallDirectionAndShoot()
    {

        if (Math.Abs(swipeStartPosition.x - swipeCurveLeft.x) <= Math.Abs(swipeCurveRight.x - swipeStartPosition.x))
        {

            if (swipeCurveRight.y >= swipeEndPosition.y)
            {
                this.ShootStraightBall(); 
            }

            else
            {
                swipeCurveRight.x += (swipeCurveRight.x - swipeStartPosition.x);
                this.ShootCurvedBall(swipeCurveRight);
            }
        }

        else
        {

            if (swipeCurveLeft.y >= swipeEndPosition.y)
            {
                this.ShootStraightBall();
            }

            else
            {
                swipeCurveLeft.x -= (swipeStartPosition.x - swipeCurveLeft.x);
                this.ShootCurvedBall(swipeCurveLeft);                
            }
        }
    }


    private void ShootStraightBall()
    {

        isBallInMovement = true;
        ballRigidBody.isKinematic = false;

        ballRigidBody.AddForce(
            -overallSwipeDirection.x * throwForceX,
            -overallSwipeDirection.y * throwForceY,
            -overallSwipeDirection.y * throwForceZ / swipeIntervalTime
        );


        movementStartTime = Time.time;
    }


    private void ShootCurvedBall(Vector2 curveVector)
    {

        isBallInMovement = true;
        ballRigidBody.isKinematic = false;
        isBallThrowCurved = true;

        ballRigidBody.AddForce(
            0f, 
            -overallSwipeDirection.y * throwForceY,
            -overallSwipeDirection.y * throwForceZ / swipeIntervalTime
        );


        movementStartTime = Time.time;

        this.SetQuadraticCuvedBallData(swipeStartPosition, curveVector, swipeEndPosition);
    }


    private void SetQuadraticCuvedBallData(Vector2 startingVector, Vector2 mVector, Vector2 endingVector)
    {     
        curveData = new QuadraticCurveData()
        {
            startVector = startingVector,
            middleVector = mVector,
            endVector = endingVector
        };
    }


    private void CurveQuadraticBall(float time, QuadraticCurveData curveData)
    {
        Vector3 curve = this.CalculateQuadraticBezierCurve(
            time,
            Camera.main.ScreenToWorldPoint(new Vector3(curveData.startVector.x, curveData.startVector.y, 1)),
            Camera.main.ScreenToWorldPoint(new Vector3(curveData.middleVector.x, curveData.middleVector.y, 1)),
            Camera.main.ScreenToWorldPoint(new Vector3(curveData.endVector.x, curveData.endVector.y, 1))
        );

        gameObject.transform.position += new Vector3(curve.x - gameObject.transform.position.x, 0f, 0f);
    }

    private Vector3 CalculateQuadraticBezierCurve(float time, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float timeMinusOne = 1 - time;
        Vector3 a = timeMinusOne * timeMinusOne * p0;
        Vector3 b = 2 * timeMinusOne * time * p1;
        Vector3 c = time * time * p2;

        return a + b + c;
    }

    public int SubstractLife(int livesToSubstract)
    {
        if (livesToSubstract > currentLives)
        {
            currentLives = 0;
        }
        else
        {
            currentLives -= livesToSubstract;
        }
        return currentLives;
    }

    public void RestartLives()
    {
        currentLives = startingLives;
    }

    public void SetIsRespawnCanceled(bool isRespawnCancelled)
    {
        this.isRespawnCancelled = isRespawnCancelled;
    }

    public void CancelRespawnCorrutineIfActive()
    {
        if (isRespawnCorrutineActive)
        {
            this.SetIsRespawnCanceled(true);
        }
    }

    public void SetIsGoalScored(bool goalScored)
    {
        this.isGoalScored = goalScored;
        if (goalScored)
        {
            this.RestartLives();
        }
    }

    public GameObject Ouch;
    private IEnumerator AwaitToSpawnBall()
    {
        isRespawnCorrutineActive = true;
        yield return new WaitForSeconds(respawnTime);

        if (!isRespawnCancelled)
        {
            Ouch.SetActive(false);
            this.RespawnBall();
        }
        else
        {
            isRespawnCancelled = false;
        }
        isRespawnCorrutineActive = false;
    }


    public void RespawnBall()
    {
        hata.enabled = false;
        basket.enabled = false;
        yesSesi.enabled = false;
        yes.SetActive(false);
        isBallInMovement = false;
        isLastForceApplied = false;
        isBallThrowCurved = false;
        ballRigidBody.velocity = Vector3.zero;
        ballRigidBody.angularVelocity = Vector3.zero;
        ballRigidBody.AddForce(0f, 0f, 0f);
        ballRigidBody.isKinematic = true;
        gameObject.transform.position = ballSpawnPosition;
        trailRenderer.Clear();
    }
    
}
