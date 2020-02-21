using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;

public class ClientData{

    static int CLASS_HARDCODED_BYTE_SIZE = 20;
    public int playerId;

    public Position movement;
    public Position sound;

    public ClientData(){
        
    }

    public ClientData(int playerId, int newPositionX, int newPositionY, int soundX, int soundY){
        this.playerId = playerId;
        movement = new Position{column = newPositionX, row = newPositionY};
        sound = new Position{column = soundX, row = soundY};
    }

    public ClientData(DataStreamReader reader){
        DataStreamReader.Context readerCtx = default(DataStreamReader.Context);

        this.playerId = reader.ReadInt(ref readerCtx);
        movement = new Position{column = reader.ReadInt(ref readerCtx), row = reader.ReadInt(ref readerCtx)};
        sound = new Position{column = reader.ReadInt(ref readerCtx), row = reader.ReadInt(ref readerCtx)};
    }

    static public DataStreamWriter DirectlyPackCliendData(int playerId, int newPositionX, int newPositionY, int soundX, int soundY){
        DataStreamWriter writer = new DataStreamWriter(CLASS_HARDCODED_BYTE_SIZE, Allocator.Temp);

        writer.Write(playerId);
        writer.Write(newPositionX);
        writer.Write(newPositionY);
        writer.Write(soundX);
        writer.Write(soundY);

        return writer;
    }

    public DataStreamWriter PackClientData(){
        return ClientData.DirectlyPackCliendData(playerId,movement.column,movement.row,sound.column, sound.row);
    }

    public override string ToString(){
        return string.Format("Player{0} - mov:({1},{2}) - sound:({3},{4})",
        playerId, movement.column, movement.row, sound.column, sound.row);
    }
}

public struct Position{
    public int column;
    public int row;
}
