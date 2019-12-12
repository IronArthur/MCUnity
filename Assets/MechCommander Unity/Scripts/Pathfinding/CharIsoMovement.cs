using System;
using System.Collections;
using UnityEngine;

class CharIsoMovement : MonoBehaviour
{

    public Transform target;

    public Vector3 IsoTarget;
    public float speed = 20;

    Vector2[] path;
    int targetIndex;

//    private MCGIsoObject isoOBj;
//
//    // Use this for initialization
//    void Start()
//    {
//
//        isoOBj = GetComponent<MCGIsoObject>();
//
//        isoOBj.IsoPosition = new Vector3(4  , 2, 10);
//
//        IsoTarget = new Vector3(87  , 87,10);
//
//        StartCoroutine(RefreshPath());
//
//    }
//
//
//    IEnumerator Move()
//    {
//        for (int i = 0; i < 30; i++)
//        {
//            Debug.Log("Wait " + i);
//            for (int ji = 0; ji < 8; ji++)
//            {
//                isoOBj.IsoPosition += new Vector3(0.5f, 0, 0);
//                yield return new WaitForSeconds(0.1f);
//            }
//
//            for (int ji = 0; ji < 8; ji++)
//            {
//                isoOBj.IsoPosition += new Vector3(0, 0.5f, 0);
//                yield return new WaitForSeconds(0.1f);
//            }
//
//            for (int ji = 0; ji < 8; ji++)
//            {
//                isoOBj.IsoPosition += new Vector3(-0.5f, 0, 0);
//                yield return new WaitForSeconds(0.1f);
//            }
//
//            for (int ji = 0; ji < 8; ji++)
//            {
//                isoOBj.IsoPosition += new Vector3(0, -0.5f, 0);
//                yield return new WaitForSeconds(0.1f);
//            }
//
//
//        }
//
//    }
//
//
//    IEnumerator RefreshPath()
//    {
//        Vector3 targetPositionOld = IsoTarget + Vector3.up; // ensure != to target.position initially
//
//        while (true)
//        {
//            if (targetPositionOld != IsoTarget)
//            {
//                targetPositionOld = IsoTarget;
//                Debug.Log(" walking ISO coords from  :" + isoOBj.ScreenToIsoCoords(transform.position) + " to : " +(IsoTarget));
//                Debug.Log(" walking from :" + transform.position + " to : " + isoOBj.IsoToScreenCoords(IsoTarget));
//                path = Pathfinding.RequestPath(transform.position, isoOBj.IsoToScreenCoords( IsoTarget));
//                StopCoroutine("FollowPath");
//                StartCoroutine("FollowPath");
//            }
//
//            yield return new WaitForSeconds(.25f);
//        }
//    }
//
//    IEnumerator FollowPath()
//    {
//        if (path.Length > 0)
//        {
//            targetIndex = 0;
//            Vector2 currentWaypoint = path[0];
//
//            while (true)
//            {
//                if ((Vector2)transform.position == currentWaypoint)
//                {
//                    targetIndex++;
//                    if (targetIndex >= path.Length)
//                    {
//                        yield break;
//                    }
//                    currentWaypoint = path[targetIndex];
//                }
//
//                transform.position = Vector2.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
//                yield return null;
//
//            }
//        }
//    }
//
//    public void OnDrawGizmos()
//    {
//        if (path != null)
//        {
//            for (int i = targetIndex; i < path.Length; i++)
//            {
//                Gizmos.color = Color.black;
//                //Gizmos.DrawCube((Vector3)path[i], Vector3.one *.5f);
//
//                if (i == targetIndex)
//                {
//                    Gizmos.DrawLine(transform.position, path[i]);
//                } else
//                {
//                    Gizmos.DrawLine(path[i - 1], path[i]);
//                }
//            }
//        }
//    }

}

