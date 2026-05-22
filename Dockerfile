FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["LibraryManagement.API/LibraryManagement.API.csproj",           "LibraryManagement.API/"]
COPY ["LibraryManagement.Core/LibraryManagement.Core.csproj",         "LibraryManagement.Core/"]
COPY ["LibraryManagement.Application/LibraryManagement.Application.csproj", "LibraryManagement.Application/"]
COPY ["LibraryManagement.Infrastructure/LibraryManagement.Infrastructure.csproj", "LibraryManagement.Infrastructure/"]
RUN dotnet restore "LibraryManagement.API/LibraryManagement.API.csproj"

COPY . .
RUN dotnet publish "LibraryManagement.API/LibraryManagement.API.csproj" \
    -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LibraryManagement.API.dll"]
