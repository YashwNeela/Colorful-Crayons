using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Reflection
{
    public class LaserBeam
    {
        Vector2 pos, dir;

        GameObject laserObj;
        LineRenderer laser;
        List<Vector2> laserIndices = new List<Vector2>();

        public LaserBeam(Vector2 pos, Vector2 dir, Material material)
        {
            this.laser = new LineRenderer();
            this.laserObj = new GameObject();
            this.laserObj.name = "Laser Beam 2D";
            this.pos = pos;
            this.dir = dir;

            this.laser = this.laserObj.AddComponent(typeof(LineRenderer)) as LineRenderer;
            this.laser.startWidth = 0.1f;
            this.laser.endWidth = 0.1f;
            this.laser.material = material;
            this.laser.startColor = Color.green;

            CastRay(pos, dir, laser);
        }

        void CastRay(Vector2 pos, Vector2 dir, LineRenderer laser)
        {
            laserIndices.Add(pos);
            RaycastHit2D hit = Physics2D.Raycast(pos, dir, 30);

            if (hit.collider != null)
            {
                CheckHit(hit, dir, laser);
            }
            else
            {
                laserIndices.Add(pos + dir * 30);
                UpdateLaser();
            }
        }

        void CheckHit(RaycastHit2D hitInfo, Vector2 direction, LineRenderer laser)
        {
            if (hitInfo.transform.name == "Mirror")
            {
                
                
                {
                    Vector2 pos = hitInfo.point;
                    Vector2 dir = Vector2.Reflect(direction, hitInfo.normal);

                    CastRay(pos, dir, laser);
                }
            }
            else
            {
                laserIndices.Add(hitInfo.point);
                UpdateLaser();
            }
        }

        void UpdateLaser()
        {
            int count = 0;
            laser.positionCount = laserIndices.Count;

            foreach (Vector2 idx in laserIndices)
            {
                laser.SetPosition(count, idx);
                count++;
            }
        }
    }
}
