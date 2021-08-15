FROM mcr.microsoft.com/dotnet/runtime:6.0-alpine
COPY ../build/Telegram App/
WORKDIR /App
ENTRYPOINT ["dotnet", "Cards.Telegram.dll"]