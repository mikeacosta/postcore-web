FROM mcr.microsoft.com/dotnet/core/runtime:2.2

WORKDIR /Postcore.Web
COPY Postcore.Web/bin/Release/netcoreapp2.2/publish .
 
ENV ASPNETCORE_URLS http://+:5000
EXPOSE 5000
 
ENTRYPOINT ["dotnet", "mymvcweb.dll"]