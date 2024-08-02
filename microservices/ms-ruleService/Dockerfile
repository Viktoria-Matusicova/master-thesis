# Specify the base image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base

# Set the working directory
WORKDIR /app

# Expose the port
EXPOSE #port#

# Set environment variable
ENV ASPNETCORE_URLS=#url#

#Sets the base image for building the application
FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build

# Set the working directory
WORKDIR /src

#Copy the project file into the container.
COPY ["microservices/ms-ruleService/ruleService.csproj", "microservices/ms-ruleService/"]

#Restore the dependencies for the project
RUN dotnet restore "microservices/ms-ruleService/ruleService.csproj"

#Copy all the files from the current directory into the container
COPY . .

# Set the working directory
WORKDIR "/src/microservices/ms-ruleService"

#Build the application
RUN dotnet build "ruleService.csproj" -c Release -o /app/build

#Create a new image
FROM build AS publish

#Publish the application
RUN dotnet publish "ruleService.csproj" -c Release -o /app/publish /p:UseAppHost=false

#Sets the base image for the final image
FROM base AS final

# Set the working directory
WORKDIR /app

#Copy published application to the final image
COPY --from=publish /app/publish .

#Run the application
ENTRYPOINT ["dotnet", "ruleService.dll"]
