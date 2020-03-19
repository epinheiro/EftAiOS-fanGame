using Unity.Burst;
using Unity.Networking.Transport;
using Unity.Collections;
using Unity.Jobs;

[BurstCompile]
struct SendDataJob : IJob {
    public UdpNetworkDriver driver;
    public NetworkConnection connection;

    [DeallocateOnJobCompletion]
    public NativeArray<int> varargs;

    public void Execute(){
        const int integerByteSize = 4;
        int dataPackSize = varargs.Length * integerByteSize;

        DataStreamWriter dataPack = new DataStreamWriter(dataPackSize, Allocator.Temp);

        for(int i=0 ; i<varargs.Length ; i++){
            dataPack.Write(varargs[i]);
        }

        connection.Send(driver, dataPack);
    }
}
