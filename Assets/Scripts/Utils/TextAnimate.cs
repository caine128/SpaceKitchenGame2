using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextAnimate : MonoBehaviour
{
    private TextMeshProUGUI tmp_component;
    public IEnumerator CO => _co;
    private IEnumerator _co;

    private TMP_TextInfo textInfo;


    [SerializeField] private AnimationCurve easeCurve;

    int materialIndex;
    private TMP_MeshInfo[] cachedVertexData;
    private Color32[] vertexColors;
    private Vector3[] vertexPositions;

    private void Awake()
    {
        tmp_component = GetComponent<TextMeshProUGUI>();
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Z))
    //    {
    //        Debug.Log("pressed");
    //        SetTextAlphaInstant(isVisible: false);
    //        Reveal();
    //    }
    //    else if (Input.GetKeyDown(KeyCode.M))
    //    {
    //        Debug.Log("pressed");
    //        InstantFinaliseAndReveal();
    //    }

    //    else if (Input.GetKeyDown(KeyCode.K))
    //    {
    //        Debug.Log("pressed");
    //        SetTextSizeInstant(isFullSize: false);
    //        UpSize();
    //    }
    //}



    private int GetMaterialIndexFromFirstVisible()
    {
        int? materialReferenceIndex = null;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (textInfo.characterInfo[i].isVisible)
            {
                materialReferenceIndex = textInfo.characterInfo[i].materialReferenceIndex;
                break;
            }
        }
        return materialReferenceIndex.Value;
    }

    private void PrepareComponent()
    {
        tmp_component.ForceMeshUpdate();

        textInfo = tmp_component.textInfo;
        materialIndex = GetMaterialIndexFromFirstVisible();

        cachedVertexData = textInfo.CopyMeshInfoVertexData();
    }

    public void SetVisibility(bool isVisible)
    {
        if (isVisible) tmp_component.color = new Color(tmp_component.color.r, tmp_component.color.g, tmp_component.color.b, 1);
        else tmp_component.color = new Color(tmp_component.color.r, tmp_component.color.g, tmp_component.color.b, 0);
    }

    public void Reveal(Action followingAction_IN = null, float lerpSpeedModifier = 1f)
    {
        PrepareComponent();

        vertexColors = textInfo.meshInfo[materialIndex].colors32;

        SetTextAlphaInstant(isVisible: false);  

        if (_co is not null)
        {
            StopCoroutine(_co);
            _co = null;
        }
        _co = RevealRoutine(followingAction_IN,lerpSpeedModifier);
        StartCoroutine(_co);
    }


    public void UpSize(Action followingAction_IN = null, float lerpSpeedModifier = 1f)
    {
        PrepareComponent();

        vertexPositions = textInfo.meshInfo[materialIndex].vertices;
        SetTextSizeInstant(isFullSize : false); 

        if (_co is not null)
        {
            StopCoroutine(_co);
            _co = null;
        }
        _co = UpSizeRoutine(followingAction_IN, lerpSpeedModifier);
        StartCoroutine(_co);
    }

    private void SetTextSizeInstant(bool isFullSize)
    {
        float newYPoint = (vertexPositions[0].y + vertexPositions[2].y) / 2;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            int vertexIndex = charInfo.vertexIndex;

            if (!charInfo.isVisible) continue;

            vertexPositions[vertexIndex + 0] = new Vector3(cachedVertexData[materialIndex].vertices[vertexIndex + 0].x, isFullSize ? cachedVertexData[materialIndex].vertices[vertexIndex + 0].y : newYPoint, cachedVertexData[materialIndex].vertices[vertexIndex + 0].z);
            vertexPositions[vertexIndex + 1] = new Vector3(cachedVertexData[materialIndex].vertices[vertexIndex + 1].x, isFullSize ? cachedVertexData[materialIndex].vertices[vertexIndex + 1].y : newYPoint, cachedVertexData[materialIndex].vertices[vertexIndex + 1].z);
            vertexPositions[vertexIndex + 2] = new Vector3(cachedVertexData[materialIndex].vertices[vertexIndex + 2].x, isFullSize ? cachedVertexData[materialIndex].vertices[vertexIndex + 2].y : newYPoint, cachedVertexData[materialIndex].vertices[vertexIndex + 2].z);
            vertexPositions[vertexIndex + 3] = new Vector3(cachedVertexData[materialIndex].vertices[vertexIndex + 3].x, isFullSize ? cachedVertexData[materialIndex].vertices[vertexIndex + 3].y : newYPoint, cachedVertexData[materialIndex].vertices[vertexIndex + 3].z);

            tmp_component.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
        }
    }

    private void SetTextAlphaInstant(bool isVisible)
    {

        Color32 newColor = isVisible == true ? cachedVertexData[materialIndex].colors32[0] : new Color32(255, 255, 255, 0);

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            int vertexIndex = charInfo.vertexIndex;

            if (!charInfo.isVisible) continue;

            vertexColors[vertexIndex + 0] = newColor;
            vertexColors[vertexIndex + 1] = newColor;
            vertexColors[vertexIndex + 2] = newColor;
            vertexColors[vertexIndex + 3] = newColor;

            tmp_component.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }
    }


    private IEnumerator UpSizeRoutine(Action followingAction_IN, float lerpSpeedModifier)
    {
        float elapsedTime = 0;

        while (elapsedTime < (TimeTickSystem.TEXTANIM_LERPDURATION * lerpSpeedModifier))
        {
            float easeFactor = elapsedTime / (TimeTickSystem.TEXTANIM_LERPDURATION * lerpSpeedModifier);
            easeFactor = easeCurve.Evaluate(easeFactor);

            for (int i = 0; i < vertexPositions.Length; i++)
            {
                vertexPositions[i] = new Vector3(cachedVertexData[materialIndex].vertices[i].x, Mathf.LerpUnclamped(vertexPositions[i].y, cachedVertexData[materialIndex].vertices[i].y, easeFactor), cachedVertexData[materialIndex].vertices[i].z);

                if (i != 0 && i > Mathf.FloorToInt(vertexPositions.Length * easeFactor)) break;
            }

            elapsedTime += Time.deltaTime;
            tmp_component.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
            yield return null;
        }

        SetTextSizeInstant(isFullSize: true);
        followingAction_IN?.Invoke();
        _co = null;
    }




    private IEnumerator RevealRoutine(Action followingAction_IN, float lerpSpeedModifier)
    {
        float elapsedTime = 0f;

        while (elapsedTime < (TimeTickSystem.TEXTANIM_LERPDURATION* lerpSpeedModifier))
        {
            float easeFactor = elapsedTime / (TimeTickSystem.TEXTANIM_LERPDURATION* lerpSpeedModifier);
            easeFactor = easeCurve.Evaluate(easeFactor);

            for (int i = 0; i < vertexColors.Length; i++)
            {
                if (vertexColors[i].a >= 255) continue;
                vertexColors[i] = Color32.Lerp(vertexColors[i], cachedVertexData[materialIndex].colors32[i], easeFactor);
                if (i != 0 && i > Mathf.FloorToInt(vertexColors.Length * easeFactor)) break;
            }

            elapsedTime += Time.deltaTime;
            tmp_component.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            yield return null;
        }

        SetTextAlphaInstant(isVisible: true);
        followingAction_IN?.Invoke();
        _co = null;
        

    }

    public void InstantFinaliseAndResolveVisibility(bool isVisible)
    {
        if (_co is not null)
        {
            StopCoroutine(_co);
            if(vertexColors is not null) SetTextAlphaInstant(isVisible: isVisible);
            if(vertexPositions is not null) SetTextSizeInstant(isFullSize: isVisible);
            _co = null;
        }
    }
}
