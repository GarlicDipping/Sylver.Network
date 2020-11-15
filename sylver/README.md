# Sylver.Network

[![Build Status](https://travis-ci.org/Eastrall/Sylver.Network.svg?branch=master)](https://travis-ci.org/Eastrall/Sylver.Network)
[![codecov](https://codecov.io/gh/Eastrall/Sylver.Network/branch/master/graph/badge.svg)](https://codecov.io/gh/Eastrall/Sylver.Network)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/efaa4d26423845a8ac80445d1371e40d)](https://www.codacy.com/manual/Eastrall/Sylver.Network?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=Eastrall/Sylver.Network&amp;utm_campaign=Badge_Grade)
[![Nuget](https://img.shields.io/nuget/v/Sylver.Network)](https://www.nuget.org/packages/Sylver.Network/)

`Sylver.Network` is a simple and fast networking library built with C# and the .NET Core Framework. It simplifies the creation of socket servers and clients over the TCP/IP protocol.

## Introduction

:information_source: `Sylver.Network` is a rewrite of my previous networking library named `Ether.Network`. This new version looks like the old one, but it has a better code structure, unit tests and performance improvements.

Just like the old version, `Sylver.Network` kept the same *"easy-to-use"* concept that allows you to create your own socket servers and/or socket client in a **few lines** of code.

## How to install

Install `Sylver.Network` through the Visual Studio project package manager or using the following command in the Package Manager Console:

```
$> Install-Package Sylver.Network
```

Or you can use the `dotnet` command:

```
$> dotnet add package Sylver.Network
```

## Features

### Server

- Server configuration
    - Listening host / port
    - Allocated bytes per clients connected
- Client management
- Broadcast messages to all connected clients or a given list of connected clients

### Client

- Client configuration
    - Remote server address / port
    - Allocated bytes for the receive and send operations
    - Retry configuration (One time, limited time, infinite)
- Connect to remote server
- Disconnect from the remote server
- Send/Receive messages to/from the server

### Packets

- Create packet streams
- Read data from packet streams
- Customize your packet structure with a packet processor

## Thanks

**Thank you** to everyone who has contributed to `Ether.Network` with features, bug fixes and suggestions to make the Ether library better! All contributions have been integrated to `Sylver.Network` (and reworked to fit the library design).
