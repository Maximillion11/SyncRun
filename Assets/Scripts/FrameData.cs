using System.Collections.Generic;

[System.Serializable]
public class FrameData
{
    public float StartPositionX = 0;
    public float StartPositionY = 0;
    public int LastFrame = 0;
    public Dictionary<int, float> Horizontal = new Dictionary<int, float>();
    public Dictionary<int, bool> JumpBool = new Dictionary<int, bool>();

    public FrameData(float startPositionXF, float startPositionYF, int lastFrameI, Dictionary<int, float> horizontalF, Dictionary<int, bool> jumpBoolB)
    {
        StartPositionX = startPositionXF;
        StartPositionY = startPositionYF;
        LastFrame = lastFrameI;
        Horizontal = horizontalF;
        JumpBool = jumpBoolB;
    }
}