using Moq;
using Sylver.Network.Data;

namespace Sylver.Network.Tests.Mocks
{
    internal sealed class NetPacketProcessorMock : Mock<IPacketProcessor>
    {
        /// <summary>
        /// Creates a new <see cref="NetPacketProcessorMock"/> instance.
        /// </summary>
        /// <param name="includeHeader">Include packet header.</param>
        public NetPacketProcessorMock(bool includeHeader)
        {
            SetupGet(x => x.HeaderSize).Returns(sizeof(int));
            SetupGet(x => x.IncludeHeader).Returns(includeHeader);
        }
    }
}
