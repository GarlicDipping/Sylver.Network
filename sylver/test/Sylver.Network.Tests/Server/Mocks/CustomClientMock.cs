using Moq;
using Sylver.Network.Server;
using Sylver.Network.Tests.Mocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sylver.Network.Tests.Server.Mocks
{
    public sealed class CustomClientMock : Mock<CustomClient>
    {
        /// <summary>
        /// Gets or sets the socket mock.
        /// </summary>
        public NetSocketMock SocketMock { get; }

        /// <summary>
        /// Creates a new <see cref="CustomClientMock"/> instance.
        /// </summary>
        public CustomClientMock()
            : base(null)
        {
            SocketMock = new NetSocketMock();

            SetupGet(x => x.Socket).Returns(SocketMock);
        }
    }
}
