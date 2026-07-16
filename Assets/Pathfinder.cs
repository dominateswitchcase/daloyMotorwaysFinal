using System.Collections.Generic;
using UnityEngine;

public static class Pathfinder
{
    public static List<RoadSegment> FindPath(RoadNode start, RoadNode goal)
    {
        Queue<RoadNode> frontier = new();
        Dictionary<RoadNode, RoadSegment> cameFromRoad = new();
        Dictionary<RoadNode, RoadNode> cameFromNode = new();

        frontier.Enqueue(start);

        cameFromNode[start] = null;

        while (frontier.Count > 0)
        {
            RoadNode current = frontier.Dequeue();

            if (current == goal)
                break;

            foreach (RoadSegment road in current.connectedRoads)
            {
                RoadNode next = road.GetOtherNode(current);

                if (next == null)
                    continue;

                if (cameFromNode.ContainsKey(next))
                    continue;

                frontier.Enqueue(next);

                cameFromNode[next] = current;
                cameFromRoad[next] = road;
            }
        }

        List<RoadSegment> path = new();

        if (!cameFromNode.ContainsKey(goal))
            return path;

        RoadNode node = goal;

        while (node != start)
        {
            path.Insert(0, cameFromRoad[node]);
            node = cameFromNode[node];
        }

        return path;
    }
}