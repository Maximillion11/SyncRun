using System.Collections.Generic;

[System.Serializable]
public class FrameData
{
    public List<float> PositionsX = new List<float>();
    public List<float> PositionsY = new List<float>();

    public FrameData(List<float> positionXF, List<float> positionYF)
    {
        PositionsX = positionXF;
        PositionsY = positionYF;
    }
}