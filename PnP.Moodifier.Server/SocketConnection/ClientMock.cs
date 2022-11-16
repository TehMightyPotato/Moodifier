using SimpleSockets.Client;

namespace MightyPotato.PnP.Moodifier.Server.Connection;

public class ClientMock
{
    private SimpleSocketTcpClient _client;
    private string _ip;
    private int _port;
    
    public ClientMock(string ip, int port)
    {
        _ip = ip;
        _port = port;
        _client = new SimpleSocketTcpClient();
    }

    public async Task Start()
    {
        _client.StartClient(_ip, _port);
        for (var i = 0; i < 100; i++)
        {
            await _client.SendMessageAsync(i.ToString());
            await Task.Delay(1);
        }
        _client.Close();
        await Task.Delay(10);
    }
}