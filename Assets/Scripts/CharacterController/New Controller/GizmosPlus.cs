using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
/**
 * Code added by Marco Zepeda from his personal code library to have additional gizmos drawing tool
 */
namespace More2DGizmos
{
    public class GizmosPlus
    {
        #region Capsules
        public static void DrawWireCapsule(Vector2 _pos, float rotation, Vector2 size, CapsuleDirection2D capsuleDirection, Color _color = default)
        {
            DrawCapsuleData(size, capsuleDirection, out float radius, out _, out float adjustAngle, out float pointOffset);

            if (_color != default(Color))
            {
                Handles.color = _color;
                Gizmos.color = _color;
            }

            if (pointOffset == 0)
            {
                Handles.DrawWireArc(_pos, Vector3.forward, Vector3.right, 360, radius);
            }
            else
            {
                Vector2 focusNeg = _pos + Kinematics.GetVector(-pointOffset, rotation + adjustAngle);
                Vector2 focusPos = _pos + Kinematics.GetVector(pointOffset, rotation + adjustAngle);

                Handles.DrawWireArc(focusPos, Vector3.forward, Kinematics.GetVector(1, rotation + adjustAngle + 270), 180, radius);
                Handles.DrawWireArc(focusNeg, Vector3.forward, Kinematics.GetVector(1, rotation + adjustAngle + 90), 180, radius);
                Gizmos.DrawLine(focusPos + Kinematics.GetVector(radius, rotation - adjustAngle + 90), focusNeg + Kinematics.GetVector(radius, rotation - adjustAngle + 90));
                Gizmos.DrawLine(focusPos + Kinematics.GetVector(-radius, rotation - adjustAngle + 90), focusNeg + Kinematics.GetVector(-radius, rotation - adjustAngle + 90));
            }
        }
        public static void DrawConnectedCapsules(Vector2 origin1, Vector2 origin2, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Color cap1 = default(Color), Color cap2 = default(Color), Color ConnectionLines = default(Color))
        {
            DrawCapsuleData(size, capsuleDirection, out float radius, out float length, out float adjustAngle, out float pointOffset);
            Vector2 leftOrigin, rightOrigin;
            if (origin1.x > origin2.x)
            {
                rightOrigin = origin2;
                leftOrigin = origin1;
            }
            else
            {
                rightOrigin = origin1;
                leftOrigin = origin2;
            }
            Vector2 PrimaryLeftFocus, PrimaryRightFocus, SecondaryLeftFocus, SecondaryRightFocus;
            Vector2 focusLNeg = FocusPoint(leftOrigin, -pointOffset, angle + adjustAngle);
            Vector2 focusLPos = FocusPoint(leftOrigin, pointOffset, angle + adjustAngle);
            Vector2 focusRNeg = FocusPoint(rightOrigin, -pointOffset, angle + adjustAngle);
            Vector2 focusRPos = FocusPoint(rightOrigin, pointOffset, angle + adjustAngle);
            System.Func<float, float> middleLine = getLinearEquation(origin1, origin2);
            if (IsPointAboveLine(focusLNeg, middleLine))
            {
                PrimaryLeftFocus = focusLNeg;
                SecondaryLeftFocus = focusLPos;
            }
            else
            {
                PrimaryLeftFocus = focusLPos;
                SecondaryLeftFocus = focusLNeg;
            }

            if (IsPointAboveLine(focusRNeg, middleLine))
            {
                PrimaryRightFocus = focusRNeg;
                SecondaryRightFocus = focusRPos;
            }
            else
            {
                PrimaryRightFocus = focusRPos;
                SecondaryRightFocus = focusRNeg;
            }

            DrawWireCapsule(origin1, angle, size, capsuleDirection, cap1);
            DrawWireCapsule(origin2, angle, size, capsuleDirection, cap2);

            float slope = Mathf.Tan(Mathf.Deg2Rad * Kinematics.SignedEulerAngle(rightOrigin - leftOrigin));
            //Debug.Log("slope: " + slope);
            float x = (slope * radius) / Mathf.Sqrt(Mathf.Pow(slope, 2) + 1);
            float y = Mathf.Sqrt(Mathf.Pow(radius, 2) - Mathf.Pow(x, 2));
            Gizmos.color = ConnectionLines;
            Gizmos.DrawLine(PrimaryLeftFocus + new Vector2(-x, y), PrimaryRightFocus + new Vector2(-x, y));
            Gizmos.DrawLine(SecondaryLeftFocus + new Vector2(x, -y), SecondaryRightFocus + new Vector2(x, -y));

        }

        private static void DrawCapsuleData(Vector2 size, CapsuleDirection2D capsuleDirection, out float radius, out float length, out float adjustAngle, out float pointOffset)
        {
            if (capsuleDirection == CapsuleDirection2D.Vertical)
            {
                radius = size.x / 2;
                length = size.y;
                adjustAngle = 90;
            }
            else
            {
                radius = size.y / 2;
                length = size.x;
                adjustAngle = 0;
            }
            pointOffset = 0;
            if (radius * 2 < length)
            {
                pointOffset = (length / 2) - radius;
            }
        }
        private static Vector2 FocusPoint(Vector2 origin, float pointOffset, float angle)
        {
            return origin + Kinematics.GetVector(pointOffset, angle);
        }

        public static RaycastHit2D DrawCapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, int layerMask, Color cap1 = default, Color cap2 = default, Color ConnectionLines = default)
        {
            RaycastHit2D hit = Physics2D.CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, layerMask);
            Vector2 pos2Origin = origin + Kinematics.GetVector(distance, Kinematics.SignedEulerAngle(direction));
            DrawConnectedCapsules(origin, pos2Origin, size, capsuleDirection, angle, cap1, cap2, ConnectionLines);
            if (hit)
            {
                Debug.DrawLine(hit.point, hit.point + hit.normal.normalized * 0.2f, Color.yellow);
                Debug.DrawLine(hit.point, hit.point + Kinematics.GetVector(hit.distance, Kinematics.SignedEulerAngle(-direction)), Color.magenta);
            }
            return hit;
        }
        public static RaycastHit2D DrawCapsuleCast(CapsuleSim capsule, Vector2 displacement, int layerMask, Color cap1 = default, Color cap2 = default, Color ConnectionLines = default)
        {
            return DrawCapsuleCast(capsule.origin, capsule.size, capsule.direction, capsule.rotation, displacement, displacement.magnitude, layerMask, cap1, cap2, ConnectionLines);
        }
        public static RaycastHit2D DrawCapsuleColliderCast(CapsuleCollider2D col, Vector2 displacement, int layerMask, Color cap1 = default, Color cap2 = default, Color ConnectionLines = default)
        {
            return DrawCapsuleCast(CapsuleSim.CapsuleColliderCenter(col), col.size, col.direction, col.transform.eulerAngles.z, displacement, displacement.magnitude, layerMask, cap1, cap2, ConnectionLines);
        }
        public static int DrawOverlapCapsuleCol(CapsuleCollider2D col, Color hit = default, Color miss = default)
        {
            Collider2D[] results = new Collider2D[5];
            var hitCount = col.OverlapCollider(new ContactFilter2D().NoFilter(), results);
            if (hitCount <= 0)
            {
                DrawWireCapsule(CapsuleSim.CapsuleColliderCenter(col), col.transform.eulerAngles.z, col.size, col.direction, miss);
                return hitCount;
            }
            DrawCapsuleCollider(col, hit);
            ColliderDistance2D colliderDistance = col.Distance(results[0]);
            Gizmos.color = new Color(255, 0, 255);

            Gizmos.DrawWireSphere(colliderDistance.pointA, .025f);
            Gizmos.color = new Color(0, 128, 255);
            Gizmos.DrawLine(colliderDistance.pointA, colliderDistance.pointB);
            return hitCount;
        }

        public static void DrawCapsuleCollider(CapsuleCollider2D col, Color color = default)
        {
            DrawWireCapsule(CapsuleSim.CapsuleColliderCenter(col), col.transform.eulerAngles.z, col.size, col.direction, color);
        }
        public static void DrawCapsule(CapsuleSim capsuleSim, Color color = default)
        {
            DrawWireCapsule(capsuleSim.origin, capsuleSim.rotation, capsuleSim.size, capsuleSim.direction, color);
        }
        //Increases/Decreases size of drawn capsule collider
        public static void DrawScaledCapsuleCollider(CapsuleCollider2D col, Vector2 sizeDifference, Color color = default)
        {
            sizeDifference = new Vector2(Mathf.Max(-col.size.x, sizeDifference.x), Mathf.Max(-col.size.y, sizeDifference.y));
            DrawWireCapsule(CapsuleSim.CapsuleColliderCenter(col), col.transform.eulerAngles.z, col.size - sizeDifference, col.direction, color);
        }
        #endregion



        public static RaycastHit2D DrawCapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, int layerMask)
        {
            Color color = Physics2D.CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, layerMask) ? Color.red : Color.green;
            return DrawCapsuleCast(origin, size, capsuleDirection, angle, direction, distance, layerMask, color, color, color);
        }
        static public RaycastHit2D BoxCast(Vector2 origen, Vector2 size, float angle, Vector2 direction, float distance, int mask)
        {
            RaycastHit2D hit = Physics2D.BoxCast(origen, size, angle, direction, distance, mask);

            //Setting up the points to draw the cast
            Vector2 p1, p2, p3, p4, p5, p6, p7, p8;
            float w = size.x * 0.5f;
            float h = size.y * 0.5f;
            p1 = new Vector2(-w, h);
            p2 = new Vector2(w, h);
            p3 = new Vector2(w, -h);
            p4 = new Vector2(-w, -h);

            Quaternion q = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
            p1 = q * p1;
            p2 = q * p2;
            p3 = q * p3;
            p4 = q * p4;

            p1 += origen;
            p2 += origen;
            p3 += origen;
            p4 += origen;

            Vector2 realDistance = direction.normalized * distance;
            p5 = p1 + realDistance;
            p6 = p2 + realDistance;
            p7 = p3 + realDistance;
            p8 = p4 + realDistance;


            //Drawing the cast
            Color castColor = hit ? Color.red : Color.green;
            Debug.DrawLine(p1, p2, castColor);
            Debug.DrawLine(p2, p3, castColor);
            Debug.DrawLine(p3, p4, castColor);
            Debug.DrawLine(p4, p1, castColor);

            Debug.DrawLine(p5, p6, castColor);
            Debug.DrawLine(p6, p7, castColor);
            Debug.DrawLine(p7, p8, castColor);
            Debug.DrawLine(p8, p5, castColor);

            Debug.DrawLine(p1, p5, Color.grey);
            Debug.DrawLine(p2, p6, Color.grey);
            Debug.DrawLine(p3, p7, Color.grey);
            Debug.DrawLine(p4, p8, Color.grey);
            if (hit)
            {
                Debug.DrawLine(hit.point, hit.point + hit.normal.normalized * 0.2f, Color.yellow);
            }
            return hit;
        }
        static public RaycastHit2D BoxCast(Vector2 origen, Vector2 size, float angle, Vector2 direction, ContactFilter2D contact, List<RaycastHit2D> results, float distance)
        {
            RaycastHit2D hit = new RaycastHit2D() ;
            if(Physics2D.BoxCast(origen, size, angle, direction, contact, results, distance) > 0)
            {
                foreach(RaycastHit2D hite in results)
                {
                    if (!hite.collider.isTrigger)
                    {
                        hit = hite;
                        break;
                    }
                }
            }
            //Setting up the points to draw the cast
            Vector2 p1, p2, p3, p4, p5, p6, p7, p8;
            float w = size.x * 0.5f;
            float h = size.y * 0.5f;
            p1 = new Vector2(-w, h);
            p2 = new Vector2(w, h);
            p3 = new Vector2(w, -h);
            p4 = new Vector2(-w, -h);

            Quaternion q = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
            p1 = q * p1;
            p2 = q * p2;
            p3 = q * p3;
            p4 = q * p4;

            p1 += origen;
            p2 += origen;
            p3 += origen;
            p4 += origen;

            Vector2 realDistance = direction.normalized * distance;
            p5 = p1 + realDistance;
            p6 = p2 + realDistance;
            p7 = p3 + realDistance;
            p8 = p4 + realDistance;


            //Drawing the cast
            Color castColor = hit ? Color.red : Color.green;
            Debug.DrawLine(p1, p2, castColor);
            Debug.DrawLine(p2, p3, castColor);
            Debug.DrawLine(p3, p4, castColor);
            Debug.DrawLine(p4, p1, castColor);

            Debug.DrawLine(p5, p6, castColor);
            Debug.DrawLine(p6, p7, castColor);
            Debug.DrawLine(p7, p8, castColor);
            Debug.DrawLine(p8, p5, castColor);

            Debug.DrawLine(p1, p5, Color.grey);
            Debug.DrawLine(p2, p6, Color.grey);
            Debug.DrawLine(p3, p7, Color.grey);
            Debug.DrawLine(p4, p8, Color.grey);
            if (hit)
            {
                Debug.DrawLine(hit.point, hit.point + hit.normal.normalized * 0.2f, Color.yellow);
            }
            return hit;
        }
        private static System.Func<float, float> getLinearEquation(Vector2 point1, Vector2 point2)
        {

            float deltaX = point2.x - point1.x;
            float deltaY = point2.y - point1.y;
            return (x) => (deltaY / deltaX) * (x - point1.x) + point1.y;
        }

        private static bool IsPointAboveLine(Vector2 point, System.Func<float, float> linearEquation)
        {
            return linearEquation(point.x) < point.y;
        }
    }
}

public class CapsuleSim
{
    public Vector2 origin;
    public Vector2 size;
    public CapsuleDirection2D direction;
    public float rotation;
    public CapsuleSim(Vector2 origin, Vector2 size, CapsuleDirection2D direction, float rotation)
    {
        this.origin = origin;
        this.size = size;
        this.direction = direction;
        this.rotation = rotation;
    }
    public CapsuleSim(CapsuleCollider2D col)
    {
        origin = CapsuleColliderCenter(col);
        size = col.size;
        direction = col.direction;
        rotation = col.transform.eulerAngles.z;
    }

    public static Vector2 CapsuleColliderCenter(CapsuleCollider2D col)
    {
        GameObject obj = col.gameObject;
        Vector2 pos = obj.transform.position;
        float rotation = obj.transform.eulerAngles.z;
        return pos + Kinematics.GetVector(col.offset.y, rotation + 90) + Kinematics.GetVector(col.offset.x, rotation);
    }
}
