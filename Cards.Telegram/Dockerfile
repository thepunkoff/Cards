FROM mcr.microsoft.com/dotnet/runtime:5.0-alpine
COPY ../build/Telegram App/
WORKDIR /App
ENTRYPOINT ["dotnet", "Cards.Telegram.dll"]