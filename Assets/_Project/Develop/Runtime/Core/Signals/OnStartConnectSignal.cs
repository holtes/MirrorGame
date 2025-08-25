namespace Core.Signals
{
    public class OnStartConnectSignal
    {
        public string Nickname;

        public OnStartConnectSignal(string nickname) => Nickname = nickname;
    }
}