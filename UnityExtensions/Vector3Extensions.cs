using System;
using UnityEngine;

namespace GuestUnion {

    public static class Vector3Extensions {

        public static float Dot(this in Vector3 lhs, in Vector3 rhs) => (lhs.x * rhs.x) + (lhs.y * rhs.y) + (lhs.z * rhs.z);

        public static Vector3 Cross(this in Vector3 lhs, in Vector3 rhs) => new((lhs.y * rhs.z) - (lhs.z * rhs.y), (lhs.z * rhs.x) - (lhs.x * rhs.z), (lhs.x * rhs.y) - (lhs.y * rhs.x));

        public static Vector3 LerpTo(this in Vector3 from, in Vector3 to, float t) {
            t = Mathf.Clamp01(t);
            return new Vector3(from.x + ((to.x - from.x) * t), from.y + ((to.y - from.y) * t), from.z + ((to.z - from.z) * t));
        }

        public static Vector3 MoveTowords(this in Vector3 current, in Vector3 target, float maxDistanceDelta) {
            var dx = target.x - current.x;
            var dy = target.y - current.y;
            var dz = target.z - current.z;
            var sqrDistance = (dx * dx) + (dy * dy) + (dz * dz);
            if (sqrDistance < Mathf.Epsilon || (maxDistanceDelta > Mathf.Epsilon && sqrDistance <= maxDistanceDelta * maxDistanceDelta)) {
                return target;
            }
            var distance = (float)Math.Sqrt(sqrDistance);
            return new Vector3(current.x + (dx / distance * maxDistanceDelta), current.y + (dy / distance * maxDistanceDelta), current.z + (dz / distance * maxDistanceDelta));
        }

        public static Vector3 Project(this in Vector3 vector, in Vector3 onNormal) {
            var sqrMagnitude = onNormal.sqrMagnitude;
            if (sqrMagnitude < Mathf.Epsilon) {
                return Vector3.zero;
            }
            var dotProduct = vector.Dot(onNormal);
            return new Vector3(onNormal.x * dotProduct / sqrMagnitude, onNormal.y * dotProduct / sqrMagnitude, onNormal.z * dotProduct / sqrMagnitude);
        }

        public static Vector3 ProjectOnPlane(this in Vector3 vector, in Vector3 planeNormal) {
            var sqrMagnitude = planeNormal.sqrMagnitude;
            if (sqrMagnitude < Mathf.Epsilon) {
                return vector;
            }
            var dotProduct = vector.Dot(planeNormal);
            return new Vector3(vector.x - (planeNormal.x * dotProduct / sqrMagnitude), vector.y - (planeNormal.y * dotProduct / sqrMagnitude), vector.z - (planeNormal.z * dotProduct / sqrMagnitude));
        }

        /// <summary>x+y+z</summary>
        /// <returns>x+y+z</returns>
        public static float Sum(this in Vector3 vector3) => vector3.x + vector3.y + vector3.z;

        public static Vector3 ApplySpread(this in Vector3 direction, in Vector3 upwards, float horizontal, float vertical) {
            if (horizontal > Mathf.Epsilon) {
                var result = Quaternion.AngleAxis(horizontal, upwards) * direction;
                if (vertical > Mathf.Epsilon) {
                    result = Quaternion.AngleAxis(vertical, direction.Cross(Vector3.up)) * result;
                }
                return result;
            }
            if (vertical > Mathf.Epsilon) {
                return Quaternion.AngleAxis(vertical, direction.Cross(upwards)) * direction;
            }
            return direction;
        }
    }
}