# Unity Authentication with Firebase (Google, Facebook, Telephone, Email) and Database.

This a test project I made for learning Firebase integration with unity.

It's open source. :clap:

| [:sparkles: Getting Started](#getting-started) | [:rocket: Installation](#installation) |
| --------------- | -------- |

## Getting Started

Follow the below instructions to get started:

1. [Make sure you have all Requirements](#requirements)
2. Create an account in [google firebase](https://firebase.google.com) and create a new project
3. Create a Unity App and start the Authentication.
4. [Download Source Code](#download)
5. Create a new keystore on the Unity project
6. On the new app, add the fingerprints that you can get by using this command on the prompt from inside the java/bin folder: keytool -keystore path-to-debug-or-production-keystore -list -v
7. Copy the Web API ID from google sign-in methods to the Google Sign In Script

For Facebook Sign-in, create a developer account and start a new app, then add a new plataform to the app on the facebook developer console and fill the informations.
Then find the FacebookSettings on the Unity project and fill it with the facebook developer app informations. Also you will need to download [OpenSSL](https://code.google.com/archive/p/openssl-for-windows/downloads) and copy the bin folder to the java/bin folder.

## Requirements

Make sure you have the below requirements before starting:

- [Visual Studio Community](https://visualstudio.microsoft.com/pt-br/downloads/)
- Basic Knowledge about Unity and C#

## Installation

You can get access to the project source code by using one of the following ways:

- [:sparkles: Download Source Code](https://github.com/EricMarinho/Unity-Authentication-with-Firebase/archive/master.zip)
- Clone the repository locally:

```bash
git clone https://github.com/EricMarinho/Unity-Authentication-with-Firebase.git
```

## License

Made by [Ilidam](https://github.com/EricMarinho)
