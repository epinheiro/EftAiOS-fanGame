public struct NoiseInfo
{
    public readonly string tileCode;
    public readonly bool isAttack;

    public NoiseInfo(string tileCode, bool isAttack){
        this.tileCode = tileCode;
        this.isAttack = isAttack;
    }
}
