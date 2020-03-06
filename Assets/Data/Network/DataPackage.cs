using Unity.Burst;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using Unity.Jobs;
using System.Collections.Generic;
using System;

public static class DataPackageWrapper
{
    public static IJob CreateSendDataJob(UdpNetworkDriver driver, NetworkConnection connection, params int[] integersToPack){       
        NativeArray<int> array = new NativeArray<int>(integersToPack, Allocator.TempJob);
        
        SendDataJob sendData = new SendDataJob{
            driver = driver,
            connection = connection,
            varargs = array
        };

        return sendData;
    }

}
