namespace TeachMod.UtilityType;

/// <summary>
/// 如果value达到Max那么就开始减小，如果value达到Min那么就开始增加
/// </summary>
/// <param name="value">数值</param>
/// <param name="max">最大值</param>
/// <param name="min">最小值</param>
/// <param name="step">步频</param>
public class IfMaxSubElseAdd(float value, float max, float min, float step = 0.1f)
{
    public float Step { get; set; } = step;
    public float Max { get; set; } = max;
    public float Min { get; set; } = min;
    public float Value { get; set; } = value;
    private Statu statu = Statu.Add;
    public static implicit operator float(IfMaxSubElseAdd value) => value.Value;
    public static explicit operator int(IfMaxSubElseAdd value) => (int)value.Value;
    public static explicit operator long(IfMaxSubElseAdd value) => (long)value.Value;
    public IfMaxSubElseAdd Run()
    {
        if (Value >= Max - Step) {
            statu = Statu.Sub;
        } else if (Value <= Min + Step) {
            statu = Statu.Add;
        }
        switch(statu) {
            case Statu.Add: 
            {
                Value += Step;
                break;
            }
            case Statu.Sub: 
            {
                Value -= Step;
                break;
            }
        }
        return this;
    }

    private enum Statu
    {
        Add,
        Sub
    }
}
