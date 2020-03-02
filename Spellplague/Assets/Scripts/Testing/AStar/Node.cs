using System;
using UnityEngine;

namespace Spellplague.AI
{
    public enum NodeState
    {
        None,
        Open,
        Closed
    }

    public class Node : IComparable<Node>
    {
        public Vector3 Position { get; set; }
        public Vector2 ArrayPosition { get; set; }
        public Node ParentNode { get; set; }
        public NodeState NodeState { get; set; }
        public bool IsWalkable { get; set; }

        public float DifficultyMultiplier { get; set; }
        public float G { get; set; }
        public float H { get; set; }
        public float F { get { return G + H; } }

        public Node(Vector3 position, Vector2 arrayPosition, bool isWalkable, 
            float difficultyMultiplier)
        {
            Position = position;
            ArrayPosition = arrayPosition;
            IsWalkable = isWalkable;
            DifficultyMultiplier = difficultyMultiplier;
        }

        public void Reset()
        {
            ParentNode = null;
            NodeState = NodeState.None;
            G = 0;
            H = 0;
        }

        #region Equality Comparisons
        public int CompareTo(Node other)
        {
            if (F < other.F) return -1;
            else if (ApproximatelyEqual(F, other.F)) return 0;
            else return 1;

            bool ApproximatelyEqual(float a, float b)
            {
                // Quickly approximate if two floating points are almost equal
                return a + 0.0000000596F >= b && a - 0.0000000596F <= b;
            }
        }
        
        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }

        public static bool operator ==(Node left, Node right)
        {
            return left.F == right.F;
        }

        public static bool operator !=(Node left, Node right)
        {
            return !(left.F == right.F);
        }

        public static bool operator <(Node left, Node right)
        {
            return left.F < right.F;
        }

        public static bool operator <=(Node left, Node right)
        {
            return left.F <= right.F;
        }

        public static bool operator >(Node left, Node right)
        {
            return left.F > right.F;
        }

        public static bool operator >=(Node left, Node right)
        {
            return left.F >= right.F;
        }
        #endregion
    }
}