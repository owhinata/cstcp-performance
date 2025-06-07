cmake_minimum_required(VERSION 3.10)
project(TcpServerNative C)

set(CMAKE_C_STANDARD 11)

add_executable(TcpServerNative server.c)
