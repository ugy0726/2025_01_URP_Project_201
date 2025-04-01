using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{
    public float activeTime = 2.0f;                     //�ܻ� ȿ�� ���� �ð�
    public MovementInput moveScript;                    //ĳ������ �������� ���� �ϴ� ��ũ��Ʈ
    public float speedBoost = 6;                        //���� ȿ�� ���� �ӵ� ������ 
    public Animator animator;                           //ĳ������ �ִϸ��̼��� �����ϴ� ������Ʈ
    public float animSpeedBoost = 1.5f;                 //�ܻ� ȿ�� ��� �� �ִϸ��̼� �ӵ� ������ 

    [Header("Mesh Releted")]
    public float meshRefreshRate = 1.0f;                    //�ܻ��� �����Ǵ� �ð� ����
    public float meshDestoryDelay = 3.0f;                   //������ �ܻ��� ������� �� �ɸ��� �ð�
    public Transform positionToSpawn;                       //�ܻ��� ������ ��ġ 

    [Header("Shader Releted")]
    public Material mat;                                //�ܻ� ����� ����
    public string shaderVerRef;                         //���̴����� ����� ���� �̸� (Alpha)
    public float shaderVarRate = 0.1f;                  //���̴� ȿ���� ��ȭ �ӵ�
    public float shaderVarRefreshRate = 0.05f;          //���̴� ȿ���� ������Ʈ �Ǵ� �ð� ���� 

    private SkinnedMeshRenderer[] skinnedRenderer;          //ĳ������ 3D ���� ������ �ϴ� ������Ʈ��
    private bool isTrailActive;                             //���� �ܻ� ȿ���� Ȱ��ȭ �Ǿ� �ִ��� Ȯ�� �ϴ� ����

    private float normalSpeed;                              //���� �̵� �ӵ��� �����ϴ� ����
    private float normalAnimSpeed;                          //���� �ִϸ��̼� �ӵ��� �����ϴ� ���� 

    //������ �������� ������ �����ϴ� �ڷ�ƾ 
    IEnumerator AnimateMaterialFloat(Material m, float valueGoal, float rate, float refreshRate)
    {
        float valueToAnimate = m.GetFloat(shaderVerRef);    //���� ���� �����´�.

        //��ǥ ���� ���� �� �� ���� �ݺ�
        while (valueToAnimate > valueGoal)
        {
            valueToAnimate -= rate;
            m.SetFloat(shaderVerRef, valueToAnimate);
            yield return new WaitForSeconds(refreshRate);
        }
    }

    IEnumerator ActivateTrail(float timeActivated)      //�ܻ� ȿ�� �ߵ� 
    {
        //���� ���� ������ ����
        normalSpeed = moveScript.movementSpeed;                 //���� �ӵ��� �����ϰ� ������ �ӵ� ����
        moveScript.movementSpeed = speedBoost;

        normalAnimSpeed = animator.GetFloat("animSpeed");           //���� �ִϸ��̼� �ӵ� �����ϰ� ������ �ӵ� ����
        animator.SetFloat("animSpeed", animSpeedBoost);

        while (timeActivated > 0)
        {
            timeActivated -= meshRefreshRate;                       //�ð� ī��带 �Ѵ�. 0������ 

            if (skinnedRenderer == null)
                skinnedRenderer = positionToSpawn.GetComponentsInChildren<SkinnedMeshRenderer>(); //������ ��ġ�� ������ ������Ʈ���� ������

            for (int i = 0; i < skinnedRenderer.Length; i++)         //�� �޽� �������� ���� �ܻ� ����
            {
                GameObject gObj = new GameObject();                 //���ο� ������Ʈ ����
                gObj.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);

                MeshRenderer mr = gObj.AddComponent<MeshRenderer>();
                MeshFilter mf = gObj.AddComponent<MeshFilter>();

                Mesh m = new Mesh();                    //���� ĳ������ ��� �޽÷� ��ȯ
                skinnedRenderer[i].BakeMesh(m);
                mf.mesh = m;
                mr.material = mat;
                //�ܻ��� ���̵� �ƿ� ȿ�� ���� 
                StartCoroutine(AnimateMaterialFloat(mr.material, 0, shaderVarRate, shaderVarRefreshRate));

                Destroy(gObj, meshDestoryDelay);        //���� �ð� �� �ܻ� ����
            }
            //���� �ܻ� �������� ���
            yield return new WaitForSeconds(meshRefreshRate);
        }

        //���� �ӵ��� �ִϸ��̼� �ӵ��� ����
        moveScript.movementSpeed = normalSpeed;
        animator.SetFloat("animSpeed", normalAnimSpeed);
        isTrailActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isTrailActive)  //�����̽��ٸ� ������ ���� �ܻ� ȿ���� ��Ȱ��ȭ�� ��
        {
            isTrailActive = true;
            StartCoroutine(ActivateTrail(activeTime));              //�ܻ� ȿ�� �ڷ�ƾ ���� 
        }
    }
}