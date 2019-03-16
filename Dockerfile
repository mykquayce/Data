FROM microsoft/dotnet:2.2-sdk as build-env
WORKDIR /app

COPY . ./
RUN dotnet restore Data.sln -s https://api.nuget.org/v3/index.json -s http://nuget/nuget
RUN dotnet publish Data.Api/Data.Api.csproj -c Release -o /app/publish -r linux-x64


FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
COPY --from=build-env app/publish .
ENTRYPOINT ["dotnet", "Data.Api.dll"]
