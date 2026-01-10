FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

COPY ["WorkoutOptimization.Endpoint/WorkoutOptimization.Endpoint.csproj", "WorkoutOptimization.Endpoint/"]
COPY ["WorkoutOptimization.Logic/WorkoutOptimization.Logic.csproj", "WorkoutOptimization.Logic/"]
COPY ["WorkoutOptimization.Repository/WorkoutOptimization.Repository.csproj", "WorkoutOptimization.Repository/"]
COPY ["WorkoutOptimization.Models/WorkoutOptimization.Models.csproj", "WorkoutOptimization.Models/"]

RUN dotnet restore "WorkoutOptimization.Endpoint/WorkoutOptimization.Endpoint.csproj"

COPY . .

WORKDIR /app/WorkoutOptimization.Endpoint
RUN dotnet build "WorkoutOptimization.Endpoint.csproj" -c Release -o /app/build /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app
COPY --from=build /app/build .

ENV ASPNETCORE_URLS=http://+:5135

ENTRYPOINT [ "dotnet", "WorkoutOptimization.Endpoint.dll" ]