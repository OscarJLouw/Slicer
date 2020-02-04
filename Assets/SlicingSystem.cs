using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlicingSystem : MonoBehaviour
{
    public List<GameObject> objectsToCut;

    private bool cutting = false;
    private bool foundCut = false;
    private Vector3 startPoint;
    private Vector3 endPoint;

    private Vector3 cameraStartPoint;
    private Vector3 cameraEndPoint;

    private Vector3[] positions;

    private Camera mainCamera;

    public LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        positions = new Vector3[2];

        lineRenderer.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray cuttingRay = mainCamera.ScreenPointToRay(Input.mousePosition);

            Vector3 point = cuttingRay.GetPoint(3f);

            if (!cutting)
            {
                cutting = true;

                startPoint = point;
                endPoint = point;

                cameraStartPoint = Input.mousePosition;
                cameraEndPoint = Input.mousePosition;
            }
            else
            {
                if (point != startPoint)
                {
                    foundCut = true;

                    endPoint = point;
                    cameraEndPoint = Input.mousePosition;

                    positions[0] = startPoint;
                    positions[1] = point;

                    lineRenderer.gameObject.SetActive(true);
                    lineRenderer.SetPositions(positions);
                }
                else
                {
                    lineRenderer.gameObject.SetActive(false);
                    foundCut = false;
                }
            }
        }
        else
        {
            if (cutting)
            {
                lineRenderer.gameObject.SetActive(false);
                cutting = false;

                if (foundCut)
                {
                    //Vector3 worldNormal = Vector3.Cross(endPoint - startPoint, startPoint - mainCamera.transform.position);
                    foreach (GameObject cutObject in objectsToCut)
                    {
                        // cut the mesh
                        Sliceable sliceable = cutObject.GetComponent<Sliceable>();

                        if (sliceable)
                        {
                            //sliceable.Slice(startPoint, worldNormal);
                            sliceable.SliceByLine(mainCamera, cameraStartPoint, cameraEndPoint);

                        }
                    }
                }

                foundCut = false;
            }
        } 

        if (Input.GetMouseButton(1) && !cutting)
        {
            Vector3 targetPos = mainCamera.ScreenPointToRay(Input.mousePosition).GetPoint(5f);

            foreach (GameObject cutObject in objectsToCut)
            {
                Rigidbody rb = cutObject.GetComponent<Rigidbody>();

                rb.AddForce((targetPos - cutObject.transform.position) * 3f);
            }
        }
    }
}
