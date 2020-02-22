using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;

public class PlayerTurnData{

    static int CLASS_HARDCODED_BYTE_SIZE = 24;

    static public readonly int commandCode = (int) ServerCommunication.ServerCommand.PutPlay;
    public readonly int playerId;

    Position movementTo;
    Position sound;

    public PlayerTurnData(int playerId, int newPositionX, int newPositionY, int soundX, int soundY){
        this.playerId = playerId;
        movementTo = new Position{column = newPositionX, row = newPositionY};
        sound = new Position{column = soundX, row = soundY};
    }

    public PlayerTurnData(DataStreamReader reader){
        DataStreamReader.Context readerCtx = default(DataStreamReader.Context);

        int commandCheck = reader.ReadInt(ref readerCtx);

        if (commandCheck == commandCode){
            this.playerId = reader.ReadInt(ref readerCtx);
            movementTo = new Position{column = reader.ReadInt(ref readerCtx), row = reader.ReadInt(ref readerCtx)};
            sound = new Position{column = reader.ReadInt(ref readerCtx), row = reader.ReadInt(ref readerCtx)};
        }else{
            readerCtx = default(DataStreamReader.Context);
            throw new System.Exception(string.Format("Command {0} received is not compatible with command {1}", commandCheck, commandCode));
        }
    }

    static public DataStreamWriter CreateAndPackPlayerTurnData(int playerId, int newPositionX, int newPositionY, int soundX, int soundY){
        DataStreamWriter writer = new DataStreamWriter(CLASS_HARDCODED_BYTE_SIZE, Allocator.Temp);

        writer.Write(commandCode);
        writer.Write(playerId);
        writer.Write(newPositionX);
        writer.Write(newPositionY);
        writer.Write(soundX);
        writer.Write(soundY);

        return writer;
    }

    public DataStreamWriter PackPlayerTurnObjectData(){
        return PlayerTurnData.CreateAndPackPlayerTurnData(playerId,movementTo.column,movementTo.row,sound.column, sound.row);
    }

    public override string ToString(){
        return string.Format("Command {0} : Player{1} - mov({2},{3}) - sound({4},{5})",
        (ServerCommunication.ServerCommand) commandCode, playerId, movementTo.column, movementTo.row, sound.column, sound.row);
    }
}

public struct Position{
    public int column;
    public int row;
}
