using System;
using System.Collections.Generic;
using UnityEngine;

public static class BFSUtils
{
    #region BFS Node
    private class BFSNode
    {
        public Vector2Int Position;
        public Vector2Int Direction; // Hướng đi để đến được Node này
        public int TurnCount;
        public BFSNode Parent; // Tham chiếu đến node cha

        public BFSNode(Vector2Int position, Vector2Int direction, int turnCount, BFSNode parent)
        {
            Position = position;
            Direction = direction;
            TurnCount = turnCount;
            Parent = parent;
        }
    }
    #endregion

    private static readonly Vector2Int[] directions = new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

    /// <summary>
    /// Tìm đường đi từ điểm bắt đầu đến đích bằng thuật toán BFS được tối ưu cho game Pikachu (duyệt theo đường thẳng).
    /// </summary>
    /// <param name="start">Vị trí bắt đầu.</param>
    /// <param name="goal">Vị trí đích cần đến.</param>
    /// <param name="maxTurns">Số lần rẽ tối đa cho phép.</param>
    /// <param name="isWalkable">Hàm kiểm tra một ô có phải là ô trống (có thể đi qua) hay không.</param>
    /// <param name="inBounds">Hàm kiểm tra vị trí có nằm trong giới hạn bản đồ hay không.</param>
    /// <returns>Danh sách các điểm rẽ và điểm cuối. Trả về null nếu không có đường đi.</returns>
    public static List<Vector2Int> FindPathPikachu(Vector2Int start, Vector2Int goal, int maxTurns, Func<Vector2Int, bool> isWalkable, Func<Vector2Int, bool> inBounds)
    {
        var queue = new Queue<BFSNode>();
        var visited = new HashSet<(Vector2Int, Vector2Int)>(); // (Vị trí, hướng đi tới vị trí đó)
        
        // Bắt đầu BFS từ điểm start, không có hướng đi và không có cha
        queue.Enqueue(new BFSNode(start, Vector2Int.zero, -1, null)); // Bắt đầu với -1 lần rẽ
        
        while (queue.Count > 0)
        {
            var node = queue.Dequeue();

            // Nếu số lần rẽ đã vượt quá giới hạn, không xét nhánh này nữa
            if (node.TurnCount > maxTurns)
            {
                continue;
            }

            // Từ node hiện tại, thử đi thẳng theo cả 4 hướng
            foreach (var dir in directions)
            {
                // Không đi ngược lại hướng vừa tới
                if (node.Direction != Vector2Int.zero && dir == -node.Direction)
                {
                    continue;
                }

                Vector2Int nextPos = node.Position;
                // Đi thẳng một mạch theo hướng 'dir'
                while (true)
                {
                    nextPos += dir;

                    // 1. Kiểm tra biên
                    if (!inBounds(nextPos)) break;

                    // 2. Kiểm tra xem có đến đích không
                    if (nextPos == goal)
                    {
                        // Tính số lần rẽ cuối cùng
                        int finalTurns = node.TurnCount + (node.Direction != dir && node.Direction != Vector2Int.zero ? 1 : 0);
                        if (finalTurns <= maxTurns)
                        {
                            // TÌM THẤY ĐƯỜNG ĐI -> Truy vết
                            var path = new List<Vector2Int>();
                            path.Add(goal);
                            path.Add(node.Position); // Thêm điểm rẽ cuối
                            var current = node;
                            while (current.Parent != null)
                            {
                                current = current.Parent;
                                path.Add(current.Position);
                            }
                            path.Reverse();
                            return path;
                        }
                    }

                    // 3. Nếu gặp chướng ngại vật (không phải ô trống), dừng đường thẳng này
                    if (!isWalkable(nextPos)) break;

                    // 4. Nếu là ô trống, đây là một điểm rẽ tiềm năng
                    // Tính số lần rẽ mới
                    int newTurns = node.TurnCount + 1;
                    
                    var state = (nextPos, dir);
                    if (!visited.Contains(state))
                    {
                        visited.Add(state);
                        // Đưa điểm rẽ tiềm năng này vào hàng đợi để xét tiếp
                        queue.Enqueue(new BFSNode(nextPos, dir, newTurns, node));
                    }
                }
            }
        }
        return null;
    }
}