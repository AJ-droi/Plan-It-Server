FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5261

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["Plan-It/Plan-It.csproj", "Plan-It/"]
RUN dotnet restore "Plan-It/Plan-It.csproj"
COPY . .
WORKDIR "/src/Plan-It"
RUN dotnet build "Plan-It.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Plan-It.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Plan-It.dll"]
