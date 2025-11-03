using System;
using UnityEngine;

namespace OneLine
{
    [Serializable]
    public struct Pixel : IEquatable<Pixel>
    {
        public static readonly Pixel Zero = new Pixel(0, 0);
        
        public Pixel(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        [SerializeField]
        public int x;

        [SerializeField]
        public int y;
        
        public int sqrMagnitude => x * x + y * y;

        public bool Equals(Pixel other)
        {
            return x == other.x && y == other.y;
        }

        public override bool Equals(object obj)
        {
            return obj is Pixel other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (x * 397) ^ y;
            }
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }

        public static Pixel operator +(Pixel a, Pixel b)
        {
            a.x += b.x;
            a.y += b.y;
            return a;
        }

        public static Pixel operator -(Pixel a, Pixel b)
        {
            a.x -= b.x;
            a.y -= b.y;
            return a;
        }

        public static Vector2 operator +(Vector2 a, Pixel b)
        {
            a.x += b.x;
            a.y += b.y;
            return a;
        }

        public static Vector2 operator -(Vector2 a, Pixel b)
        {
            a.x -= b.x;
            a.y -= b.y;
            return a;
        }

        public static Vector2 operator +(Pixel a, Vector2 b)
        {
            b.x += a.x;
            b.y += a.y;
            return b;
        }

        public static Vector2 operator -(Pixel a, Vector2 b)
        {
            b.x -= a.x;
            b.y -= a.y;
            return b;
        }

        public static bool operator ==(Pixel a, Pixel b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(Pixel a, Pixel b)
        {
            return a.x != b.x || a.y != b.y;
        }

        public static bool operator ==(Vector2Int a, Pixel b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(Vector2Int a, Pixel b)
        {
            return a.x != b.x || a.y != b.y;
        }

        public static bool operator ==(Pixel a, Vector2Int b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(Pixel a, Vector2Int b)
        {
            return a.x != b.x || a.y != b.y;
        }

        public static implicit operator Vector2(Pixel p)
        {
            return new Vector2(p.x, p.y);
        }

        public static implicit operator Vector2Int(Pixel p)
        {
            return new Vector2Int(p.x, p.y);
        }

        public static implicit operator Pixel(Vector2Int p)
        {
            return new Pixel(p.x, p.y);
        }
    }
}