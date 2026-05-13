using UnityEngine;

[System.Serializable]
public enum ToLevel
{
	Tutorial,
	Level1,
	Level2,
	Level3,
	Level4
};

public class MoneySystem : MonoBehaviour
{
  public static MoneySystem Instance;

  // the value of the target item for this level, set per level
  public float levelStartMoney = 200000f;

  // percentage of starting money deducted per hit, set to 15%
  public float penaltyPercent = 0.15f;

  public ToLevel toLevel;

  // tracks money remaining this level
  public float CurrentMoney { get; private set; }

  // tracks cumulative money earned across all levels
  public float TotalMoney { get; private set; }

  void Awake()
  {
    Instance = this;
    CurrentMoney = levelStartMoney;
  }

  // deducts a percentage of starting money on hit
  public void TakeDamage()
  {
    float penalty = levelStartMoney * penaltyPercent;
    CurrentMoney -= penalty;
    Debug.Log($"Hit. Money remaining: {CurrentMoney}");

    if (CurrentMoney <= 0)
    {
      CurrentMoney = 0;
      GameOver();
    }
  }

  // called when player escapes successfully, adds remaining money to total
  public void LevelComplete()
  {
    TotalMoney += CurrentMoney;
    EscapeTimer.Instance.StopTimer();
    Debug.Log($"Level complete. Earned: {CurrentMoney}, Total: {TotalMoney}");
    
		GameLevelManager.Instance.LevelComplete();
    
  }

  // resets money for the current level on game over
  public void ResetLevel()
  {
    CurrentMoney = levelStartMoney;
    GameLevelManager.Instance.LoadGameLevel(GameLevelManager.Instance.currentLevel.name);
  }

  // hook into scene transition later
  void GameOver()
  {
    Debug.Log("No money left. Game Over.");
    ResetLevel();
  }
}