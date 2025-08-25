namespace Core.Signals
{
    public class OnStartHostSignal
    {
        public string Nickname;

        public OnStartHostSignal(string nickname) => Nickname = nickname;
    }
}