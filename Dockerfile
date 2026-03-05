# Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "StudentAttendance.csproj"
RUN dotnet publish "StudentAttendance.csproj" -c Release -o /app/publish

# Run
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:54812
EXPOSE 54812
ENTRYPOINT ["dotnet", "StudentAttendance.dll"]