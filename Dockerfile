FROM mcr.microsoft.com/dotnet/core/sdk:latest as build-env
WORKDIR /src

COPY . ./
RUN dotnet restore Data.sln -s https://api.nuget.org/v3/index.json -s http://nuget/nuget
RUN dotnet publish Data.Api/Data.Api.csproj -c Release -o /publish -r linux-x64


FROM mcr.microsoft.com/dotnet/core/aspnet:latest
WORKDIR /app
COPY --from=build-env /publish .
ENTRYPOINT ["dotnet", "Data.Api.dll"]
