using System.Collections;
using UnityEngine;

public class EscapeTimer : MonoBehaviour
{
  public static EscapeTimer Instance;

  // seconds the player has to escape after picking up target item
  public float escapeTime = 60f;

  // seconds the target item can sit on the floor before returning to its spawn
  public float floorReturnTime = 10f;

  // true while the escape countdown is actively running
  public bool TimerRunning { get; private set; }

  // current time left on the escape countdown
  private float timeRemaining;

  // reference to the active floor return coroutine so it can be cancelled on another pickup
  private Coroutine floorCoroutine;

  void Awake()
  {
    Instance = this;
  }

  // begins the escape countdown  when the target item is first picked up
  public void StartTimer()
  {
    if (TimerRunning) return;
    TimerRunning = true;
    timeRemaining = escapeTime;
    StartCoroutine(CountDown());
  }

  // ticks down each frame and triggers game over when time hits zero
  IEnumerator CountDown()
  {
    while (timeRemaining > 0)
    {
      timeRemaining -= Time.deltaTime;
      yield return null;
    }
    TimerRunning = false;
    GameOver();
  }

  // starts the 10 second countdown to return the target item if left on the floor
  public void StartFloorCountdown(ItemPickup item, Vector3 origin)
  {
    if (floorCoroutine != null)
      StopCoroutine(floorCoroutine);
    floorCoroutine = StartCoroutine(FloorReturn(item, origin));
  }

  // waits for the amount of seconds set in floorReturnTime then returns the item if it still has no parent
  IEnumerator FloorReturn(ItemPickup item, Vector3 origin)
  {
    yield return new WaitForSeconds(floorReturnTime);
    if (item.transform.parent == null)
      item.ReturnToOrigin();
  }

  // stops the floor return coroutine when the item is picked back up
  public void CancelFloorCountdown()
  {
    if (floorCoroutine != null)
      StopCoroutine(floorCoroutine);
  }

  // called when the escape timer hits zero
  // implement Game Over logic here later
  void GameOver()
  {
    Debug.Log("Time's up. Game Over.");
  }

  // stops the escape timer when the player successfully escapes
  public void StopTimer()
  {
    TimerRunning = false;
    StopAllCoroutines();
  }

  // exposes remaining time for UI display
  public float TimeRemaining => timeRemaining;
}