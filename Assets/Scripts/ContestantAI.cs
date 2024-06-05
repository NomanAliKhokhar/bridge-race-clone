using UnityEngine;


public class ContestantAI : Character
{
    enum State
    {
        Decision,
        Move,
        Stop,
    }


    public LayerMask brickLayer;
    private Vector3 targetPosition;
    private State currentState;
    public int BrickMoveCount = 10;
    private BridgeController currentBridge;
    private float timeStuck;
    private Vector3 lastPosition;



    public override void Init(Color color)
    {
        base.Init(color);
        SetState(State.Decision);
        BrickMoveCount = Random.Range(1, BrickMoveCount);
    }
    
    private void Update()
    {
        if (!IsInit) return;
        switch (currentState)
        {
            case State.Decision:
                IdleState();
                break;
            case State.Move:
                Debug.Log("CheckSteps 1");

                if (currentBridge is not null && !CheckSteps())
                {
                    Debug.Log("CheckSteps 2");

                    currentBridge = null;
                    targetPosition = transform.position;
                    agent.SetDestination(targetPosition);
                    SetState(State.Decision);
                }
                else if (Vector3.Distance(transform.position, targetPosition) < agent.stoppingDistance)
                {
                    Debug.Log("CheckSteps 3");

                    currentBridge = null;
                    SetState(State.Decision);
                }

                //Stuck check!
                if (Vector3.Distance(transform.position, lastPosition) <= 0f)
                {
                    timeStuck += Time.deltaTime;
                    if (timeStuck > 2f)
                    {
                        SetState(State.Decision);
                    }
                }
                lastPosition = transform.position;
                break;
            default:
                break;
        }
    }

    private void SetState(State state)
    {

        switch (state)
        {
            case State.Decision:
                animator.SetBool(GameStatic.RUN, false);
                break;
            case State.Move:
                timeStuck = 0;
                animator.SetBool(GameStatic.RUN, true);
                agent.SetDestination(targetPosition);
                break;
            default:
                break;
        }
        currentState = state;
    }



    private void IdleState()
    {
        if (StackPoint.childCount > BrickMoveCount)
        {
            //yeterli tugla var kopruye git!
            currentBridge = GameManager.Instance.FindBestBridge(Color, currentFloor);
            targetPosition = currentBridge.TopPosition;
            SetState(State.Move);
            return;
        }

        if (Radar.FindNearestTarget(transform.position, 40f, brickLayer, Color, out Brick brick))
        {
            targetPosition = brick.transform.position;
            SetState(State.Move);
        }

    }


}


public static class Radar
{
    private static readonly Collider[] results = new Collider[120];
    public static bool FindNearestTarget(Vector3 origin, float radius, LayerMask targetLayer, Color color, out Brick brick)
    {
        int found = Physics.OverlapSphereNonAlloc(origin, radius, results, targetLayer);
        brick = null;
        if (found > 0)
        {
            float closestDistanceSqr = Mathf.Infinity;
            for (int i = 0; i < found; i++)
            {
                Vector3 directionToTarget = results[i].transform.position - origin;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    //T tmpTarget = results[i].transform.GetComponent<T>();
                    if (results[i].TryGetComponent(out Brick tmpTarget) && tmpTarget.Color.Equals(color))
                    {
                        closestDistanceSqr = dSqrToTarget;
                        brick = tmpTarget;
                    }
                }
            }
        }
        return brick is not null;
    }
}