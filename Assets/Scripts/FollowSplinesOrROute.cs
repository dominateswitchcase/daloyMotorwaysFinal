using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class FollowSplinesOrROute : MonoBehaviour
{
    private enum State { Moving, Stopped, Reversing }

    [System.Serializable]
    public struct SplineConnection
    {
        [Tooltip("The spline this connection starts from.")]
        public int fromSplineIndex;

        [Tooltip("Normalized position (0-1) on the FROM spline where the switch happens.")]
        [Range(0f, 1f)] public float fromT;

        [Tooltip("The spline this connection leads to.")]
        public int toSplineIndex;

        [Tooltip("Normalized position (0-1) on the TO spline where it enters.")]
        [Range(0f, 1f)] public float toT;
    }

    [SerializeField] private SplineContainer m_splineContainer;
    [SerializeField] private int m_splineIndex;

    [Header("Movement")]
    [SerializeField] private float m_speed = 1f;
    [SerializeField] private bool m_alignToTangent = true;
    [Tooltip("Fixes models that aren't authored facing Unity's forward (+Z). Try (0,-90,0) or (0,90,0) if the object looks rotated sideways while moving.")]
    [SerializeField] private Vector3 m_rotationOffset = Vector3.zero;

    [Header("Obstacle Detection")]
    [SerializeField] private float m_detectionDistance = 4f;
    [SerializeField] private float m_detectionRadius = 0.5f;
    [SerializeField] private LayerMask m_obstacleLayers = ~0;
    [SerializeField] private bool m_drawDetectionGizmo = true;

    [Header("Route")]
    [Tooltip("Set this to the spline index you want the object to head toward. Change it (Inspector or script) to trigger a reverse-and-switch. Leave it equal to the current spline to just stay on the current route.")]
    [SerializeField] private int m_routeToTake;

    [Header("Spline Connections")]
    [Tooltip("Add one entry per connection point. E.g. spline 0 -> spline 1, spline 1 -> spline 2, spline 2 -> spline 0 (wraparound). Click + to add a new one, then set its from/to spline indices and t values.")]
    [SerializeField] private List<SplineConnection> m_connections = new List<SplineConnection>();

    [Header("Debug")]
    [SerializeField] private bool m_debugLogs = true;

    private State m_state = State.Moving;
    private float m_t;
    private bool m_isBlocked;

    private int m_lastTargetSplineIndex;
    private SplineConnection m_activeConnection;
    private bool m_hasActiveConnection;

    private void Start()
    {
        // start with no pending route change - target matches whatever spline we're already on
        m_routeToTake = m_splineIndex;
        m_lastTargetSplineIndex = m_splineIndex;
    }

    private void Update()
    {
        if (m_splineContainer == null) return;

        float splineLength = m_splineContainer.CalculateLength(m_splineIndex);
        if (splineLength <= 0f) return;

        m_isBlocked = CheckForObstacle();

        CheckForRouteChangeRequest();

        switch (m_state)
        {
            case State.Moving:
                HandleMoving(splineLength);
                break;

            case State.Stopped:
                HandleStopped();
                break;

            case State.Reversing:
                HandleReversing(splineLength);
                break;
        }

        EvaluateAndApplyPosition();
    }

    // call this from another script to request a lane change, e.g. m_follower.RequestRouteChange(2);
    public void RequestRouteChange(int newSplineIndex)
    {
        m_routeToTake = newSplineIndex;
    }

    // detects when m_routeToTake has been changed (Inspector or script) since last frame
    private void CheckForRouteChangeRequest()
    {
        if (m_routeToTake == m_lastTargetSplineIndex) return;

        m_lastTargetSplineIndex = m_routeToTake;

        if (m_routeToTake == m_splineIndex)
        {
            Log("Target route set back to current spline, nothing to do.");
            return;
        }

        if (!TryFindConnection(m_splineIndex, m_routeToTake, out m_activeConnection))
        {
            Log($"No connection defined from spline {m_splineIndex} to spline {m_routeToTake}. Ignoring route change.", true);
            return;
        }

        m_hasActiveConnection = true;
        Log($"Route change requested: spline {m_splineIndex} -> spline {m_routeToTake}. Reversing to switch point at t={m_activeConnection.fromT:F3}.");
        m_state = State.Reversing;
    }

    private void HandleMoving(float splineLength)
    {
        if (m_isBlocked)
        {
            Log($"Obstacle detected on spline {m_splineIndex}. Stopping.");
            m_state = State.Stopped;
            return;
        }

        m_t += (m_speed / splineLength) * Time.deltaTime;

        // loop back to the start instead of stopping at the end
        if (m_t > 1f)
        {
            m_t -= 1f;
        }
    }

    private void HandleStopped()
    {
        if (!m_isBlocked)
        {
            Log("Obstacle cleared. Resuming movement on current route.");
            m_state = State.Moving;
        }

        // otherwise stays frozen here - waiting for the obstacle to clear,
        // or for m_routeToTake to change (handled in CheckForRouteChangeRequest)
    }

    private void HandleReversing(float splineLength)
    {
        if (!m_hasActiveConnection) { m_state = State.Stopped; return; }

        float switchT = m_activeConnection.fromT;
        float direction = (switchT < m_t) ? -1f : 1f;
        m_t += direction * (m_speed / splineLength) * Time.deltaTime;

        bool reachedSwitchPoint = direction < 0f ? m_t <= switchT : m_t >= switchT;

        if (reachedSwitchPoint)
        {
            m_t = switchT;
            Log($"Reached switch point. Switching to spline {m_activeConnection.toSplineIndex}.");

            int previousIndex = m_splineIndex;
            m_splineIndex = m_activeConnection.toSplineIndex;
            m_t = m_activeConnection.toT;

            Log($"Switched from spline {previousIndex} to spline {m_splineIndex} at t={m_t:F3}.");
            m_state = State.Moving;
        }
    }

    // now matches on BOTH from and to, since a single spline could have multiple possible connections
    private bool TryFindConnection(int fromIndex, int toIndex, out SplineConnection connection)
    {
        for (int i = 0; i < m_connections.Count; i++)
        {
            if (m_connections[i].fromSplineIndex == fromIndex && m_connections[i].toSplineIndex == toIndex)
            {
                connection = m_connections[i];
                return true;
            }
        }

        connection = default;
        return false;
    }

    private void EvaluateAndApplyPosition()
    {
        m_splineContainer.Evaluate(m_splineIndex, m_t, out float3 position, out float3 tangent, out float3 upVector);

        transform.position = position;

        if (m_alignToTangent && !tangent.Equals(float3.zero))
        {
            float3 facingDir = (m_state == State.Reversing) ? -tangent : tangent;
            Quaternion lookRotation = Quaternion.LookRotation(facingDir, upVector);

            // apply a fixed offset to correct for the model's default orientation,
            // while still allowing full rotation through curves
            transform.rotation = lookRotation * Quaternion.Euler(m_rotationOffset);
        }
    }

    private bool CheckForObstacle()
    {
        return Physics.SphereCast(
            transform.position,
            m_detectionRadius,
            transform.forward,
            out RaycastHit hit,
            m_detectionDistance,
            m_obstacleLayers
        );
    }

    private void Log(string message, bool isWarning = false)
    {
        if (!m_debugLogs) return;

        if (isWarning)
            Debug.LogWarning($"[SplineFollower] {message}", this);
        else
            Debug.Log($"[SplineFollower] {message}", this);
    }

    private void OnDrawGizmosSelected()
    {
        if (m_splineContainer == null) return;

        if (m_drawDetectionGizmo)
        {
            Gizmos.color = m_isBlocked ? Color.red : Color.green;
            Gizmos.DrawWireSphere(transform.position + transform.forward * m_detectionDistance, m_detectionRadius);
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * m_detectionDistance);
        }

        int splineCount = m_splineContainer.Splines.Count;

        foreach (var connection in m_connections)
        {
            if (connection.fromSplineIndex >= 0 && connection.fromSplineIndex < splineCount)
            {
                m_splineContainer.Evaluate(connection.fromSplineIndex, connection.fromT, out float3 posFrom, out _, out _);
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(posFrom, 0.25f);
            }

            if (connection.toSplineIndex >= 0 && connection.toSplineIndex < splineCount)
            {
                m_splineContainer.Evaluate(connection.toSplineIndex, connection.toT, out float3 posTo, out _, out _);
                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(posTo, 0.15f);
            }
        }
    }
}