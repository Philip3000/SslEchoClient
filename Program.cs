using System;



namespace SslEchoClient
{
    class Program
    {
        private static bool _isClientConnected;
        private static bool _isServerAuthenticated;
        static void Main(string[] args)
        {
            SslClient sslclient = new SslClient("localhost", 6789);

            _isClientConnected = sslclient.Connect();

            if (_isClientConnected)
            {
                _isServerAuthenticated = sslclient.AuthenticateAsClient();

            }

            if (_isServerAuthenticated)
            {
                sslclient.Talk();
            }

        }
    }
}