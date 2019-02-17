# Standalone Pinterest App with WCF

Features Pinterest profile and Pin Notes features using POST method. Windows Communication Foundation (WCF) is integrated to create a service and able to send the signin profile remotely to client.

# How to run the codes

1. Get the client ID and client secret by registering your app to https://developers.pinterest.com/
2. Once you obtain all the information, add it to the line of codes at 'Login.cs'

        public const string clientId = "YOUR CLIENT ID";
        public const string clientSecret = "YOUR CLIENT SECRET";
        public const string redirectUri = "YOUR REDIRECT URI";
        
# Windows Communication Foundation (WCF)

Run the Client side code to get the user profile remotely when service is hosted.
