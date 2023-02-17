using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PickBonusScript : MonoBehaviour
{
  public List<Button> playAndDenomButtons;
  // 0 play, 1 left, 2 right
  public List<TMP_Text> allTMPTexts;
  // 0 denom, 1 current balance, 2 last win
  public List<ChestScript> allChestScripts;
  public List<GameObject> allChestObjects;
  public List<PanelScript> allChestPanelScripts;
  public List<GameObject> allChestPanelObjects;

  public Transform chestHolderTransform;
  public Transform gridTransform;
  public float currentBalanceFloat = 10f;
  public float lastWinFloat = 0f;
  public float fullReward;

  public int[] thirtyPercentArr = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
  public int[] fifteenPercentArr = new int[] { 12, 16, 24, 32, 48, 64 };
  public int[] fivePercentArr = new int[] { 100, 200, 300, 400, 500 };
  public float[] denomOptionArr = new float[] { 0.25f, 0.5f, 1.0f, 5.0f };
  public int currentDenomIndex;

  public List<float> rewardOptions;

  public int clickedInt;

  public int minimumRandom = 0;
  public Toggle togglePooper;

  public Coroutine currentBalanceCo;
  public Coroutine lastWinCo;


  void Start()
  {
    currentDenomIndex = 2;
    ButtonDenom(0);
    SetCurrentBalanceText();
    SetLastWinText();
    GeneratePanelsAndChests();

    HardReset();
  }

  public void GeneratePanelsAndChests()
  {
    var defaultChestObject = allChestObjects[0];
    var defaultPanelObject = allChestPanelObjects[0];
    var chestXPos = 0f;
    for (int x = 1; x < 9; x++)
    {
      //
      var newChestObject = Instantiate(defaultChestObject, chestHolderTransform);
      var newPanelObject = Instantiate(defaultPanelObject, gridTransform);
      //
      allChestObjects.Add(newChestObject);
      allChestPanelObjects.Add(newPanelObject);
      //
      var getChestScript = newChestObject.GetComponent<ChestScript>();
      var getPanelScript = newPanelObject.GetComponent<PanelScript>();
      //
      allChestScripts.Add(getChestScript);
      allChestPanelScripts.Add(getPanelScript);
      //
      AssignRenderTextures(getPanelScript, getChestScript);
      chestXPos -= 100f;
      newChestObject.transform.localPosition = new Vector3(chestXPos, 0f, 0f);
    }

    for (int y = 0; y < 9; y++)
    {
      allChestPanelScripts[y].myChestScript = allChestScripts[y];
    }
  }

  public void AssignRenderTextures(PanelScript newPanelScript, ChestScript newChestScript)
  {
    var newRT = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
    newRT.Create();
    newPanelScript.myRawImage.texture = newRT;
    //
    newChestScript.myCamera.targetTexture = newRT;
  }

  public void DoTogglePooper()
  {
    var isOn = togglePooper.isOn;
    if (isOn) { minimumRandom = 50; }
    if (!isOn) { minimumRandom = 0; }
    togglePooper.SetIsOnWithoutNotify(isOn);
  }

  public void GiveMoneyCheat()
  {
    currentBalanceFloat += 10f;
    DoTextCoroutine(1);
    ButtonDenom(0);
  }

  public void ButtonPlay()
  {
    currentBalanceFloat -= denomOptionArr[currentDenomIndex];
    DoTextCoroutine(1);
    //
    lastWinFloat = 0f;
    DoTextCoroutine(2);
    foreach (Button newButton in playAndDenomButtons)
    {
      newButton.interactable = false;
    }
    AssignRewards();
  }

  public void DoTextCoroutine(int textIndex)
  {
    if (textIndex == 1)
    {
      if (currentBalanceCo != null) { StopCoroutine(currentBalanceCo); }
      currentBalanceCo = StartCoroutine(UpdateTextCoroutine(textIndex));
    }
    if (textIndex == 2)
    {
      if (lastWinCo != null) { StopCoroutine(lastWinCo); }
      lastWinCo = StartCoroutine(UpdateTextCoroutine(textIndex));
    }
  }

  public IEnumerator UpdateTextCoroutine(int textIndex)
  {
    var elapsedTime = 0f;
    var updateSpeed = 0.5f;
    var endBalance = currentBalanceFloat;
    if (textIndex == 2) { endBalance = lastWinFloat; }

    var startAmount = float.Parse(allTMPTexts[textIndex].text.Replace("$", ""));
    while (elapsedTime < updateSpeed)
    {
      var newAmount = Mathf.Lerp(startAmount, endBalance, elapsedTime / updateSpeed);
      allTMPTexts[textIndex].text = newAmount.ToString("C2");
      elapsedTime += Time.deltaTime;
      yield return new WaitForEndOfFrame();
    }
    if (elapsedTime >= updateSpeed)
    {
      if (textIndex == 1)
      {
        SetCurrentBalanceText();
        StopCoroutine(currentBalanceCo);
      }
      if (textIndex == 2)
      {
        SetLastWinText();
        StopCoroutine(lastWinCo);
      }
    }
  }

  public void AssignRewards()
  {
    rewardOptions.Clear();
    int randomInt = UnityEngine.Random.Range(minimumRandom, 100);
    var currentMultiplier = 0;
    if (randomInt > 49 && randomInt < 79)
    {
      int index = UnityEngine.Random.Range(0, thirtyPercentArr.Length);
      currentMultiplier = thirtyPercentArr[index];
    }
    else if (randomInt < 94 && randomInt >= 79)
    {
      int index = UnityEngine.Random.Range(0, fifteenPercentArr.Length);
      currentMultiplier = fifteenPercentArr[index];
    }
    else if (randomInt >= 95)
    {
      int index = UnityEngine.Random.Range(0, fivePercentArr.Length);
      currentMultiplier = fivePercentArr[index];
    }

    fullReward = denomOptionArr[currentDenomIndex] * (float)currentMultiplier;
    var tempReward = fullReward;

    if (currentMultiplier != 0)
    {
      var chestAmount = UnityEngine.Random.Range(2, 8);
      for (int i = 0; i < chestAmount - 1; i++)
      {
        var split = Random.Range(0f, tempReward - 0.1f);
        split = Mathf.Round(split / 0.05f) * 0.05f;
        rewardOptions.Add(split);
        tempReward -= split;
      }
      tempReward = Mathf.Round(tempReward / 0.05f) * 0.05f;
      rewardOptions.Add(tempReward);

      var tempList = new List<float>();
      //get rid of 0s
      for (int x = 0; x < rewardOptions.Count; x++)
      {
        if (rewardOptions[x] > 0)
        {
          tempList.Add(rewardOptions[x]);
        }
      }
      // then make the biggest one last (for the drama)
      var largestIndex = -1;
      var largestFloat = 0f;
      for (int y = 0; y < tempList.Count; y++)
      {
        if (tempList[y] > largestFloat)
        {
          largestIndex = y;
          largestFloat = tempList[y];
        }
      }
      tempList.RemoveAt(largestIndex);
      tempList.Add(largestFloat);

      rewardOptions.Clear();
      rewardOptions = tempList;

    }

    foreach (PanelScript newPanelScript in allChestPanelScripts)
    {
      newPanelScript.Setup();
    }

    //uncomment to see the values when theyre generated

    // Debug.Log("CURRENT MULTPLIER: " + currentMultiplier + " CURRENT TO WIN: " + fullReward.ToString("C2"));
    // foreach(float newFloat in rewardOptions){
    //     Debug.Log("REWARD PORTION: " + newFloat.ToString("C2"));
    // }
    //     Debug.Log("---------------");

  }



  public void ButtonDenom(int dirIndex)
  {
    currentDenomIndex = currentDenomIndex + dirIndex;
    var newDenom = denomOptionArr[currentDenomIndex];
    if (currentDenomIndex <= 0)
    {
      playAndDenomButtons[1].interactable = false;
      playAndDenomButtons[2].interactable = true;
    }
    if (currentDenomIndex > 0 && currentDenomIndex < denomOptionArr.Length)
    {
      playAndDenomButtons[1].interactable = true;
      playAndDenomButtons[2].interactable = true;
    }
    if (currentDenomIndex >= denomOptionArr.Length - 1)
    {
      playAndDenomButtons[1].interactable = true;
      playAndDenomButtons[2].interactable = false;
    }

    allTMPTexts[0].text = "" + denomOptionArr[currentDenomIndex].ToString("C2");
    if (newDenom <= currentBalanceFloat) { playAndDenomButtons[0].interactable = true; }
    if (newDenom > currentBalanceFloat) { playAndDenomButtons[0].interactable = false; }
  }


  public void SetCurrentBalanceText()
  {
    allTMPTexts[1].text = "" + currentBalanceFloat.ToString("C2");
  }
  public void SetLastWinText()
  {
    allTMPTexts[2].text = "" + lastWinFloat.ToString("C2");
  }

  public void PanelClicked(PanelScript newPanelScript)
  {
    var getIndex = allChestPanelScripts.IndexOf(newPanelScript);
    if (clickedInt >= rewardOptions.Count)
    {
      // do pooper
      clickedInt = 0;
      newPanelScript.ShowPooper();
      allChestScripts[getIndex].DoLid(120f, 0.5f, 1);
      StartCoroutine(GameOver());
      return;
    }
    if (clickedInt < rewardOptions.Count)
    {
      allChestScripts[getIndex].DoLid(120f, 0.5f, 0);
      newPanelScript.ShowReward(rewardOptions[clickedInt]);
      lastWinFloat += rewardOptions[clickedInt];
      DoTextCoroutine(2);

    }

    clickedInt += 1;
  }

  public void DisableAllClicks()
  {
    foreach (PanelScript newPanelScript in allChestPanelScripts)
    {
      newPanelScript.DisableClick();
    }
  }

  public void ShowOtherChests()
  {
    for (int x = 0; x < allChestPanelScripts.Count; x++)
    {
      var newPanelScript = allChestPanelScripts[x];
      if (!newPanelScript.isOpen)
      {
        var rewardOrPooper = UnityEngine.Random.Range(0, 3);
        if (rewardOrPooper == 0)
        {
          newPanelScript.ShowDud(0f);
        }
        if (rewardOrPooper != 0)
        {
          var getRandomValue = Mathf.Round(UnityEngine.Random.Range(5f, (100f * denomOptionArr[currentDenomIndex])) / 0.05f) * 0.05f;
          newPanelScript.ShowDud(getRandomValue);
        }
        allChestScripts[x].DoLid(120f, 0.25f, -1);
      }
    }
  }

  public void HardReset()
  {
    for (int x = 0; x < allChestPanelScripts.Count; x++)
    {
      allChestPanelScripts[x].ResetToStart();
      allChestScripts[x].ResetToStart();
    }
    playAndDenomButtons[0].interactable = true;
    ButtonDenom(0);
  }

  public void SoftReset()
  {
    for (int x = 0; x < allChestPanelScripts.Count; x++)
    {
      allChestPanelScripts[x].DoSoftReset();
      allChestScripts[x].DoSoftReset();
    }
  }

  public IEnumerator GameOver()
  {
    DisableAllClicks();
    yield return new WaitForSeconds(1.5f);
    ShowOtherChests();
    yield return new WaitForSeconds(2f);
    SoftReset();
    currentBalanceFloat += lastWinFloat;
    DoTextCoroutine(1);
    yield return new WaitForSeconds(0.75f);
    HardReset();
  }

}
