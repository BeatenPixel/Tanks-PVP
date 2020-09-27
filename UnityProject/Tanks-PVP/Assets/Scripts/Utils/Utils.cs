using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Forsis.Utils {

    public static class Utils {

        public static bool IsInMask(BlockCollisionLayer layer, BlockCollisionLayer mask) {
            return mask == (mask | layer);
        }

        public static Vector2 DirToVector2D(this byte b) {
            if(b == 0) {
                return new Vector2(0, 1);
            } else if (b == 1) {
                return new Vector2(1, 0);
            } else if(b == 2) {
                return new Vector2(0, -1);
            } else if(b == 3) {
                return new Vector2(-1, 0);
            } else {
                return new Vector2(float.MaxValue, float.MaxValue);
            }
        }

        public static byte DirToByte(this Vector2 d) {
            Debug.Log(d);
            if(d.y > 0.1f) {
                return 0; 
            } else if(d.x > 0.1f) {
                return 1;
            } else if(d.y < -0.1f) {
                return 2;
            } else if(d.x < -0.1f) {
                return 3;
            } else {
                Debug.LogError("Invalid direction!");
                return 255;
            }
        }

    }

    public class Timer {

        private float period;
        private float lastTriggerTime;

        public Timer(float _period) {
            period = _period;
            lastTriggerTime = Time.time;
        }

        public bool Check() {
            if(Time.time > lastTriggerTime + period) {
                lastTriggerTime = Time.time;
                return true;
            } else {
                return false;
            }
        }
    }

}
