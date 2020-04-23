public struct NoiseInfo
{
    public readonly string tileCode;
    public readonly bool isAttack;
    public readonly UnityEngine.Material uiColor;

    public NoiseInfo(string tileCode, bool isAttack, UnityEngine.Material uiColor){
        this.tileCode = tileCode;
        this.isAttack = isAttack;
        this.uiColor = uiColor;
    }
}
