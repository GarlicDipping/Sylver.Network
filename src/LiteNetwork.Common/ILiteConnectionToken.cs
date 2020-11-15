using LiteNetwork.Protocol;
using System.Net.Sockets;

namespace LiteNetwork.Common
{
    public interface ILiteConnectionToken
    {
        ILiteConnection Connection { get; }

        Socket Socket { get; }

        LiteDataToken DataToken { get; }
    }
}
