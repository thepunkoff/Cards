FROM alpine:3.14 AS grpc-build
WORKDIR /opt
RUN apk add --update alpine-sdk autoconf libtool linux-headers cmake git && \ 
    \
    git clone -b v1.36.4 https://github.com/grpc/grpc --depth 1 --shallow-submodules && \
    cd grpc && git submodule update --init --depth 1 && \
    \
    mkdir -p cmake/build && cd cmake/build && \
    \
    cmake -DCMAKE_BUILD_TYPE=RelWithDebInfo \
      -DgRPC_BACKWARDS_COMPATIBILITY_MODE=ON \
      -DgRPC_BUILD_TESTS=OFF \
      ../.. && \
    \
    make grpc_csharp_ext -j4 && \
    \
    mkdir -p /out && cp libgrpc_csharp_ext.* /out

FROM mcr.microsoft.com/dotnet/runtime:6.0-alpine as final
COPY --from=grpc-build /out/libgrpc_csharp_ext.so /App/libgrpc_csharp_ext.x64.so
COPY ../build/Server App/
WORKDIR /App
ENTRYPOINT ["dotnet", "Cards.Server.dll"]