FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY ./publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:80

ENTRYPOINT ["dotnet", "Masa.Tsc.Service.Admin.dll"]
