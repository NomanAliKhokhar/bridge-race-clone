using Lean.Pool;
using UnityEngine;
using UnityEngine.AI;

public abstract class Character : MonoBehaviour
{
    public Transform StackPoint => stackPoint;

    public Color Color;
    [SerializeField] protected Animator animator;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] private Transform stackPoint;

    [SerializeField] private Transform rayPoint;
    [SerializeField] private LayerMask stepLayer;

    private SkinnedMeshRenderer skinnedMeshRenderer;

    [SerializeField] protected int currentFloor;

    protected bool IsInit;
#if UNITY_EDITOR
    private void OnValidate()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }
#endif

    protected virtual void Awake()
    {
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        //Color = GameManager.Instance.RandomColorAIColors;
    }
    public virtual void Init(Color color)
    {
        ClearStack();
        Color = color;
        skinnedMeshRenderer.material.SetColor(GameStatic.BASE_COLOR, Color);
        agent.isStopped = false;
        IsInit = true;

    }

    public virtual void Stop()
    {
        agent.isStopped = true;
        animator.SetBool(GameStatic.RUN, false);
        IsInit = false;
    }
    public void Despawn()
    {
        LeanPool.Despawn(this);
    }

    private void ClearStack()
    {
        int childCount = stackPoint.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            stackPoint.GetChild(i).GetComponent<Brick>().Despawn();
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {

        if (other.TryGetComponent(out Brick brick) && brick.Color.Equals(Color))
        {
            brick.Take(this);
            return;
        }
        if (other.TryGetComponent(out BridgeGate gate))
        {
            gate.GetComponentInParent<Floor>().DisableAllBridgePoints(gate);
            GameManager.Instance.UnlockNextFloor();
            return;
        }
        //if (other.TryGetComponent(out BridgeGateLast gate_last))
        //{

        //    GameManager.Instance.GameOver(this);
        //    return;
        //}
    }


    protected bool CheckSteps()
    {
        RaycastHit hit;
        if (Physics.Raycast(rayPoint.position, rayPoint.TransformDirection(Vector3.down), out hit, 10f, stepLayer, QueryTriggerInteraction.Ignore))
        {
            Debug.Log("CheckSteps A");

            Debug.DrawRay(rayPoint.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.green);

            if (hit.transform.TryGetComponent(out Step step))
            {
                Debug.Log("CheckSteps B");

                if (step.Color == Color)
                {
                    Debug.Log("CheckSteps C");

                    //basamak zaten bizim devam edebiliriz..
                    animator.SetBool(GameStatic.RUN, true);
                    return true;
                }
                Debug.Log("CheckSteps D");

                int childCount = stackPoint.childCount;
                if (childCount > 0 && stackPoint.GetChild(childCount - 1).TryGetComponent(out Brick brick))
                {
                    Debug.Log("CheckSteps E");

                    //basamak bizim degil ama tuglamiz var. 
                    brick.Despawn();
                    step.SetColor(Color);
                    animator.SetBool(GameStatic.RUN, true);
                    return true;
                }

                if (Vector3.Dot(transform.forward, step.transform.forward) > 0)
                {

                    Debug.Log("CheckSteps F");

                    //basamak bizim degil, tuglamiz da yok. 
                    //hala daha basamaga dogru ilerlemeye calisiliyor 
                    //return ediyoruz...
                    animator.SetBool(GameStatic.RUN, false);
                    return false;
                }
            }

            if (hit.transform.TryGetComponent(out Floor floor))
            {
                Debug.Log("CheckSteps G");

                currentFloor = floor.floorIndex;
            }

                //ground uzerindeyiz.. 
                animator.SetBool(GameStatic.RUN, true);
            return true;
        }
        Debug.DrawRay(rayPoint.position, transform.TransformDirection(Vector3.down) * 10f, Color.red);
        animator.SetBool(GameStatic.RUN, false);
        return false;
    }
}
