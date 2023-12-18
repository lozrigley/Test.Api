FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5007

ENV ASPNETCORE_URLS=http://+:5007;;

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "Test.Api/Test.Api.csproj"
COPY . .
WORKDIR "/src/Test.Api"
RUN dotnet build "Test.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Test.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Test.Api.dll"]
