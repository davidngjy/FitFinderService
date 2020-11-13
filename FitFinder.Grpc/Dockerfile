#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
ENV sql_username=
ENV sql_password=
ENV ASPNETCORE_ENVIRONMENT=
LABEL org.opencontainers.image.authors DavidNg
LABEL org.opencontainers.image.source https://github.com/davidngjy/FitFinderService
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["FitFinder.Grpc/FitFinder.Grpc.csproj", "FitFinder.Grpc/"]
COPY ["FitFinder.Application/FitFinder.Application.csproj", "FitFinder.Application/"]
COPY ["FitFinder.Domain/FitFinder.Domain.csproj", "FitFinder.Domain/"]
COPY ["FitFinder.Gateway/FitFinder.Gateway.csproj", "FitFinder.Gateway/"]
COPY ["FitFinder.Infrastructure/FitFinder.Infrastructure.csproj", "FitFinder.Infrastructure/"]
RUN dotnet restore "FitFinder.Grpc/FitFinder.Grpc.csproj"
COPY . .
WORKDIR "/src/FitFinder.Grpc"
RUN dotnet build "FitFinder.Grpc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FitFinder.Grpc.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FitFinder.Grpc.dll"]