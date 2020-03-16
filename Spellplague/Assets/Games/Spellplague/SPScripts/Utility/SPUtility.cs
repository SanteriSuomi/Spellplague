using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Spellplague.Utility
{
    public static class SPUtility
    {
        /// <summary>
        /// Multiplier that is commonly used throughout the project.
        /// </summary>
        public const float CommonUpdateMultiplier = 1000;

        private const float DefaultTaskDelay = 8.33f;

        public static Task TaskDelay() 
            => Task.Delay(TimeSpan.FromMilliseconds(DefaultTaskDelay));

        public static Task TaskDelay(float delay) 
            => Task.Delay(TimeSpan.FromMilliseconds(delay));

        public static bool CheckPosition(Vector3 currentPosition, Vector3 desiredPosition)
        {
            return (!Mathf.Approximately(currentPosition.x, desiredPosition.x))
                || (!Mathf.Approximately(currentPosition.y, desiredPosition.y))
                || (!Mathf.Approximately(currentPosition.z, desiredPosition.z));
        }

        public static bool CheckPositionAll(Vector3 currentPosition, Vector3 desiredPosition)
        {
            return !(Mathf.Approximately(currentPosition.x, desiredPosition.x)
                && Mathf.Approximately(currentPosition.y, desiredPosition.y)
                && Mathf.Approximately(currentPosition.z, desiredPosition.z));
        }

        public static bool CheckPosition2D(Vector2 currentPosition, Vector2 desiredPosition)
        {
            return !(Mathf.Approximately(currentPosition.x, desiredPosition.x)
                && Mathf.Approximately(currentPosition.y, desiredPosition.y));
        }

        public static bool CheckRotation(Quaternion currentRotation, Quaternion desiredRotation)
        {
            return (!Mathf.Approximately(currentRotation.eulerAngles.x, desiredRotation.eulerAngles.x))
                || (!Mathf.Approximately(currentRotation.eulerAngles.y, desiredRotation.eulerAngles.y))
                || (!Mathf.Approximately(currentRotation.eulerAngles.z, desiredRotation.eulerAngles.z));
        }

        public static Vector3 SmoothStep(Vector3 startPosition, Vector3 endPosition, float smoothAmount)
        {
            float x = Mathf.SmoothStep(startPosition.x, endPosition.x, smoothAmount);
            float y = Mathf.SmoothStep(startPosition.y, endPosition.y, smoothAmount);
            float z = Mathf.SmoothStep(startPosition.z, endPosition.z, smoothAmount);
            return new Vector3(x, y, z);
        }
    }
}