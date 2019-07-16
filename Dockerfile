# from command line, run -> dotnet publish -o ./publish
# to execute this file, run -> docker build -t postcore .
# to test container:
# docker run --name postcore --env ASPNETCORE_ENVIRONMENT=Development -p 80:80 postcore:latest
# or
# docker run -p 5000:80 postcore

FROM microsoft/dotnet:2.2.0-aspnetcore-runtime

WORKDIR /app
COPY ./Postcore.Web/publish .
 
ENTRYPOINT ["dotnet", "Postcore.Web.dll"]