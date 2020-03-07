using Unity.Networking.Transport;
using Unity.Jobs;

public interface INetworkData
{
    /// <summary>
    /// Serializing the data to a array of integers, that will be used by SendDataJob
    /// </summary> 
    int[] DataToArray();
}
