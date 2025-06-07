# cstcp-performance

Simple TCP client and server used for measuring transfer performance. Build the managed implementations with the .NET SDK:

```bash
dotnet build TcpServer/TcpServer.csproj -c Release
dotnet build TcpClient/TcpClient.csproj -c Release
```

To build the Rust client, use Cargo:

```bash
cargo build --release --manifest-path TcpClientRust/Cargo.toml
```

To build the native Linux server, use CMake:

```bash
cmake -S TcpServerNative -Bbuild -DCMAKE_BUILD_TYPE=Release && cmake --build build
```

## Package Installation

```bash
winget install --id Microsoft.DotNet.SDK.9
winget install --id Rustlang.Rustup
winget install --id Microsoft.VisualStudio.2022.BuildTools -e  # Enable C++ build tools
```

## Results

CPU: AMD Ryzen 7 8840U w/ Radeon 780M Graphics
Ethernet: 2.5 Gbps

| Client | Server | Result |
| ------ | ------ | ------ |
| TcpClient [C#]       | TcpServerNative [Linux] | 1298.641 ms |
| TcpClientRust [Rust] | TcpServerNative [Linux] | 1295.753 ms |
