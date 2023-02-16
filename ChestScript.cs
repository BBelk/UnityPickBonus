using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChestScript : MonoBehaviour
{
    public GameObject chestBaseObject;
    public GameObject chestLidObject;
    public List<ParticleSystem> allChestPS;
    public Coroutine lidCo;
    public Camera myCamera;

    public void ResetToStart(){
        if(lidCo != null){StopCoroutine(lidCo);}
        allChestPS[0].Stop();
        allChestPS[1].Stop();
        chestLidObject.transform.localEulerAngles = Vector3.zero;
    }

    public void DoSoftReset(){
        if(lidCo != null){StopCoroutine(lidCo);}
        StartCoroutine(DelayLid());
    }

    public IEnumerator DelayLid(){
        yield return new WaitForSeconds(0.5f);
        DoLid(0f, 0.25f, -1);
    }
    public void DoLid(float newAngle, float lidTime, int indexParticleSystem){
        if(lidCo != null){StopCoroutine(lidCo);}
        lidCo = StartCoroutine(LidCoroutine(newAngle, lidTime));
        PlayParticleSystem(indexParticleSystem);
    }

    public void PlayParticleSystem(int indexParticleSystem){
        if(indexParticleSystem == 0 || indexParticleSystem == 1){
            allChestPS[indexParticleSystem].Play();
        }
    }

    public IEnumerator LidCoroutine(float newAngle, float lidTime){
        var currentAngle = chestLidObject.transform.localEulerAngles.x;
        //
        // I had to add this because of quanternions, I don't have time to play with quanternions unless I'm getting paid for it :)
        if(newAngle == 0f){currentAngle += 60f;}
        //
        //
        var elapsedTime = 0f;
        while(elapsedTime < lidTime){
            chestLidObject.transform.localEulerAngles = new Vector3(Mathf.Lerp(currentAngle, newAngle, elapsedTime / lidTime), 0f, 0f);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        if(elapsedTime >= lidTime){
            chestLidObject.transform.localEulerAngles = new Vector3(newAngle, 0f, 0f);
            StopCoroutine(lidCo);
        }
    }
}
