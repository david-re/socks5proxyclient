using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Net;
using System.Net.Sockets;
namespace socks5proxyclient {
    class Program {
        static void Main(string[] args) {
            var proxyAddr = "218.62.97.105:1080";
            var proxyIp = proxyAddr.Split(':')[0];
            var proxyPort = int.Parse(proxyAddr.Split(':')[1]);
            var client = new TcpClient(proxyIp, proxyPort);

            Console.WriteLine("Connected to proxy: {0} at port: {1}", proxyIp, proxyPort);

            var connection = client.GetStream();
            var writer = new BinaryWriter(connection);
            var reader = new BinaryReader(connection);
            writer.Write(new byte[] { 0x05, 0x01, 0x00 });
            var response = reader.ReadBytes(2);

            if (response[1] != 0x00)
                throw new WebException("Proxy doesn't support connection without authentication.");

            writer.Write(new byte[] { 0x05, 0x01, 0x00, 0x01 });

            var ip = "173.194.35.152";
            foreach (var part in ip.Split('.'))
                writer.Write(byte.Parse(part));

            var port = 80;
            writer.Write(IntToNetworkByteOrder(port));

            response = reader.ReadBytes(10);
            var errorCode = response[1];

            if (errorCode != 0)
                throw new WebException("Connection to proxy failed. Errorcode " + errorCode);

            Console.WriteLine("Connected to target over proxy.");
        }

        private static byte[] IntToNetworkByteOrder(int port) {
            var hb = port / 256;
            var lb = port % 256;
            return new byte[] { (byte)hb, (byte)lb };
        }
    }
}