using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CircleSync : MonoBehaviour
{
    public static int PosID = Shader.PropertyToID("_Player_Position");
    public static int SizeID = Shader.PropertyToID("_Size");

    public List<Material> Materials;

    public Camera Camera;
    public LayerMask Mask;
    public float raycastDistance = 5;
    float alpha;
    Ray ray;


    [Tooltip("The speed until the effect appears or disappears")]
    public float speedAlpha;


    private void Update()
    {
        var dir = Camera.transform.position - transform.position;
        ray = new Ray(transform.position, dir.normalized);
        //ray = new Ray(transform.position, Quaternion.AngleAxis(-45, Vector3.up) * -Vector3.forward);

        if (Physics.Raycast(ray,out var hit, raycastDistance, Mask ))
        {
            foreach (Material material in Materials)
            {
                
                alpha += Time.deltaTime * speedAlpha;
                alpha = Mathf.Clamp(alpha, 0, 0.9f);
                material.SetFloat(SizeID, alpha);
            }
        }


        else
        {
            
            alpha -= Time.deltaTime * speedAlpha;
            alpha = Mathf.Clamp(alpha, 0, 0.7f);
            foreach (Material material in Materials)
            {

                material.SetFloat(SizeID, alpha);
            }
        }

        var view = Camera.WorldToViewportPoint(transform.position);
        foreach (Material material in Materials)
        {
            material.SetVector(PosID, view);
        }



    }
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
          Gizmos.color = Color.yellow;
            Gizmos.DrawRay(ray);  
        }
    }
}
