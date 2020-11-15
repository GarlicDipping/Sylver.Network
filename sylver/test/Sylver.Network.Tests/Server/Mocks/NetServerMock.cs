using System;
using System.Linq.Expressions;
using Moq;
using Moq.Protected;
using Sylver.Network.Server;

namespace Sylver.Network.Tests.Server.Mocks
{
    public sealed class NetServerMock<TClient> : Mock<NetServer<TClient>>
        where TClient : class, INetServerClient
    {
        public bool BeforeStartCalled { get; private set; }

        public bool AfterStartCalled { get; private set; }

        public bool BeforeStopCalled { get; private set; }

        public bool AfterStopCalled { get; private set; }

        public NetServerMock(params object[] args)
            : base(args)
        {
            SetupMethods();
        }

        public NetServerMock(MockBehavior behavior)
            : base(behavior)
        {
            SetupMethods();
        }

        public NetServerMock(MockBehavior behavior, params object[] args)
            : base(behavior, args)
        {
            SetupMethods();
        }

        public NetServerMock(Expression<Func<NetServer<TClient>>> newExpression, MockBehavior behavior = MockBehavior.Loose)
            : base(newExpression, behavior)
        {
            SetupMethods();
        }

        public void VerifyOnBeforeStart(Times times)
        {
            this.Protected().Verify("OnBeforeStart", times);
        }
        public void VerifyOnAfterStart(Times times)
        {
            this.Protected().Verify("OnAfterStart", times);
        }

        public void VerifyOnBeforeStop(Times times)
        {
            this.Protected().Verify("OnBeforeStop", times);
        }

        public void VerifyOnAfterStop(Times times)
        {
            this.Protected().Verify("OnAfterStop", times);
        }

        public void VerifyOnClientConnected(Times times)
        {
            this.Protected().Verify("OnClientConnected", times, ItExpr.IsAny<TClient>());
        }

        public void VerifyOnClientDisconnected(Times times)
        {
            this.Protected().Verify("OnClientDisconnected", times, ItExpr.IsAny<TClient>());
        }

        private void SetupMethods()
        {
            this.Protected().Setup("OnBeforeStart").Callback(() => BeforeStartCalled = true);
            this.Protected().Setup("OnAfterStart").Callback(() => AfterStartCalled = true);
            this.Protected().Setup("OnBeforeStop").Callback(() => BeforeStopCalled = true);
            this.Protected().Setup("OnAfterStop").Callback(() => AfterStopCalled = true);
        }
    }
}
