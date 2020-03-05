using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Spellplague.Utility
{
    public static class SPUtility
    {
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

        public static bool CheckRotation(Quaternion currentRotation, Quaternion desiredRotation)
        {
            return (!Mathf.Approximately(currentRotation.eulerAngles.x, desiredRotation.eulerAngles.x))
                || (!Mathf.Approximately(currentRotation.eulerAngles.y, desiredRotation.eulerAngles.y))
                || (!Mathf.Approximately(currentRotation.eulerAngles.z, desiredRotation.eulerAngles.z));
        }

        public static Vector3 SmoothStep(Vector3 start, Vector3 end, float smoothAmount)
        {
            float x = Mathf.SmoothStep(start.x, end.x, smoothAmount);
            float y = Mathf.SmoothStep(start.y, end.y, smoothAmount);
            float z = Mathf.SmoothStep(start.z, end.z, smoothAmount);
            return new Vector3(x, y, z);
        }
    }
}