# FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
# WORKDIR /app

# COPY *.csproj ./
# RUN dotnet restore
# COPY . ./
# RUN dotnet publish -c Release

# FROM mcr.microsoft.com/dotnet/sdk:5.0
# WORKDIR /app
# EXPOSE 80
# COPY --from=build-env /app/out .
# ENTRYPOINT [ "dotnet", "Daurecard.dll" ]

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Daurecard.csproj", ""]
RUN dotnet restore "./Daurecard.csproj"
#RUN dotnet restore
COPY . .
WORKDIR "/src/."
RUN dotnet build "Daurecard.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Daurecard.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Daurecard.dll"]
#CMD ASPNETCORE_URLS=http://*:$PORT dotnet Daurecard.dll