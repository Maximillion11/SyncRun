using System.Collections.Generic;

[System.Serializable]
public class FrameData
{
    public List<float> positionsX = new List<float>();
    public List<float> positionsY = new List<float>();

    public FrameData(List<float> positionXF, List<float> positionYF)
    {
        positionsX = positionXF;
        positionsY = positionYF;
    }
}