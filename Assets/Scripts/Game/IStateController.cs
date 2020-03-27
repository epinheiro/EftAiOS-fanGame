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

    protected virtual void StateEnd(){
        throw new System.Exception("IStateController child must implements own StateEnd and calls it when control passes to next state");
    }

    protected void ResetStateController(){
        isRunning = false;
    }
}
