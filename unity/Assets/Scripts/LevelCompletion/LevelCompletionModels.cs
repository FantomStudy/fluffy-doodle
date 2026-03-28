using System;

[Serializable]
public sealed class LevelCompletionRequest
{
    public bool completed = true;
}

[Serializable]
public sealed class LevelCompletionResponse
{
    public bool success;
    public string message;
    public LevelCompletionResponseData data;
}

[Serializable]
public sealed class LevelCompletionResponseData
{
    public string levelId;
    public bool isCompleted;
    public bool wasCompleted;
    public int awardedStars;
    public int awardedExp;
    public int currentStars;
    public int currentExp;
    public int currentLevel;
}
