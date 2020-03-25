public abstract class IStateController
{
    bool isRunning = false;

    public void Execute(){
        if(!isRunning){
            isRunning = true;
            GUISetter();
        }else{
            ExecuteLogic();
        }
    }

    protected virtual void ExecuteLogic(){
        throw new System.Exception("IStateController child must implements own Execute");
    }

    protected virtual void GUISetter(){
        throw new System.Exception("IStateController child must implements own GUISetter");
    }

    protected void ResetStateController(){
        isRunning = false;
    }
}
