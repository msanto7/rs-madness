FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY RSMadnessEngine.Api/RSMadnessEngine.Api.csproj RSMadnessEngine.Api/
COPY RSMadnessEngine.Data/RSMadnessEngine.Data.csproj RSMadnessEngine.Data/
RUN dotnet restore RSMadnessEngine.Api/RSMadnessEngine.Api.csproj

COPY . .
RUN dotnet publish RSMadnessEngine.Api/RSMadnessEngine.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "RSMadnessEngine.Api.dll"]
