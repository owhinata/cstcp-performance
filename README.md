# cstcp-performance

Simple TCP client and server used for measuring transfer performance. Build the managed implementations with the .NET SDK:

```bash
dotnet build TcpServer/TcpServer.csproj
dotnet build TcpClient/TcpClient.csproj
```

To build the Rust client, use Cargo:

```bash
cargo build --release --manifest-path TcpClientRust/Cargo.toml
```

To build the native Linux server, use CMake:

```bash
cmake -S TcpServerNative -B build && cmake --build build
```
