using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootIK : MonoBehaviour
{
    Animator playerAnimator;
    
    public LayerMask layerMask;
    Vector3 rightFootPosition, leftFootPosition, leftFootIKPosition, rightFootIKPosition;
    Quaternion leftFootIKRotation, rightFootIKRotation;
    float lastPelvisPosY, lastRightFootPosition, lastLeftFootPosition;

    public bool enableIK = true;
    [Range(0f, 2f)] public float heightFromGroundRaycast = 1.14f;
    [Range(0f, 2f)] public float raycastDownDistance = 1.5f;
    public float pelvisOffset = 0f;
    public float pelvisSpeed = 0.28f;
    public float feetToIKPositionSpeed = 0.5f;


    private void Start()
    {
        playerAnimator = GetComponent<Animator>();
    }
    private void FixedUpdate()
    {
        if (enableIK == false)
        {
            return;
        }

        AdjustFeetTarget(ref rightFootPosition, HumanBodyBones.RightFoot);
        AdjustFeetTarget(ref leftFootPosition, HumanBodyBones.LeftFoot);

        FeetPositionSolver(rightFootPosition, ref rightFootIKPosition, ref rightFootIKRotation);
        FeetPositionSolver(leftFootPosition, ref leftFootIKPosition, ref leftFootIKRotation);
    }
    private void OnAnimatorIK(int layerIndex)
    {
        MovePelvisHeight();
        HandleFoot(AvatarIKGoal.RightFoot, "RightFootIK", rightFootIKPosition, rightFootIKRotation, ref lastRightFootPosition);
        HandleFoot(AvatarIKGoal.LeftFoot, "LeftFootIK", leftFootIKPosition, leftFootIKRotation, ref lastLeftFootPosition);
    }

    private void HandleFoot(AvatarIKGoal foot, string weightString, Vector3 footIKPos, Quaternion footIKRot, ref float lastFootPos)
    {
        playerAnimator.SetIKPositionWeight(foot, playerAnimator.GetFloat(weightString));
        playerAnimator.SetIKRotationWeight(foot, playerAnimator.GetFloat(weightString));
        MoveFeetToIKPoint(foot, footIKPos, footIKRot, ref lastFootPos);
    }

    void MoveFeetToIKPoint(AvatarIKGoal foot, Vector3 inPosIK, Quaternion inRotIK, ref float lastFootPosY)
    {
        Vector3 targetIKPos = playerAnimator.GetIKPosition(foot);
        if (inPosIK != Vector3.zero)
        {
            targetIKPos = transform.InverseTransformPoint(targetIKPos);
            inPosIK = transform.InverseTransformPoint(inPosIK);
            float yVar = Mathf.Lerp(lastFootPosY, inPosIK.y, feetToIKPositionSpeed);
            targetIKPos.y += yVar;
            lastFootPosY = yVar;
            targetIKPos = transform.TransformPoint(targetIKPos);
            playerAnimator.SetIKRotation(foot, inRotIK);
        }
        playerAnimator.SetIKPosition(foot, targetIKPos);
    }
    void MovePelvisHeight()
    {
        if (rightFootIKPosition == Vector3.zero || leftFootIKPosition == Vector3.zero || lastPelvisPosY == 0)
        {
            lastPelvisPosY = playerAnimator.bodyPosition.y;
            return;
        }

        float leftOffetPos = leftFootIKPosition.y - transform.position.y;
        float rightOffsetPos = rightFootIKPosition.y - transform.position.y;
        float totalOffset = (leftOffetPos < rightOffsetPos) ? leftOffetPos : rightOffsetPos;

        Vector3 newPelvisPos = playerAnimator.bodyPosition + Vector3.up * totalOffset;
        newPelvisPos.y = Mathf.Lerp(lastPelvisPosY, newPelvisPos.y, pelvisSpeed);
        playerAnimator.bodyPosition = newPelvisPos;
        lastPelvisPosY = playerAnimator.bodyPosition.y;
    }
    void FeetPositionSolver(Vector3 fromSkyPosition, ref Vector3 feetIKPosition, ref Quaternion feetIKRotation)
    {
        RaycastHit hit;
        if (Physics.Raycast(fromSkyPosition, Vector3.down, out hit, raycastDownDistance + heightFromGroundRaycast, layerMask))
        {
            feetIKPosition = fromSkyPosition;
            feetIKPosition.y = hit.point.y + pelvisOffset;
            feetIKRotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * transform.rotation;
            return;
        }
        feetIKPosition = Vector3.zero; //Fails
    }
    void AdjustFeetTarget(ref Vector3 feetPosition, HumanBodyBones foot)
    {
        feetPosition = playerAnimator.GetBoneTransform(foot).position;
        feetPosition.y = transform.position.y + heightFromGroundRaycast;
    }
}
