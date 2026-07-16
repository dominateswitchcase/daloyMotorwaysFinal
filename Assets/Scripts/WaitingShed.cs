using UnityEngine;
using System.Collections.Generic;

public class WaitingShed : MonoBehaviour
{
    [Header("Capacity")]
    public int maxCapacity = 9;
    public float queueSpacing = 1f;

    private int reservedSpots = 0;

    [Tooltip("Colors of nearby buildings. Jeepneys will drop off passengers matching these colors.")]
    public List<Color> acceptedColors = new List<Color>();

    public List<CommuterAI> waitingLine { get; private set; } = new List<CommuterAI>();

    public bool HasSpace()
    {
        return (waitingLine.Count + reservedSpots) < maxCapacity;
    }

    public void ReserveSpot()
    {
        reservedSpots++;
    }

    public int JoinLine(CommuterAI commuter)
    {
        if (reservedSpots > 0) reservedSpots--;
        waitingLine.Add(commuter);
        return waitingLine.Count - 1;
    }

    public void LeaveLine(CommuterAI commuter)
    {
        if (waitingLine.Contains(commuter)) waitingLine.Remove(commuter);
    }

    public CommuterAI BoardPassenger()
    {
        if (waitingLine.Count > 0)
        {
            CommuterAI next = waitingLine[0];
            waitingLine.RemoveAt(0);
            return next;
        }
        return null;
    }

    public Vector3 GetQueueSlotPosition(int index)
    {
        return transform.position + (transform.right * index * queueSpacing);
    }

    public bool AcceptsColor(Color commuterColor)
    {
        foreach (Color color in acceptedColors)
        {
            if (color == commuterColor) return true;
        }
        return false;
    }
}