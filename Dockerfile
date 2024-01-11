FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build

WORKDIR /source/src/telegram-lotte-bot.CLI

COPY . /source

RUN dotnet publish -a x64 --use-current-runtime --self-contained false -c Release -o /app 
    

FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine AS final
WORKDIR /app

COPY --from=build /app .

ARG BotToken
ARG LotteAuth

ENV BotToken=$BotToken
ENV LotteAuth=$LotteAuth

ENTRYPOINT ["dotnet", "telegram-lotte-bot.CLI.dll"]