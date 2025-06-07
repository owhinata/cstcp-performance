#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <netinet/tcp.h>
#include <sys/time.h>
#include <errno.h>
#include <time.h>

#define PORT 5000
#define BUFFER_SIZE 512
#define SEND_COUNT 1000

static void fill_random(unsigned char* buf, size_t len)
{
    for (size_t i = 0; i < len; ++i) {
        buf[i] = rand() % 256;
    }
}

ssize_t send_all(int fd, const void* buf, size_t len)
{
    const unsigned char* p = buf;
    size_t total = 0;
    while (total < len) {
        ssize_t n = send(fd, p + total, len - total, 0);
        if (n <= 0) {
            return n;
        }
        total += n;
    }
    return total;
}

int main()
{
    srand((unsigned int)time(NULL));

    int listener = socket(AF_INET, SOCK_STREAM, 0);
    if (listener < 0) {
        perror("socket");
        return 1;
    }

    int opt = 1;
    setsockopt(listener, SOL_SOCKET, SO_REUSEADDR, &opt, sizeof(opt));
    setsockopt(listener, IPPROTO_TCP, TCP_NODELAY, &opt, sizeof(opt));

    struct sockaddr_in addr;
    memset(&addr, 0, sizeof(addr));
    addr.sin_family = AF_INET;
    addr.sin_addr.s_addr = INADDR_ANY;
    addr.sin_port = htons(PORT);

    if (bind(listener, (struct sockaddr*)&addr, sizeof(addr)) < 0) {
        perror("bind");
        close(listener);
        return 1;
    }

    if (listen(listener, 5) < 0) {
        perror("listen");
        close(listener);
        return 1;
    }

    printf("TCP Server started on port %d.\n", PORT);

    while (1) {
        struct sockaddr_in client_addr;
        socklen_t addrlen = sizeof(client_addr);
        int client = accept(listener, (struct sockaddr*)&client_addr, &addrlen);
        if (client < 0) {
            perror("accept");
            continue;
        }
        printf("Client connected.\n");

        setsockopt(client, IPPROTO_TCP, TCP_NODELAY, &opt, sizeof(opt));

        unsigned char buf[BUFFER_SIZE];
        fill_random(buf, BUFFER_SIZE);

        struct timeval start, end;
        gettimeofday(&start, NULL);

        for (int i = 0; i < SEND_COUNT; ++i) {
            if (send_all(client, buf, BUFFER_SIZE) <= 0) {
                perror("send");
                goto cleanup;
            }
        }

        unsigned char resp[16];
        ssize_t received = 0;
        while (received == 0) {
            received = recv(client, resp, sizeof(resp), 0);
            if (received < 0) {
                if (errno == EINTR)
                    continue;
                perror("recv");
                goto cleanup;
            }
            if (received == 0) {
                goto cleanup;
            }
        }

        gettimeofday(&end, NULL);

        double elapsed = (end.tv_sec - start.tv_sec) * 1000.0;
        elapsed += (end.tv_usec - start.tv_usec) / 1000.0;

        char result[64];
        int len = snprintf(result, sizeof(result), "%.3f", elapsed);
        send_all(client, result, len);

    cleanup:
        close(client);
        printf("Client disconnected.\n");
    }

    close(listener);
    return 0;
}

