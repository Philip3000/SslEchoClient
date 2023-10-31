using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SslEchoClient
{

    internal class SslClient
    {
        private string _targetHost;
        private int _targetPort;
        private TcpClient _clientSocket;
        private Stream _unsecureStream;
        private SslStream _sslStream;
        private bool _leaveInnerStreamOpen = false;
        private bool _checkCertificateRevocation = false;
        private SslProtocols _enabledSSLProtocols = SslProtocols.Tls;
        private X509CertificateCollection _certificateCollection;
        private RemoteCertificateValidationCallback _remoteCertificateValidationCallback;
        private LocalCertificateSelectionCallback _localCertificateSelectionCallback;
        public SslClient(string targetHost, int targetPort)
        {
            _targetHost = targetHost;
            _targetPort = targetPort;
            _certificateCollection = new X509CertificateCollection();
        }

        public bool Connect()
        {
            _clientSocket = new TcpClient(_targetHost, _targetPort);
            _unsecureStream = _clientSocket.GetStream();
            Console.WriteLine("Client Connected");
            return true;
        }


        public bool AuthenticateAsClient()
        {
            _remoteCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);
            _sslStream = new SslStream(_unsecureStream, _leaveInnerStreamOpen, _remoteCertificateValidationCallback);
            _sslStream.AuthenticateAsClient(_targetHost, _certificateCollection, _enabledSSLProtocols, _checkCertificateRevocation);
            Console.WriteLine("Client Authenticated");
            return true;
        }
        public void Talk()
        {
            StreamReader sr = new StreamReader(_sslStream);
            StreamWriter sw = new StreamWriter(_sslStream);
            sw.AutoFlush = true; 

            string message = Console.ReadLine();

            while (message != "by")
            {
                sw.WriteLine(message);
                string serverAnswer = sr.ReadLine();
                Console.WriteLine("Server: " + serverAnswer);
                message = Console.ReadLine();
            }

            _sslStream.Close();
            _clientSocket.Close();
        }
        private bool ValidateServerCertificate(object sender, X509Certificate serverCertificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            Console.WriteLine("Client Sender: " + sender.ToString());
            Console.WriteLine("Server [subject] and CA [issuer] certificate params : " +
           serverCertificate.ToString());
            Console.WriteLine("SSL Policy errors: " + sslPolicyErrors.ToString());
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                Console.WriteLine("Client validation of server certificate successful.");
                return true;
            }
            Console.WriteLine("Errors in certificate validation:");
            Console.WriteLine(sslPolicyErrors);
            return false;
        }
        private X509Certificate CertificateSelectionCallback(object sender, string targetHost, X509CertificateCollection localCollection, X509Certificate remoteCertificate, string[] acceptableIssuers)
        {
            return localCollection[0];
        }
    }
}
