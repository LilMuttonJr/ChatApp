# Chat Console Application

Chat Console Application is a simple chat system implemented in C# for local network communication between clients and a server.

## Installation

### Server

1. Clone the repository for the server:
git clone https://github.com/LilMuttonJr/ChatApp.git

2. Navigate into the server directory:
cd App

3. Build the client (if necessary):
dotnet build

## Usage

### Server

1. Start the server:
dotnet run

2. The server will start listening for client connections on the local network.

### Client

1. Start a client instance on each computer within the local network:
dotnet run

2. When prompted, enter the server's IP address and port number to connect.

3. Begin chatting with other clients connected to the same server instance.

## Notes

- This application is designed to work on local networks only. It does not support connections over the internet.
- Ensure all clients and the server are on the same local network to communicate effectively.

## License

This project is licensed under the MIT License. See the [LICENSE](license) file for details.
