using UnityEngine;

public class CommuterAI : MonoBehaviour
{
    [Header("Commuter Data")]
    public Color commuterColor;
    public float walkSpeed = 1f;
    public float stopDistance = 0.3f;

    private WaitingShed shedScript;
    private Vector3 targetPosition;
    private bool hasJoinedLine = false;

    public void SetDestination(MonoBehaviour shedTarget)
    {
        if (shedTarget != null)
        {
            shedScript = shedTarget as WaitingShed;
            targetPosition = shedTarget.transform.position;
        }
    }

    void Update()
    {
        if (hasJoinedLine) return;

        if (shedScript != null)
        {
            WalkTowards(targetPosition);
        }
    }

    void WalkTowards(Vector3 destination)
    {
        float distance = Vector3.Distance(transform.position, destination);

        if (distance > stopDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, walkSpeed * Time.deltaTime);

            Vector3 lookDirection = new Vector3(destination.x, transform.position.y, destination.z);
            if (lookDirection != transform.position)
                transform.LookAt(lookDirection);
        }
        else
        {
            ArriveAtShed();
        }
    }

    void ArriveAtShed()
    {
        if (shedScript != null)
        {
            int slotIndex = shedScript.JoinLine(this);
            Vector3 slotPosition = shedScript.GetQueueSlotPosition(slotIndex);
            transform.position = slotPosition;
        }

        hasJoinedLine = true;
    }

    public void BoardJeepney()
    {
        Destroy(gameObject);
    }
}