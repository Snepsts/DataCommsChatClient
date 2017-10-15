/******************************************************************************
            SimpleEchoClient.cs - Simple Echo client using sockets

  Copyright 2012 by Ziping Liu for VS2010
  Prepared for CS480, Southeast Missouri State University

            SimpleChatClient.cs - Simple Chat client using sockets

  This program demonstrates the use of Sockets API to connect to a chat service,
  send commands to that service using a socket interface, and receive responses
  from that service. The user interface is via a MS Dos window.

  This program has been compiled and tested under Microsoft Visual Studio 2017.

  Copyright 2017 by Michael Ranciglio for VS2017
  Prepared for CS480, Southeast Missouri State University

******************************************************************************/
/*-----------------------------------------------------------------------
 *
 * Program: SimpleChatClient
 * Purpose: contact chatserver, send user input and receive/print server response
 * Usage:   SimpleChatClient <compname> [portnum]
 * Note:    <compname> can be either a computer name, like localhost, xx.cs.semo.edu
 *          or an IP address, like 150.168.0.1
 *
 *-----------------------------------------------------------------------
 */

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class SimpleChatClient
{
	private static async Task<string> GetInputAsync() //method to grab input async (type while receiving messages)
	{
		return await Task.Run(() => Console.ReadLine());
	}

	public static void Main(string[] args)
	{
		if ((args.Length < 1) || (args.Length > 2))
		{ // Test for correct # of args
			throw new ArgumentException("Parameters: <Server> <Port>");
		}

		IPHostEntry serverInfo = Dns.GetHostEntry(args[0]);//using IPHostEntry support both host name and host IPAddress inputs
		IPAddress[] serverIPaddr = serverInfo.AddressList; //addresslist may contain both IPv4 and IPv6 addresses

		byte[] data = new byte[1024];
		string input, msg, genMsg;
		Socket server;
		server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

		bool exit = false;
		DateTime currTime = DateTime.Now;
		genMsg = "SimpleChatClient log generated at: " + currTime + Environment.NewLine;
		using (System.IO.StreamWriter log = new System.IO.StreamWriter(@"C:\Users\Public\ChatClientLog.txt"))
		{
			log.WriteLine(genMsg);
			log.WriteLine(Environment.NewLine); //more lines to make it clear that the log starts below here

			try
			{
				server.Connect(serverIPaddr, Int32.Parse(args[1]));
			}
			catch (SocketException e)
			{
				string errorMsg = "Unable to connect to server." + Environment.NewLine + e.ToString();
				Console.WriteLine(errorMsg);
				log.WriteLine(errorMsg);

				return;
			}

			IPEndPoint serverep = (IPEndPoint)server.RemoteEndPoint;
			string connectMsg = "Connected with " + serverep.Address + " at port " + serverep.Port;
			Console.WriteLine(connectMsg);
			log.WriteLine(connectMsg);

			int recv = server.Receive(data); //this section grabs the first message (the welcome to the server message)
			msg = Encoding.ASCII.GetString(data, 0, recv);
			Console.WriteLine(msg);
			log.WriteLine(msg);

			while (true)
			{
				Task<string> T = GetInputAsync();

				while (!T.IsCompleted)
				{
					if (server.Available > 0)
					{
						Console.WriteLine();
						data = new byte[1024];
						recv = server.Receive(data);

						if (recv == 0) //server non-responsive
						{
							exit = true;
							break;
						}

						msg = Encoding.ASCII.GetString(data, 0, recv);

						if (msg == "exit")
						{
							string exitMsg = "Exit received, initiating disconnect.";
							Console.WriteLine(exitMsg);
							log.WriteLine(exitMsg);

							exit = true;
							break;
						}

						Console.WriteLine(msg);
						log.WriteLine(msg);
					}

					System.Threading.Thread.Sleep(50); //Wait for .05 seconds before checking again
				}

				if (exit)
					break;

				//when the while loop finishes, we know we have something to send

				input = T.Result;

				if (input.Length == 0)
					continue;

				if (input == "exit")
				{
					server.Send(Encoding.ASCII.GetBytes(input));
					log.WriteLine(input);
					string exitMsg = "Exit sent, initiating disconnect.";
					Console.WriteLine(exitMsg);
					log.WriteLine(exitMsg);

					break;
				}

				currTime = DateTime.Now;
				string prefix = "[" + currTime + "] client: "; //prepare our prefix to our message (time and name)
				msg = prefix + input; //create message
				server.Send(Encoding.ASCII.GetBytes(msg)); //send message
				Console.WriteLine(msg);
				log.WriteLine(msg); //record message
			}

			string disconnectMsg = "Disconnecting from server...";
			Console.WriteLine(disconnectMsg);
			log.WriteLine(disconnectMsg);
			server.Shutdown(SocketShutdown.Both);
			server.Close();
		}
	}
}
