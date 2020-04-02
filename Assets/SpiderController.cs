using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderController : MonoBehaviour
{
    public Transform body;
    public float bodyBounceHeight = 0.4f;
    public float animationTime = 0.3f;
    public float raycastDistance = 2f;
    public float stepDistance = 0.8f;
    public float stepHeight = 0.8f;

    public bool debug = false;

    [System.Serializable]
    public class Leg
    {
        public string LegName = "Leg ";
        public Transform raycastTransform;
        public Transform footTransform;
        public Transform poleTransform;
        public Leg oppositeLeg;

        [HideInInspector]
        public Vector3 raycastHitPoint = Vector3.zero;
        [HideInInspector]
        public Vector3 groundedFootPoint = Vector3.zero;
        [HideInInspector]
        public Vector3 lastGroundedFootPoint = Vector3.zero;
        [HideInInspector]
        public Vector3 airborneFootPoint = Vector3.zero;
        public bool grounded = false;
        [HideInInspector]
        public float animationTimer = 0f;
    }

    public List<Leg> legList;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < legList.Count; i++)
        {
            Leg leg = legList[i];

            RaycastHit hit;
            if (Physics.Raycast(leg.raycastTransform.position, Vector3.down, out hit, raycastDistance))
            {
                leg.raycastHitPoint = hit.point;
                leg.groundedFootPoint = hit.point;
                leg.lastGroundedFootPoint = hit.point;
                leg.airborneFootPoint = hit.point;
                leg.grounded = true;
            }

            if (i % 2 == 0)
            {
                leg.oppositeLeg = legList[i + 1];
                leg.groundedFootPoint += transform.forward * 0.2f;
            }
            else
            {
                leg.oppositeLeg = legList[i - 1];
            }

            leg.footTransform.position = leg.groundedFootPoint;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float averageHeight = 0f;

        foreach (Leg leg in legList)
        {
            if (leg.raycastTransform && leg.grounded)
            {
                RaycastHit hit;
                if (Physics.Raycast(leg.raycastTransform.position, -transform.up, out hit, raycastDistance))
                {
                    leg.raycastHitPoint = hit.point;
                }
            }

            if (leg.grounded)
            {
                leg.footTransform.position = leg.groundedFootPoint;

                if ((leg.groundedFootPoint - leg.raycastHitPoint).sqrMagnitude > stepDistance * stepDistance)
                {
                    if (leg.oppositeLeg.grounded == true)
                    {
                        leg.lastGroundedFootPoint = leg.groundedFootPoint;
                        leg.groundedFootPoint = leg.raycastHitPoint;
                        leg.grounded = false;
                    }
                }
            } else
            {
                leg.animationTimer += Time.deltaTime;
                if(leg.animationTimer > animationTime)
                {
                    leg.animationTimer = 0f;
                    leg.grounded = true;
                    leg.airborneFootPoint = leg.groundedFootPoint;
                } else
                {
                    float yPosition = Mathf.Sin((leg.animationTimer / animationTime) * Mathf.PI) * stepHeight;
                    leg.airborneFootPoint = Vector3.Lerp(leg.airborneFootPoint, leg.groundedFootPoint, leg.animationTimer/animationTime) + (Vector3.up * yPosition);
                }

                leg.footTransform.position = leg.airborneFootPoint;
            }

            averageHeight += leg.footTransform.localPosition.y;
        }

        averageHeight /= 6;
        body.transform.position = transform.position + transform.up * (0.4f + averageHeight * bodyBounceHeight);

        RaycastHit bodyHit;
        if(Physics.Raycast(transform.position + Vector3.up, -transform.up, out bodyHit, raycastDistance))
        {

            //body.transform.rotation = Quaternion.LookRotation(bodyHit.normal).eulerAngles.x;
        } else
        {
            //body.transform.rotation = Quaternion.identity;
        }
    }

    private void OnDrawGizmos()
    {
        if (debug && legList != null)
        {
            foreach (Leg leg in legList)
            {
                if (leg.raycastTransform) {

                    if (leg.raycastTransform)
                    {
                        Gizmos.color = new Color(1,1,0,0.3f);
                        Gizmos.DrawSphere(leg.raycastTransform.position, 0.05f);
                    }

                    if (leg.raycastHitPoint != Vector3.zero)
                    {
                        Gizmos.color = new Color(0, 1, 0, 0.3f);
                        Gizmos.DrawSphere(leg.raycastHitPoint, 0.05f);
                    }

                    if(leg.groundedFootPoint != Vector3.zero)
                    {
                        Gizmos.color = new Color(1, 0, 0, 0.3f);
                        Gizmos.DrawSphere(leg.groundedFootPoint, 0.06f);
                    }
                }
            }
        }
    }
}
