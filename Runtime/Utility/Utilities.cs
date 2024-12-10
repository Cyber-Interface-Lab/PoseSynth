using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{

    /// <summary>
    /// Useful static variables and functions.
    /// </summary>
    public static class Utilities
    {
        public static int FPS = 60;

        #region GUI Window ID
        private static int s_windowId = 0;
        /// <summary>
        /// Get non-duplicate ID of GUI window.
        /// </summary>
        /// <returns></returns>
        public static int GetWindowId() => s_windowId++;
        #endregion

        #region Math
        /// <summary>
        /// Flatten the list of Vector3.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static List<float> Flatten(List<Vector3> v)
        {
            var result = new List<float>();
            for (int i = 0; i < v.Count; i++)
            {
                result.Add(v[i].x);
                result.Add(v[i].y);
                result.Add(v[i].z);
            }
            return result;
        }
        /// <summary>
        /// Create the list of Vector3 based on the list of float.
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static List<Vector3> Vectorize3(List<float> f)
        {
            var result = new List<Vector3>();
            for (int i = 0; i < f.Count; i += 3)
            {
                result.Add(new Vector3(f[i], f[i + 1], f[i + 2]));
            }
            return result;
        }
        /// <summary>
        /// Get a modified angle in the range (-180, 180].
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static float GetCyclicAngle(float angle)
        {
            var cyclicAngle = Mathf.Repeat(angle, 360f);
            return 180f < cyclicAngle ? cyclicAngle - 360f : cyclicAngle;
        }
        /// <summary>
        /// Get a modified angle in the range (-180, 180].
        /// </summary>
        /// <param name="angles"></param>
        /// <returns></returns>
        public static Vector3 GetCyclicAngle(Vector3 angles)
        {
            return new Vector3(
                x: GetCyclicAngle(angles.x),
                y: GetCyclicAngle(angles.y),
                z: GetCyclicAngle(angles.z)
                );
        }
        /// <summary>
        /// Get product of two Quaternion.
        /// P' = Q P Q^-1
        /// </summary>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public static Quaternion Multiply(Quaternion p, Quaternion q)
        {
            return Quaternion.Inverse(q) * p * q;
        }
        #endregion

        /// <summary>
        /// 引数の序数を求める
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string GetNumberWord(int num)
        {
            // 負の数はth
            if (num <= 0)
            {
                return $"{num}th";
            }

            // 1,2,3で終わるもの以外はth
            else if (3 < num % 10)
            {
                return $"{num}th";
            }

            // 1,2,3で終わるもののうち，十の位が1ならばth
            else if (num % 10 <= 3 && (num % 100) / 10 == 1)
            {
                return $"{num}th";
            }

            // それ以外の1,2,3はそれぞれst，nd，rd
            else
            {
                return num % 10 == 1 ? "1st" : num % 10 == 2 ? "2nd" : num % 10 == 3 ? "3rd" : "ERROR";
            }
        }
    }
}