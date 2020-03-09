using UnityEngine;
using Unity.Networking.Transport;

public class PutPlayRequest : INetworkData
{
    static public readonly int commandCode = (int) ServerCommunication.ServerCommand.PutPlay;
    public readonly int playerId;

    public readonly Vector2Int movementTo;
    public readonly Vector2Int sound;

    int _playerAttacked;
    public bool PlayerAttacked{
        get { return _playerAttacked == 1 ? true : false; }
        set { _playerAttacked = value ? 1 : 0; }
    }

    public int[] DataToArray(){
        return new int[]{commandCode, playerId, movementTo.x, movementTo.y, sound.x, sound.y, _playerAttacked};
    }

    /// <summary>
    /// This constructor is for WRAPPING the data to make a request
    /// </summary> 
    public PutPlayRequest(int playerId, int moveToX, int moveToY, int soundInX, int soundInY, bool playerAttacked){
        this.playerId = playerId;
        movementTo = new Vector2Int(moveToX, moveToY);
        sound = new Vector2Int(soundInX, soundInY);
        this._playerAttacked = playerAttacked ? 1 : 0;
    }

    /// <summary>
    /// This constructor is for UNWRAPPING the data to read a request
    /// </summary> 
    public PutPlayRequest(DataStreamReader reader){
        DataStreamReader.Context readerCtx = default(DataStreamReader.Context);

        int commandCheck = reader.ReadInt(ref readerCtx);

        if (commandCheck == commandCode){
            this.playerId = reader.ReadInt(ref readerCtx);
            this.movementTo = new Vector2Int(reader.ReadInt(ref readerCtx), reader.ReadInt(ref readerCtx));
            this.sound = new Vector2Int(reader.ReadInt(ref readerCtx), reader.ReadInt(ref readerCtx));
            this._playerAttacked = reader.ReadInt(ref readerCtx);
        }else{
            readerCtx = default(DataStreamReader.Context);
            throw new System.Exception(string.Format("Command {0} received is not compatible with this class command {1}", commandCheck, commandCode));
        }
    }
}
