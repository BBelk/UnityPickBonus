using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelScript : MonoBehaviour
{
  public PickBonusScript PickBonusScript;
  public ChestScript myChestScript;
  public Animation displayHolderAnim;
  public List<string> animNames;
  public GameObject displayHolderObject;
  public List<ParticleSystem> displayPSs;
  // 0 reward, 1 pooper 
  public TMP_Text displayDenomText;

  public List<GameObject> displayObjects;
  public List<Image> displayWinPoopImages;
  // 0 win, 1 pooper
  public List<Color> displayWinPoopColors;
  // 0 win, 1 win gray, 2 pooper, 3 pooper black

  public RawImage myRawImage;

  public bool canClick;
  public bool isOpen;
  public GameObject panelCoverObject;

  public void DisableClick()
  {
    canClick = false;
  }
  public void ShowReward(float newReward)
  {
    SetColorsActive();
    displayDenomText.text = "" + newReward.ToString("C2");
    ActivateDisplayObject(0);
    // PlayAnim(0);
    StartCoroutine(DelayPlayAnim(0, 0));
  }

  public void ShowPooper()
  {
    ActivateDisplayObject(1);
    // PlayAnim(0);
    StartCoroutine(DelayPlayAnim(0, 1));
  }

  public void ShowDud(float newReward)
  {
    SetColorUnactive();
    if (newReward <= 0f)
    {
      ActivateDisplayObject(1);
    }
    if (newReward > 0f)
    {
      ActivateDisplayObject(0);
      displayDenomText.text = "" + newReward.ToString("C2");
    }
    PlayAnim(0);

  }

  public IEnumerator DelayPlayAnim(int indexAnimation, int indexParticleSystem)
  {
    yield return new WaitForSeconds(0.5f);
    PlayAnim(indexAnimation);
    yield return new WaitForSeconds(0.5f);
    PlayParticleSystem(indexParticleSystem);
  }

  public void ResetToStart()
  {
    //reset everything
    panelCoverObject.SetActive(true);
    isOpen = false;
    SetColorsActive();
    displayHolderObject.transform.localScale = Vector3.zero;
    displayHolderAnim.Stop();
    displayObjects[0].SetActive(false);
    displayObjects[1].SetActive(false);
  }

  public void DoSoftReset()
  {
    PlayAnim(1);
  }

  public void Setup()
  {
    panelCoverObject.SetActive(false);
    canClick = true;
  }

  public void Clicked()
  {
    if (!canClick) { return; }
    canClick = false;
    isOpen = true;
    PickBonusScript.PanelClicked(this);
  }

  public void SetDisplayDenomText(string newString)
  {
    displayDenomText.text = newString;
  }

  public void SetColorsActive()
  {
    displayWinPoopImages[0].color = displayWinPoopColors[0];
    displayWinPoopImages[1].color = displayWinPoopColors[2];
  }

  public void SetColorUnactive()
  {
    displayWinPoopImages[0].color = displayWinPoopColors[1];
    displayWinPoopImages[1].color = displayWinPoopColors[3];
  }

  public void PlayAnim(int animIndex)
  {
    displayHolderAnim.Stop();
    displayHolderAnim.Play(animNames[animIndex]);
  }

  public void PlayParticleSystem(int particleIndex)
  {
    displayPSs[particleIndex].Play();
  }

  public void ActivateDisplayObject(int objIndex)
  {
    displayObjects[0].SetActive(false);
    displayObjects[1].SetActive(false);
    displayObjects[objIndex].SetActive(true);
  }

}
