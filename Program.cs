using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace KZT_5_Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("MachineName: {0}", Environment.MachineName);

            // Data buffer for incoming data.
            byte[] bytes = new byte[1024];
            Socket sender;
            IPHostEntry ipHostInfo;
            IPAddress ipAddress;
            IPEndPoint remoteEp;

            // Connect to a remote device.
            try
            {
                // Establish the remote endpoint for the socket.
                // This example uses port 11000 on the local computer.
                ipHostInfo = Dns.GetHostEntry(args[0]);
                ipAddress = ipHostInfo.AddressList[0];
                remoteEp = new IPEndPoint(ipAddress, 11000);

                // Create a TCP/IP  socket.
                sender = new Socket(AddressFamily.InterNetworkV6,
                    SocketType.Stream, ProtocolType.Tcp);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return;
            }

            // Connect the socket to the remote endpoint. Catch any errors.
            try
            {
                sender.Connect(remoteEp);

                Console.WriteLine("Socket connected to {0}",
                    sender.RemoteEndPoint);

                // Encode the data string into a byte array.
                byte[] msg = Encoding.ASCII.GetBytes(Environment.MachineName);

                // Send the data through the socket.
                sender.Send(msg);

                // Receive the response from the remote device.
                var bytesRec = sender.Receive(bytes);

                // If no work for this client
                if (bytesRec == 0)
                {
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                    Console.WriteLine("Nothing to do.");
                    return;
                }

                // Convert received data to list
                var receivedRow = new List<byte>();
                for (var i = 0; i < bytesRec; i++)
                {
                    receivedRow.Add(bytes[i]);
                }

                Console.WriteLine("\nI received:");
                foreach (var item in receivedRow)
                {
                    Console.Write($"{item,4}");
                }
                Console.WriteLine();

                /*Sum elements.*/
                var result = new List<byte>();
                for(var i = 0; i < receivedRow.Count / 2; i++)
                {
                    result.Add((byte)(receivedRow[i] + receivedRow[(receivedRow.Count / 2) + i]));
                }

                Console.WriteLine("\nSum row is:");
                foreach (var item in result)
                {
                    Console.Write($"{item,4}");
                }
                Console.WriteLine();

                // Send sorted data back to server
                sender.Send(result.ToArray());
                Console.WriteLine("And sended that back to server");


                // Release the socket.
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane);
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e);
            }

            
        }
    }
}
