using UnityEngine;

public class RangeSliderAttribute : PropertyAttribute
{
    public float min;
    public float max;

    public RangeSliderAttribute(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
}  
