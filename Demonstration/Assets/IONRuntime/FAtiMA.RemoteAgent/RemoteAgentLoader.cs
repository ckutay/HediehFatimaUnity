using System;
using System.Text;
using System.Net.Sockets;
using ION.Core;
using System.Diagnostics;
using System.Net;
using ION.Core.Events;
using ION.Meta;

namespace FAtiMA.RemoteAgent
{
    public class LoadRemoteAgentArgs
    {
        public enum LoadMode {CREATENEW,RESTORE};

        public bool DebugMode { get; private set; }
        public LoadMode Mode { get; private set; }
        public string LanguageActs { get; private set; }
        public string UserLanguageActs { get; private set; }
        public string DisplayName { get; private set; }
        public string UserDirectory { get; private set; }
        public string Scenario { get; private set; }
        public string ScenarioPath { get; private set; }
		public string ScenariosFile { get; private set; }
		public int Port { get; private set; }

        public LoadRemoteAgentArgs(bool debugMode,LoadMode mode, int port, string scenarioPath, string scenariosFile, string scenario, string languageActs, string userLanguageActs, string displayName, string userDirectory)
        {
            this.DebugMode = debugMode;
            this.Mode = mode;
            this.LanguageActs = languageActs;
            this.UserLanguageActs = userLanguageActs;
            this.DisplayName = displayName;
            this.UserDirectory = userDirectory;
            this.Scenario = scenario;
			this.ScenarioPath = scenarioPath;
            this.ScenariosFile = scenariosFile;
			this.Port = port;
        }
    }

    public class RemoteAgentLoader 
    {
        
        public const string SEX = "sex";
        public const string ROLE = "role";
        

        //This parameter is not used in the action
        //It is here just so the SF can get it from the RunningContext of the action
        public const string NAME = "Name";
        public const string ACTION_NAME = "LoadRemoteAgent";

        protected const string EOM_TAG = "\n";
        protected const string EOF_TAG = "<EOF>";
        //only one connection at a time
        protected const int PENDING_CONNECTION_QUEUE_LENGTH = 10;
        protected const int INITIAL_SERVER_PORT = 46874;
        protected const int SERVER_PORT_RANGE = 100;
        protected const int bufferSize = 1024;

        protected Socket serverSocket;
        protected byte[] buffer = new byte[bufferSize];
        protected static int currentServerPort = INITIAL_SERVER_PORT;

        protected RemoteMind remoteAgent;

        public RemoteAgentLoader(RemoteMind remoteAgent)
        {
            this.remoteAgent = remoteAgent;
        }
		
		// Henrique Campos - changed this in order to work on multiple OS and to receive ports from elsewhere
        public void Launch(LoadRemoteAgentArgs args)
	    {
			
	        StartServer(args.Port);
	        //once the Server is on, we can launch the java proccess
			
			Process proc = new Process();
		
			OperatingSystem osInfo = Environment.OSVersion;
			
			if(osInfo.Platform == PlatformID.Unix){ 
				LaunchUnix(args.Port, proc, args);
			}
			else if(osInfo.Platform == PlatformID.MacOSX){
				LaunchUnix(args.Port, proc, args);
			}
			else {
				LaunchWindows(args.Port, proc, args);
			}
			
			
			
			// Redirect the standard output of the compile command.
			// Synchronously read the standard output of the spawned process.
			//string output = proc.StandardOutput.ReadToEnd();
			//if (output != null)
			//{
			//	Console.WriteLine(output);
			//}
			
			//output = proc.StandardError.ReadLine();
			//if (output != null)
			//{
			//	Console.WriteLine(output);
			//}
			
			
			
		
			//proc.StandardOutput.ReadLine();
			//Process.EnterDebugMode();
			//Process p2 = new Process();
			//p2.StartInfo.CreateNoWindow = true;
			//p2.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
			//p2.StartInfo.UseShellExecute = true;
			//p2.StartInfo.FileName = "read";
			//p2.Start();
			
			ApplicationLogger.Instance().WriteLine("FAtiMA: Started Agent: " + proc.StartInfo.FileName + " " + proc.StartInfo.Arguments);
	    }
		
		// Henrique Campos - made this Agent launcher for Windows 
		private void LaunchWindows(int port, Process proc, LoadRemoteAgentArgs args){
			// For Windows:
			ApplicationLogger.Instance().WriteLine("FAtiMA: Launching Agent for Windows..");
	        proc.StartInfo.FileName ="cmd";
	        proc.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
	        proc.StartInfo.UseShellExecute = true;
	
	        if (args.Mode == LoadRemoteAgentArgs.LoadMode.RESTORE && System.IO.File.Exists(args.UserDirectory + this.remoteAgent.Body.Name))
	        {
	            proc.StartInfo.Arguments = "-cp \"FAtiMA.jar;Language.jar\" " +
	            "FAtiMA.Agent " +
	            "localhost " +
	            port + " " +
	            args.UserDirectory + " " +
	            this.remoteAgent.Body.Name;
	        }
	        else
	        {
	            proc.StartInfo.Arguments = "/K java -cp \"FAtiMA-Modular.jar;xmlenc-0.52.jar" +     
	                "\"" +
	                " FAtiMA.AgentLauncher " + args.ScenarioPath + " "+ args.ScenariosFile + " " + args.Scenario + " " + this.remoteAgent.Body.Name;

			}
			
			proc.Start();
		}
		
		// Henrique Campos - made this Agent launcher for Unix and Mac 
		private void LaunchUnix(int port, Process proc, LoadRemoteAgentArgs args){
			// For Mac:
			String fatimaArguments;
			ApplicationLogger.Instance().WriteLine("FAtiMA: Launching Agent for Unix..");
			proc.StartInfo.FileName = "java";
			//proc.StartInfo.Arguments = "/Applications/Utilities/Terminal.app/Contents/MacOS/Terminal";
			proc.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
			//proc.StartInfo.RedirectStandardInput = true;
			proc.StartInfo.CreateNoWindow = true;
			proc.StartInfo.UseShellExecute = true;
			//proc.StartInfo.RedirectStandardOutput = true;
			//proc.StartInfo.RedirectStandardInput = true;
			//proc.StartInfo.RedirectStandardError = true;
			
			if (args.Mode == LoadRemoteAgentArgs.LoadMode.RESTORE && System.IO.File.Exists(args.UserDirectory + this.remoteAgent.Body.Name))
	        {
	           fatimaArguments = "-cp FAtiMA.jar:Language.jar " +
	            "FAtiMA.Agent " +
	            "localhost " +
	            port + " " +
	            args.UserDirectory + " " +
	            this.remoteAgent.Body.Name;
	        }
	        else
	        {
	            fatimaArguments = "-cp FAtiMA-Modular.jar:xmlenc-0.52.jar " +
	                "FAtiMA.AgentLauncher " +args.ScenarioPath + " "+ args.ScenariosFile + " " + args.Scenario + " " + this.remoteAgent.Body.Name;
	        }
			
			proc.StartInfo.Arguments = fatimaArguments;
			
			proc.Start ();
			
//			proc.StandardInput.WriteLine(fatimaArguments + "\r\n");
//			proc.StandardInput.Flush();
		}
        
        
        #region RemoteConnection

        protected void StartServer(int port)
        {
            // Establish local endpoint...
            ApplicationLogger.Instance().WriteLine("FAtiMA: Creating the socket server..");
            IPAddress ipAddress = IPAddress.Any;
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
            // Create a TCP/IP socket... (Henrique Campos - changes the AddressFamily.Unspecified to ..
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // Bind the socket...
            this.serverSocket.Bind(localEndPoint);
            this.serverSocket.Listen(PENDING_CONNECTION_QUEUE_LENGTH);

            // accept new connection...
            this.serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), this.serverSocket);
            // wait until a connection is made before continuing...
            ApplicationLogger.Instance().WriteLine("FAtiMA: Comm ready!");
        }

        protected void AcceptCallback(IAsyncResult ar)
        {
            // get the socket handler...
            try
            {
                this.remoteAgent.Socket = ((Socket)ar.AsyncState).EndAccept(ar);
                ApplicationLogger.Instance().WriteLine("FAtiMA: Incoming connection ...");

                // create the state object...

                StringBuilder data = new StringBuilder();
                // begin receiving the connection request
                this.remoteAgent.Socket.BeginReceive(this.buffer, 0, bufferSize, 0,
                    new AsyncCallback(ReceiveCallback), data);

                //now that we received the connection, we can stop the serversocket
                ApplicationLogger.Instance().WriteLine("FAtiMA: Shuting Down...");
                this.serverSocket.Close();

            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
            }
        }

        protected void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                String receivedMsg = String.Empty;
                // read data from remote device...
                int bytesRead = +this.remoteAgent.Socket.EndReceive(ar);
                if (bytesRead > 0)
                {
                    // there may be more...
                    StringBuilder data = (StringBuilder)ar.AsyncState;
                    data.Append(Encoding.UTF8.GetString(this.buffer, 0, bytesRead));
                    // check for EOM.
                    receivedMsg = data.ToString();
                    int EomIndex = receivedMsg.IndexOf(EOM_TAG);
                    if (EomIndex > -1)
                    {
                        // finished receiving...
                        receivedMsg = receivedMsg.Substring(0, EomIndex);
                        // create the corresponding character
                        if (receivedMsg.StartsWith(this.remoteAgent.Body.Name))
                        {
                            //everything is ok, the agent that connected is the right agent
                            this.remoteAgent.Start();
                            //Aqui tenho de lançar o evento de que a mente acabou de se ligar com sucesso
                        }
                        else
                        {
                            //Serious error - someone else tried to connect to this RemoteCharacter
                            this.remoteAgent.Socket.Close();
                            this.remoteAgent.Socket = null;
                            //lançar um evento a dizer que houve merda
                        }
                    }
                    else
                    {
                        // not all data read. Read more...
                        this.remoteAgent.Socket.BeginReceive(this.buffer, 0, bufferSize, 0,
                            new AsyncCallback(ReceiveCallback), data);
                    }
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
            }
        }
        #endregion RemoteConnection
    }
}
