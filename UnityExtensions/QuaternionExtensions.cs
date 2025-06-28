using System;
using UnityEngine;

namespace GuestUnion {

    public static class QuaternionExtensions {

        /// <summary>对当前旋转进行扩散操作。/Apply spread to a quaternion rotation.</summary>
        /// <param name="rotation">输入旋转</param>
        /// <param name="pitchAngle">俯仰角上的角度</param>
        /// <param name="yawAngle">偏航角上的角度</param>
        /// <returns>扩散后的旋转</returns>
        public static Quaternion ApplySpread(this in Quaternion rotation, float pitchAngle, float yawAngle) {
            var forward = rotation * Vector3.forward;
            var up = rotation * Vector3.up;
            var right = forward.Cross(up);
            var planeNormal1 = yawAngle.Abs() > Mathf.Epsilon ? Quaternion.AngleAxis(yawAngle, up) * right : right;
            var planeNormal2 = pitchAngle.Abs() > Mathf.Epsilon ? Quaternion.AngleAxis(pitchAngle, right) * up : up;
            return Quaternion.LookRotation(planeNormal2.Cross(planeNormal1), up);
        }

        /// <summary>对当前旋转进行扩散操作。/Apply spread to a quaternion rotation.</summary>
        /// <param name="rotation">输入旋转</param>
        /// <param name="pitchAngle">俯仰角上的角度</param>
        /// <param name="rollAngle">翻滚角上的角度</param>
        /// <returns>扩散后的旋转</returns>
        public static Quaternion ApplySpread2(this Quaternion rotation, float pitchAngle, float rollAngle) {
            var forward = rotation * Vector3.forward;
            if (rollAngle.Abs() > Mathf.Epsilon) {
                rotation *= Quaternion.AngleAxis(rollAngle, forward);
            }
            if (pitchAngle.Abs() > Mathf.Epsilon) {
                rotation *= Quaternion.AngleAxis(pitchAngle, forward.Cross(rotation * Vector3.up));
            }
            return rotation;
        }
    }
}