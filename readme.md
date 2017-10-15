# Chat Client
#### By: Michael Ranciglio and Will Mertz

## How to use
### Building
This project was built in Microsoft Visual Studio, so you'll want to open it in
Visual Studio and compile it there. It is assumed you can figure that out
yourself.

NOTE: This project was designed to be used in conjunction with the
DataCommsChatServer project, so you'll want to have that too. You'll also want
to run this after running that one.

### Running
* Ensure you already have ran the chat server binary and it is waiting for a
client.
* Open the Windows command prompt. (Search for cmd)
* Move to the directory with the chat client binary.
* The program takes two arguments:
	* The first for the IP of the server.
		* If you're using this on your computer only, you'll want to put
		localhost for this parameter.
	* The second for the port/application number you run it on.
		* We recommended using something around 2000x (x being 0-9), make sure
		you use the same number for this client as you did for the server.
	* i.e. DataCommsChatClient.exe localhost 20003
* You should be good to send and receive messages now!!!
* You can exit the application by sending the message "exit", or receiving an
"exit" from the server.
* A chat log is written in C:\Public\Users\ChatClientLog.txt
