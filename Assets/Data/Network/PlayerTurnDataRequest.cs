using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;

public class PlayerTurnDataRequest{

    static int CLASS_HARDCODED_BYTE_SIZE = 28;

    static public readonly int commandCode = (int) ServerCommunication.ServerCommand.PutPlay;
    public readonly int playerId;

    public readonly Vector2Int movementTo;
    public readonly Vector2Int sound;

    public readonly int playerAttacked;

    public PlayerTurnDataRequest(int playerId, int newPositionX, int newPositionY, int soundX, int soundY, int playerAttacked){
        this.playerId = playerId;
        this.movementTo = new Vector2Int(newPositionX, newPositionY);
        this.sound = new Vector2Int(soundX, soundY);
        this.playerAttacked = playerAttacked;
    }

    public PlayerTurnDataRequest(DataStreamReader reader){
        DataStreamReader.Context readerCtx = default(DataStreamReader.Context);

        int commandCheck = reader.ReadInt(ref readerCtx);

        if (commandCheck == commandCode){
            this.playerId = reader.ReadInt(ref readerCtx);
            this.movementTo = new Vector2Int(reader.ReadInt(ref readerCtx), reader.ReadInt(ref readerCtx));
            this.sound = new Vector2Int(reader.ReadInt(ref readerCtx), reader.ReadInt(ref readerCtx));
            this.playerAttacked = reader.ReadInt(ref readerCtx);
        }else{
            readerCtx = default(DataStreamReader.Context);
            throw new System.Exception(string.Format("Command {0} received is not compatible with command {1}", commandCheck, commandCode));
        }
    }

    static public DataStreamWriter CreateAndPackPlayerTurnData(int playerId, int newPositionX, int newPositionY, int soundX, int soundY, int playerAttacked){
        DataStreamWriter writer = new DataStreamWriter(CLASS_HARDCODED_BYTE_SIZE, Allocator.Temp);

        writer.Write(commandCode);
        writer.Write(playerId);
        writer.Write(newPositionX);
        writer.Write(newPositionY);
        writer.Write(soundX);
        writer.Write(soundY);
        writer.Write(playerAttacked);

        return writer;
    }

    public DataStreamWriter PackPlayerTurnObjectData(){
        return PlayerTurnDataRequest.CreateAndPackPlayerTurnData(playerId,movementTo.x,movementTo.y,sound.x, sound.y, playerAttacked);
    }

    public override string ToString(){
        return string.Format("Command {0} : Player{1} - mov({2},{3}) - sound({4},{5})",
        (ServerCommunication.ServerCommand) commandCode, playerId, movementTo.x, movementTo.y, sound.x, sound.y);
    }
}