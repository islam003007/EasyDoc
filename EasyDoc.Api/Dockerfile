# See https://aka.ms/customizecontainer to learn how to customize your debug container.
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["EasyDoc.Api/EasyDoc.Api.csproj", "EasyDoc.Api/"]
COPY ["EasyDoc.Infrastructure/EasyDoc.Infrastructure.csproj", "EasyDoc.Infrastructure/"]
COPY ["EasyDoc.Core/EasyDoc.Application.csproj", "EasyDoc.Core/"]
COPY ["EasyDoc.Domain/EasyDoc.Domain.csproj", "EasyDoc.Domain/"]
COPY ["EasyDoc.SharedKernel/EasyDoc.SharedKernel.csproj", "EasyDoc.SharedKernel/"]
RUN --mount=type=cache,target=/root/.nuget/packages dotnet restore "./EasyDoc.Api/EasyDoc.Api.csproj"
COPY . .
WORKDIR "/src/EasyDoc.Api"
RUN dotnet build "./EasyDoc.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EasyDoc.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EasyDoc.Api.dll"]
